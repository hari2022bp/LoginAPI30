using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Login.Model;
using System.Collections;

namespace Login.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginDbContext _dbContext;
        private readonly ILogger<LoginController> _logger;
        public LoginController(LoginDbContext dbContext, ILogger<LoginController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
       // [Route("api/Login/PostUser")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.UserName == user.UserName && u.Password==user.Password);

                if (existingUser != null)
                {
                    return Conflict("User already exists.");
                }

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.UserId }, user);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (ex) here
                return StatusCode(500, "An error occurred while saving the user.");
            }
        }
    }
}
