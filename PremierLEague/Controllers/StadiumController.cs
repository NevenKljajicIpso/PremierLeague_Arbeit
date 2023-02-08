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
    public class StadiumController : ControllerBase
    {
        private readonly IDriver _driver;

        public StadiumController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpGet]
        public async Task<IActionResult> GetStadiums()
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (s:Stadium) RETURN s");

                var stadiums = await result.ToListAsync(r =>
                {
                    var node = r["s"].As<INode>();
                    if (!node.Properties.ContainsKey("stadiumID"))
                    {
                        return null;
                    }

                    return new Stadium
                    {
                        stadiumID = node.Properties["stadiumID"].As<int>(),
                        stadiumName = node.Properties["stadiumName"].As<string>()
                    };
                });

                return Ok(stadiums.Where(s => s != null));
            }
        }



        [HttpGet("{ID}")]
        public async Task<IActionResult> GetStadium(int ID)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (s:Stadium) WHERE s.stadiumID = $ID RETURN s", new { ID });
                var stadium = await result.ToListAsync(r => new Stadium
                {
                    stadiumID = r["s"].As<INode>().Properties["stadiumID"].As<int>(),
                    stadiumName = r["s"].As<INode>().Properties["stadiumName"].As<string>()
                });

                if (stadium == null)
                    return NotFound();

                return Ok(stadium);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateStadium([FromBody] Stadium stadium)
        {
            using (var session = _driver.AsyncSession())
            {
                await session.RunAsync($"CREATE (s:Stadium {{stadiumID: {stadium.stadiumID}, stadiumName: '{stadium.stadiumName}'}})");
            }

            return CreatedAtAction(nameof(GetStadium), new { ID = stadium.stadiumID }, stadium);
        }

        [HttpPut("{ID}")]
        public async Task<IActionResult> UpdateStadium(int ID, [FromBody] Stadium stadium)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (s:Stadium) WHERE s.stadiumID = $ID SET s.stadiumName = $name RETURN s", new { ID, name = stadium.stadiumName });
                var updatedStadium = await result.ToListAsync(r => r["s"].As<INode>());

                if (updatedStadium == null)
                {
                    return NotFound();
                }

                return Ok(updatedStadium);
            }
        }

        [HttpDelete("{ID}")]
        public async Task<IActionResult> DeleteStadium(int ID)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (s:Stadium) WHERE s.stadiumID = $ID DELETE s", new { ID });

                return NoContent();
            }
        }
    }
}
