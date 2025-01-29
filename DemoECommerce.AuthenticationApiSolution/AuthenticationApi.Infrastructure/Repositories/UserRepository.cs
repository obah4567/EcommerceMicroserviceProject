using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interface;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration configuration) : IUser
    {
        private async Task<AppUser> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user is null ? null! : user!;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                return null!;
            }
            return new GetUserDTO(userId,
                user.Name!, user.TelephoneNumber!,
                user.Email!, user.Address!, user.Role!);
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);
            if (getUser is null)
            {
                return new Response(false, "You cannot use this email for registration");
            }
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);
            if (!verifyPassword)
            {
                return new Response(false, "Invalid credentidals");
            }
            string token = GenerateToken(getUser);
            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(configuration.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name!),
                new(ClaimTypes.Email, user.Email!)
            };
            if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
            {
                claims.Add(new(ClaimTypes.Role, user.Role!));
            }
            var token = new JwtSecurityToken(
                issuer: configuration["Authentication:Issuer"],
                audience: configuration["Authentication:Audience"],
                claims: claims,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDTO userDTO)
        {
            var getUser = await GetUserByEmail(userDTO.Email);
            if (getUser is not null)
            {
                return new Response(false, "You cannot use this email for registration");
            }

            var result = context.Users.Add(new AppUser()
            {
                Name = userDTO.Name,
                Email = userDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                TelephoneNumber = userDTO.TelephoneNumber,
                Address = userDTO.Address,
                Role = userDTO.Role
            });

            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, $"User registered successfully") :
                new Response(false, $"Invalid data provided");
        }
    }
}
