using AutoMapper;
using FCG.Application.Auth;
using FCG.Application.Mapping;
using FCG.Application.Producer;
using FCG.Application.Services;
using FCG.Core.Interfaces.Repository;
using FCG.Core.Interfaces.Utils;
using FCG.Core.Utils;
using FCG.Infra.Context;
using FCG.Infra.Repository;
using FCG.Infra.Seed;
using FCG.WebApi.Configuration;
using FCG.WebApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using System;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var serviceName = "user";

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://loki:3100",
        labels: new[] { new LokiLabel { Key = "service", Value = serviceName } })
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddPrometheusExporter());

#region BUILDER
builder.Services.AddProblemDetails();

builder.RegisterConfigurations();
builder.RegisterMassTransit();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FIAP Cloud Games",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "JWT Authentication",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            []
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Core"));
    options.UseLazyLoadingProxies();
}, ServiceLifetime.Scoped);

builder.Services.AddSingleton<UserProducer>();

builder.Services.AddScoped<IPasswordHasher, Sha256PasswordHasher>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserApplicationService, UserApplicationService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role
        };
    });
#endregion

var app = builder.Build();

#region MIGRATION COM RETRY
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    const int maxAttempts = 10;
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            logger.LogInformation("Tentando aplicar migrations (tentativa {Attempt}/{MaxAttempts})...", attempt, maxAttempts);
            dbContext.Database.Migrate();
            logger.LogInformation("Migrations aplicadas com sucesso.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao aplicar migrations na tentativa {Attempt}.", attempt);
            if (attempt == maxAttempts)
            {
                logger.LogError(ex, "Não foi possível aplicar migrations após {MaxAttempts} tentativas. Encerrando aplicação.", maxAttempts);
                throw;
            }
            var delay = TimeSpan.FromSeconds(Math.Min(30, 2 * attempt));
            logger.LogInformation("Aguardando {Delay} antes da próxima tentativa...", delay);
            await Task.Delay(delay);
        }
    }
}
#endregion

#region APP
using (var scope = app.Services.CreateScope())
{
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

await DatabaseSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ProblemDetailsExceptionMiddleware>();
app.UseAuthorization();
app.MapGet("/health", () => Results.Ok("Healthy")).ExcludeFromDescription();
app.MapControllers();

app.Run();
#endregion