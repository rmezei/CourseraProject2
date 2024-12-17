using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class UserController : ControllerBase
{
    private static List<User> users = new List<User>();

    public UserController()
    {
        if(users.Count == 0)
        {
            SeedData();
        }
    }

    public static void SeedData()
    {
        users.Add(new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "John.Doe@email.com"
        });

        users.Add(new User
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "Jane.Doe@email.com"
        });
    }

    // GET: api/user
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        try{
            var userList = users.AsReadOnly();
            return Ok(userList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // GET: api/user/{id}
    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        try{
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // POST: api/user
    [HttpPost]
    public ActionResult<User> AddUser(User user)
    {
        try{
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // PUT: api/user/{id}
    [HttpPut("{id}")]
    public ActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        try{
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.DateOfBirth = updatedUser.DateOfBirth;

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    // DELETE: api/user/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        try{
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            users.Remove(user);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}