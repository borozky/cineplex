using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cineplex.Repositories;
using Cineplex.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Cineplex.api
{
    [Route("api/[controller]")]
    public class ShowsController : Controller
    {
        private IShowsRepository repo;

        public ShowsController(IShowsRepository repository)
        {
            repo = repository;
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            var shows = repo.All();

            return Ok(new
            {
                status = "success",
                message = $"Found {shows.Count()} results",
                data = new { shows = shows }
            });
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var show = repo.Find(id);
            return Ok(new
            {
                status = "success",
                message = show != null ? "Found 1 result" : "Found 0 results",
                data = new { show = show }
            });
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Show show)
        {
            if (ModelState.IsValid)
            {
                var addedShow = repo.Create(show);
                if (addedShow != null)
                {
                    return Ok(
                        new {
                            status = "OK",
                            message = "Show has successfully added",
                            data = new {show =  addedShow}
                        }
                    );
                }
            }
            return NotFound();
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Show show)
        {
            if (ModelState.IsValid)
            {
                var updatedShow = repo.Update(show);
                if (updatedShow != null)
                {
                    return Ok(
                        new
                        {
                            status = "OK",
                            message = "Show has successfully updated",
                            data = new { show = updatedShow }
                        }    
                    );
                }
            }
            return NotFound();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id > 0)
            {
                var deleted = repo.Delete(id);
                if (deleted)
                {
                    return Ok(new
                    {
                        status = "OK",
                        message = "Show has successfully deleted"
                    });
                }
            }
            return NotFound(new { status = "fail", message = "Show failed to be deleted" });
        }
    }
}
