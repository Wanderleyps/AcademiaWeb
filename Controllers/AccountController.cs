using AcademiaWeb.Identity.Interfaces;
using AcademiaWeb.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticate _authentication;
        private readonly IConfiguration _configuration;

        public AccountController(IAuthenticate authentication, IConfiguration configuration)
        {
            _authentication = authentication ??
                throw new ArgumentNullException(nameof(authentication));
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("LoginUser")]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginModel userInfo)
        {
            var result = await _authentication.Authenticate(userInfo.Email, userInfo.Password);

            if (result)
            {
                return GenerateToken(userInfo);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
                return BadRequest(ModelState);
            }
        }

        [Authorize]
        [HttpPost("CreateUser")]
        [ApiExplorerSettings(IgnoreApi = true)]//faz com que esse endpoin não seja exposto
        public async Task<ActionResult> CreateUser([FromBody] LoginModel userInfo)
        {
            var result = await _authentication.RegisterUser(userInfo.Email, userInfo.Password);

            if (result)
            {
                //return GenerateToken(userInfo);
                return Ok($"User {userInfo.Email} was created successfully");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
                return BadRequest(ModelState);
            }
        }

        private UserToken GenerateToken(LoginModel userInfo)
        {
            //declarações do usuário
            var claims = new[]
            {
                new Claim("email", userInfo.Email),
                new Claim("meuvalor", "oque voce quiser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //gerar chave privada para assinar o token
            var privateKey = new SymmetricSecurityKey(
                //parâmetro lido a partir do arquivo appsettings.json
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));

            //gerar a assinatura digital a partir da chave privada
            var credentials = new SigningCredentials(privateKey,
                SecurityAlgorithms.HmacSha256);//utilizado hmac por ser simetrico

            //definir o tempo de expiração
            var expiration = DateTime.UtcNow.AddMinutes(10);

            //gerar o token
            JwtSecurityToken token = new JwtSecurityToken(
                //emissor
                issuer: _configuration["Jwt:Issuer"],
                //audiencia
                audience: _configuration["Jwt:Audience"],
                //claims
                claims: claims,
                //data de expiracao
                expires: expiration,
                //assinatura digital
                signingCredentials: credentials
                );

            return new UserToken()
            {
                //cria o token e retorna uma string no formato json compactado
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
