using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coink.Context;
using coink.Models;

namespace coink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MunicipalitiesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MunicipalitiesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Municipality>>> GetMunicipalities()
        {
            // Use LINQ to filter and project results directly in Entity Framework
            return await _context.Municipality.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Municipality>> GetMunicipality(int id)
        {
            var municipality = await _context.Municipality.FindAsync(id);

            if (municipality == null)
            {
                return NotFound();
            }

            return municipality;
        }

    }
}