using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cineplex.Repositories;
using Cineplex.Models;


namespace Cineplex.api
{
    // USE JSEND FORMAT FOR JSON RESPONSES
    // see https://labs.omniti.com/labs/jsend

    [Route("api/[controller]")]
    public class CinemasController : Controller
    {
        private ICinemasRepository repo;
        public CinemasController(ICinemasRepository repository)
        {
            repo = repository;
        }

        // GET: api/Cinemas
        [HttpGet]
        public IActionResult Get()
        {
            var cinemas = repo.GetAll();
            return Ok(new {
                status = "success",
                message = $"Found {cinemas.Count()}",
                data = new { cinemas = cinemas }
            });
        }

        // GET api/Cinemas/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var foundCinema = repo.Find(id);
            if (foundCinema != null)
            {
                return Ok(
                    new {
                        status = "success",
                        message = $"Cinema with ID of {id} has been found",
                        data = new { cinema = foundCinema }
                    }
                );
            }

            return NotFound(new { status = "Fail", data = new { id = $"Cinema with ID of {id} cannot be found" } });
        }

        // POST api/Cinemas/
        // WARNING: This functionality must be used with CAUTION because our application doesn't allow cinemas to be removed
        [HttpPost]
        public IActionResult Post([FromBody] Cinema cinema)
        {
            if (cinema == null)
            {
                return BadRequest(new { status = "error", message = "You must supply a cinema information" });
            }

            if (ModelState.IsValid)
            {
                var createdCinema = repo.Insert(cinema);
                if (createdCinema != null)
                {
                    return Ok(new { status = "success", message = "A new cinema has been created", data = new { cinema = createdCinema } });
                } else
                {
                    return BadRequest(new { status = "fail", message = "Failed to create cinema" });
                }
            }
            return BadRequest(new {
                status = "fail",
                message = "Failed to create cinema due to errors",
                data = ModelState.Values.Select(v => v.Errors)
            });
        }

        // PUT api/Cinema/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Cinema cinema)
        {
            if (cinema == null)
            {
                return NotFound(new { status = "error", message = "Cinema information cannot be empty" });
            }
            if (cinema.Id != id)
            {
                return NotFound(new { status = "error", message = "Cinema ID and the request ID must match" });
            }

            if (ModelState.IsValid)
            {
                var updatedCinema = repo.Update(cinema);
                if (updatedCinema != null)
                {
                    return Ok( new
                        {
                            status = "success",
                            message = "Cinema has successfully updated",
                            data = new {cinema = updatedCinema}
                        }
                    );
                }
            }
            return NotFound(new
            {
                status = "fail",
                message = "Failed to update cinema",
                data = ModelState.Values.Select(v => v.Errors)
            });
        }

        // DELETE api/Cinema/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // can you delete a CINEMA ???
            return BadRequest(new { status = "fail", message = "Cinemas are not allowed to be deleted" }); 
        }
    }
}
