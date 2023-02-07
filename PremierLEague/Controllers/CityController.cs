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
    public class CityController : ControllerBase
    {
        private readonly IDriver _driver;

        public CityController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpGet]
        public async Task<IActionResult> GetCities()
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (c:City) RETURN c");

                    var cities = await result.ToListAsync(r =>
                    {
                        var node = r["c"].As<INode>();
                        if (!node.Properties.ContainsKey("cityID"))
                        {
                            return null;
                        }

                        return new City
                        {
                            cityID = node.Properties["cityID"].As<int>(),
                            cityName = node.Properties["cityName"].As<string>()
                        };
                    });

                return Ok(cities.Where(c => c != null));
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (c:City) WHERE c.cityID = $id RETURN c", new { id });
                var city = await result.ToListAsync(r => new City
                {
                    cityID = r["c"].As<INode>().Properties["cityID"].As<int>(),
                    cityName = r["c"].As<INode>().Properties["cityName"].As<string>()
                });

                if (city == null)
                    return NotFound();

                return Ok(city);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCity([FromBody] City city)
        {
            using (var session = _driver.AsyncSession())
            {
                await session.RunAsync($"CREATE (c:City {{cityId: {city.cityID}, cityName: '{city.cityName}'}})");
            }

            return CreatedAtAction(nameof(GetCity), new { id = city.cityID }, city);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] City city)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (c:City) WHERE c.cityID = $id SET c.cityName = $name RETURN c", new { id, name = city.cityName });
                var updatedCity = await result.ToListAsync(r => r["c"].As<City>());

                if (updatedCity == null)
                {
                    return NotFound();
                }

                return Ok(updatedCity);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            using (var session = _driver.AsyncSession())
            {
                var result = await session.RunAsync("MATCH (c:City) WHERE c.cityID = $id DELETE c", new { id });

                return NoContent();
            }
        }
    }
}
