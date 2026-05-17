using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRecommendation.API.Data;
using MovieRecommendation.API.DTOs;
using MovieRecommendation.API.Models;
using System.Security.Claims;

namespace MovieRecommendation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public IActionResult GetMyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var profile = _context.UserProfiles.FirstOrDefault(x => x.UserId == userId);

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName,
                FavoriteGenres = profile?.FavoriteGenres,
                LanguagePreference = profile?.LanguagePreference,
                LocalOrForeign = profile?.LocalOrForeign,
                WatchingPurpose = profile?.WatchingPurpose
            });
        }

        [HttpPut("me")]
        public IActionResult UpdateMyProfile(ProfileUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim.Value);

            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            user.FullName = dto.FullName;

            var profile = _context.UserProfiles.FirstOrDefault(x => x.UserId == userId);

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId
                };

                _context.UserProfiles.Add(profile);
            }

            profile.FavoriteGenres = dto.FavoriteGenres;
            profile.LanguagePreference = dto.LanguagePreference;
            profile.LocalOrForeign = dto.LocalOrForeign;
            profile.WatchingPurpose = dto.WatchingPurpose;

            _context.SaveChanges();

            return Ok("Profil güncellendi.");
        }
    }
}