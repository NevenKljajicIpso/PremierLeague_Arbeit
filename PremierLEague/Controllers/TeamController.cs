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
    public class TeamController : ControllerBase
    {
        private readonly IDriver _driver;

        public TeamController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (t:Team) RETURN t");

                var teams = await result.ToListAsync(r =>
                {
                    var node = r["t"].As<INode>();
                    if (!node.Properties.ContainsKey("teamID"))
                    {
                        return null;
                    }

                    return new Team
                    {
                        teamID = node.Properties["teamID"].As<int>(),
                        teamName = node.Properties["teamName"].As<string>()
                    };
                });

                return Ok(teams.Where(t => t != null));
            }
        }



        [HttpGet("{ID}")]
        public async Task<IActionResult> GetTeam(int ID)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (t:Team) WHERE t.teamID = $ID RETURN t", new { ID });
                var team = await result.ToListAsync(r => new Team
                {
                    teamID = r["t"].As<INode>().Properties["teamID"].As<int>(),
                    teamName = r["t"].As<INode>().Properties["teamName"].As<string>()
                });

                if (team == null)
                    return NotFound();

                return Ok(team);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTeam([FromBody] Team team)
        {
            using (var session = _driver.AsyncSession())
            {
                await session.RunAsync($"CREATE (t:Team {{teamID: {team.teamID}, teamName: '{team.teamName}'}})");
            }

            return CreatedAtAction(nameof(GetTeam), new { ID = team.teamID }, team);
        }

        [HttpPut("{ID}")]
        public async Task<IActionResult> UpdateTeam(int ID, [FromBody] Team team)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (t:Team) WHERE t.teamID = $ID SET t.teamName = $name RETURN t", new { ID, name = team.teamName });
                var updatedTeam = await result.ToListAsync(r => r["t"].As<INode>());

                if (updatedTeam == null)
                {
                    return NotFound();
                }

                return Ok(updatedTeam);
            }
        }

        [HttpDelete("{ID}")]
        public async Task<IActionResult> DeleteTeam(int ID)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (t:Team) WHERE t.teamID = $ID DELETE t", new { ID });

                return NoContent();
            }
        }
    }
}
