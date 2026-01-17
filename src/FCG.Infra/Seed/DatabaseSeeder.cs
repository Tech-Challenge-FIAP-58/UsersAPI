using FCG.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FCG.Core.Interfaces.Utils;
using FCG.Core.Models;

namespace FCG.Infra.Seed
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var logger = scope.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("DatabaseSeeder");

            logger.LogInformation("Iniciando processo de seed do banco de dados.");

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            if (await context.Users.AnyAsync(u => u.IsAdmin))
            {
                logger.LogInformation("Usuário administrador já existe. Seed ignorado.");
                return;
            }

            logger.LogInformation("Nenhum administrador encontrado. Iniciando bootstrap.");

            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole == null)
            {
                logger.LogInformation("Role 'Admin' não encontrada. Criando role padrão.");

                adminRole = new Role
                {
                    Name = "Admin",
                    Description = "Administrador do sistema",
                };

                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();

                logger.LogInformation("Role 'Admin' criada com sucesso.");
            }
            else
            {
                logger.LogInformation("Role 'Admin' já existente.");
            }

            var adminCpf = configuration["BootstrapAdmin:Cpf"];
            var adminEmail = configuration["BootstrapAdmin:Email"];
            var adminPassword = configuration["BootstrapAdmin:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail)
                || string.IsNullOrWhiteSpace(adminPassword)
                || string.IsNullOrWhiteSpace(adminCpf))
            {
                logger.LogError("Configuração BootstrapAdmin incompleta. Seed abortado.");
                throw new InvalidOperationException(
                    "BootstrapAdmin não configurado. Defina Cpf, Email e Password.");
            }

            logger.LogInformation("Configuração BootstrapAdmin carregada com sucesso.");

            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var adminUser = new User
            {
                Name = "Administrador",
                Email = adminEmail,
                Cpf = adminCpf,
                Address = "Localhost",
                Password = passwordHasher.Hash(adminPassword),
                IsAdmin = true,
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            logger.LogInformation("Usuário administrador criado com sucesso (Id: {UserId}).", adminUser.Id);

            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });

            await context.SaveChangesAsync();

            logger.LogInformation("Role 'Admin' associada ao usuário administrador.");
            logger.LogInformation("Seed do banco de dados finalizado com sucesso.");
        }
    }
}
