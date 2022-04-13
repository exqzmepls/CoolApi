using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolApi.Database;
using CoolApi.Database.Models;
using CoolApiModels.Users;
using System.ComponentModel.DataAnnotations;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CoolContext _context;

        public UsersController(CoolContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<GetUsersModel> GetUsers(
            [FromQuery][Required][Range(0, int.MaxValue)] int offset,
            [FromQuery][Required][Range(1, 50)] int portion,
            [FromQuery][StringLength(32)] string searchString)
        {
            return new GetUsersModel
            {
                Offset = offset,
                Portion = portion,
                TotalCount = 999,
                Content = new List<GetUserModel>
                {
                    new GetUserModel{Id = Guid.Empty, Login = "user1"},
                    new GetUserModel{Id = Guid.Empty, Login = "user2"}
                }
            };

            //return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public ActionResult<GetUserModel> GetUser([Required] Guid id)
        {
            /*var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;*/

            return new GetUserModel { Id = id, Login = "user" };
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ActionResult<GetUserModel> PutUser(Guid id, [FromBody][Required] PutUserModel user)
        {
            /*if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }*/

            return new GetUserModel { Id = Guid.Empty, Login = "newLogin" };
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<GetUserModel> PostUser(PostUserModel user)
        {
            /*_context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);*/

            return new GetUserModel { Id = Guid.Empty, Login = user.Login };
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            /*var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();*/

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
