using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityService.Data;
using IdentityService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BodyShapeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BodyShapeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BodyShape>>> GetAllBodyShapes()
        {
            return await _context.BodyShapes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BodyShape>> GetBodyShape(string id)
        {
            var bodyShape = await _context.BodyShapes.FindAsync(id);

            if (bodyShape == null)
            {
                return NotFound();
            }

            return bodyShape;
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<BodyShape>>> GetBodyShapesByCategory(string category)
        {
            return await _context.BodyShapes
                .Where(bs => bs.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BodyShape>> CreateBodyShape(BodyShape bodyShape)
        {
            _context.BodyShapes.Add(bodyShape);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBodyShape), new { id = bodyShape.Id }, bodyShape);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBodyShape(string id, BodyShape bodyShape)
        {
            if (id != bodyShape.Id)
            {
                return BadRequest();
            }

            _context.Entry(bodyShape).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BodyShapeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBodyShape(string id)
        {
            var bodyShape = await _context.BodyShapes.FindAsync(id);
            if (bodyShape == null)
            {
                return NotFound();
            }

            _context.BodyShapes.Remove(bodyShape);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BodyShapeExists(string id)
        {
            return _context.BodyShapes.Any(e => e.Id == id);
        }
    }
} 