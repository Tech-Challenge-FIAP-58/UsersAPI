using FCG.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using FCG.Core.Interfaces.Repository;
using FCG.Core.Models;

namespace FCG.Infra.Repository
{
    public class AuthRepository(ApplicationDbContext context) : EFRepository<User>(context), IAuthRepository
    {
        public async Task<int> Create(User entity)
        {
            await Register(entity);

            if (entity.IsAdmin)
            { 
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (role != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = entity.Id,
                        RoleId = role.Id
                    };

                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }
            }

            return entity.Id;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AsNoTracking().AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByCpfAsync(string cpf)
        {
            return await _dbSet.AsNoTracking().AnyAsync(u => u.Cpf == cpf);
        }

        public string GenerateToken(IConfiguration configuration, User user) => GenerateTokenPrivate(configuration, user);

        private static string GenerateTokenPrivate(IConfiguration configuration, User user)
        {
            var key = configuration["Jwt:Key"];
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            var expirationStr = configuration["Jwt:Expiration"];

            if (string.IsNullOrWhiteSpace(key))
                throw new ValidationException("JWT Key is not configured (Jwt:Key).");
            if (!int.TryParse(expirationStr, out var expirationMinutes))
                throw new ValidationException("JWT Expiration is not configured or invalid (Jwt:Expiration).");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Name),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roleNames = user.UserRoles?.Select(ur => ur.Role.Name).Distinct().ToList() ?? new List<string>();

            if (user.IsAdmin && !roleNames.Contains("Admin", StringComparer.OrdinalIgnoreCase))
                roleNames.Add("Admin");

            foreach (var role in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience
            };

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(tokenDescriptor);
        }
    }
}