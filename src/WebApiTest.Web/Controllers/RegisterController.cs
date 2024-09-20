using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiTest.Web.DataAccess;
using WebApiTest.Web.Entitys;
using WebApiTest.Web.UserDtos;

namespace WebApiTest.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegisterController : ControllerBase
{
    private readonly MyDbCOntext _dbContext;
    private readonly DbSet<User> _dbSet;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1000, 1000);


    public RegisterController(MyDbCOntext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<User>();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromForm] UserDto userDto)
    {
        await _semaphore.WaitAsync();

        try
        {
            if (userDto.Email == null)
            {
                return BadRequest(new { Message = "Email is required." });
            }

            var existingUser = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (existingUser != null)
            {
                return BadRequest(new { Message = $"The email {userDto.Email} is already in use." });
            }

            User user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Birthday = userDto.Birthday,
            };

            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
