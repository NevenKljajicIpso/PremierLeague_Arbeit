using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using PremierLeague.Model;
using System.Linq;
using System;

namespace PremierLeague.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IDriver _driver;

        public UserController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (u:User) RETURN u");

                var users = await result.ToListAsync(r =>
                {
                    var node = r["u"].As<INode>();
                    if (!node.Properties.ContainsKey("userID"))
                    {
                        return null;
                    }

                    return new User
                    {
                        userID = node.Properties["userID"].As<int>(),
                        userName = node.Properties["userName"].As<string>()
                    };
                });

                return Ok(users.Where(u => u != null));
            }
        }



        [HttpGet("{ID}")]
        public async Task<IActionResult> GetUser(int ID)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (u:User) WHERE u.userID = $ID RETURN u", new { ID });
                var user = await result.ToListAsync(r => new User
                {
                    userID = r["u"].As<INode>().Properties["userID"].As<int>(),
                    userName = r["u"].As<INode>().Properties["userName"].As<string>()
                });

                if (user == null)
                    return NotFound();

                return Ok(user);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            using (var session = _driver.AsyncSession())
            {
                await session.RunAsync($"CREATE (u:User {{userID: {user.userID}, userName: '{user.userName}'}})");
            }

            return CreatedAtAction(nameof(GetUser), new { ID = user.userID }, user);
        }

        [HttpPut("{ID}")]
        public async Task<IActionResult> UpdateUser(int ID, [FromBody] User user)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (u:User) WHERE u.userID = $ID SET u.userName = $name RETURN u", new { ID, name = user.userName });
                var updatedUser = await result.ToListAsync(r => r["u"].As<INode>());

                if (updatedUser == null)
                {
                    return NotFound();
                }

                return Ok(updatedUser);
            }
        }

        [HttpDelete("{ID}")]
        public async Task<IActionResult> DeleteUser(int ID)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (u:User) WHERE u.userID = $ID DELETE u", new { ID });

                return NoContent();
            }
        }
    }
}
