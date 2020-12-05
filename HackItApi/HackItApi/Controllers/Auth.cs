using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using HackItApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BC = BCrypt.Net.BCrypt;

namespace HackItApi.Controllers
{
    [ApiController, Route("api/auth")]
    public class Auth : Controller
    {
        private readonly HackContext _context;
        private readonly IConfiguration _config;

        public Auth(HackContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Index([FromBody]LoginModel model)
        {
            #region InputChecks

            var user = _context.Users.FirstOrDefault(c => c.Email == model.Email);
            if (user == null || !BC.Verify(model.Password, user.Password))
            {
                return BadRequest(new
                {
                    message = "Email or password is incorrect"
                });
            }

            #endregion
            
            #region Token

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("HackTheAPI2020!@Test"));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience:"https://localhost:5001",
                claims: new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, Role.User)
                },
                expires: DateTime.Now.AddDays(7),
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            #endregion

            return Ok(new
            {
                token = tokenString,
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Balance
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (_context.Users.Any(c => c.Email == model.Email))
            {
                return BadRequest(new
                {
                    message = "Account with this email already exists!"
                });
            }
            
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = BC.HashPassword(model.Password),
                Balance = 0,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok();
        }
    }
}