using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace AspNetCoreWebApiContribProtobufSample.Controllers
{
    [Route("api/[controller]")]
    public class TablesController : Controller
    {
        // GET api/tables
        [HttpGet]
        public IActionResult Get()
        {
            List<Table> tables = new List<Table>
            {
                new Table{Name= "jim", Dimensions="190x80x90", Description="top of the range from Migro"},
                new Table{Name= "jim large", Dimensions="220x100x90", Description="top of the range from Migro"}
            };

            return Ok(tables);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var table = new Table { Name = "jim", Dimensions = "190x80x90", Description = "top of the range from Migro" };
            return Ok(table);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Table value)
        {
            var got = value;
            return Created("api/tables", got);
        }
    }
}
