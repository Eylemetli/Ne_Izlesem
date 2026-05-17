using Microsoft.AspNetCore.Mvc;
using MovieRecommendation.API.Data;
using MovieRecommendation.API.DTOs;
using MovieRecommendation.API.Models;
using MovieRecommendation.API.Services;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var userExists = _context.Users.Any(x => x.Email == dto.Email);

            if (userExists)
            {
                return BadRequest("Bu email zaten kayıtlı.");
            }

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("Kayıt başarılı.");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

            if (user == null)
            {
                return BadRequest("Kullanıcı bulunamadı.");
            }

            bool isPasswordCorrect =
                BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordCorrect)
            {
                return BadRequest("Şifre yanlış.");
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Giriş başarılı.",
                token = token,
                userId = user.Id,
                email = user.Email
            });
        }
    }
}