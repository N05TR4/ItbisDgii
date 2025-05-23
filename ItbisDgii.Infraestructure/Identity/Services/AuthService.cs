using ItbisDgii.Application.DTOs.Users;
using ItbisDgii.Application.Interfaces;
using ItbisDgii.Application.Wrappers;
using ItbisDgii.Domain.Enums;
using ItbisDgii.Domain.Settings;
using ItbisDgii.Infraestructure.Identity.Helpers;
using ItbisDgii.Infraestructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ItbisDgii.Infraestructure.Identity.Services
{
    public class AuthService : IAuthServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JWTSettings _jwtSettings;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JWTSettings> jwtSettings, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<Response<AuthResponseDto>> Authenticate(AuthRequestDto request, string ipAddress)
        {
            var usuario = await _userManager.FindByEmailAsync(request.Email);
            if (usuario == null)
            {
                throw new ApplicationException($"No existe una cuenta registrada con el email {request.Email}");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);

            if (!result.Succeeded)
            {
                throw new ApplicationException("Credenciales incorrectas");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJWTToken(usuario);
            AuthResponseDto response = new AuthResponseDto();
            response.Id = usuario.Id;
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = usuario.Email;
            response.UserName = usuario.UserName;

            var rolesList = await _userManager.GetRolesAsync(usuario).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = usuario.EmailConfirmed; // Corregido el nombre de la propiedad

            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;

            return new Response<AuthResponseDto>(response, $"Usuario Autenticado {usuario.UserName}");
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var usuarioConElMismoUserName = await _userManager.FindByNameAsync(request.UserName);

            if (usuarioConElMismoUserName != null)
            {
                throw new ApplicationException($"El nombre de usuario {request.UserName} ya existe");
            }

            var usuarioConElMismoCorreo = await _userManager.FindByEmailAsync(request.Email);
            if (usuarioConElMismoCorreo != null)
            {
                throw new ApplicationException($"El Email {request.Email} ya existe");
            }

            var usuario = new ApplicationUser
            {
                Email = request.Email,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                UserName = request.UserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };

            var result = await _userManager.CreateAsync(usuario, request.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(usuario, Roles.Basic.ToString());
                return new Response<string>(usuario.Id, message: $"Usuario registrado exitosamente. {request.UserName}");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException($"Error al crear usuario: {errors}");
            }
        }

        private async Task<JwtSecurityToken> GenerateJWTToken(ApplicationUser usuario)
        {
            var userClaims = await _userManager.GetClaimsAsync(usuario);
            var roles = await _userManager.GetRolesAsync(usuario);

            var rolesClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                rolesClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddress = IpHelpers.GetIpAdress();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("uid", usuario.Id),
                new Claim("ip", ipAddress),
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
                );

            return JwtSecurityToken;
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
                CreatedByIp = ipAddress,
            };
        }

        private string RandomTokenString()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(40);
            return Convert.ToHexString(randomBytes);
        }
    }
}