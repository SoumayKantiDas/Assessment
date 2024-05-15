using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assessment.Data;
using Assessment.Models;
using Assessment.Services;


namespace Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly AssessmentContext _context;
        private readonly ICacheService _cacheService;

        public VideosController(AssessmentContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        // GET: api/Videos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideos()
        {
            var cachedVideos = await _cacheService.GetCachedVideosAsync();
            if (cachedVideos != null)
            {
                return Ok(cachedVideos);
            }

            var videos = await _context.Videos.ToListAsync();
            await _cacheService.CacheVideosAsync(videos);
            return Ok(videos);
        }

        // GET: api/Videos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Video>> GetVideo(int id)
        {
            var video = await _context.Videos.FindAsync(id);

            if (video == null)
            {
                return NotFound();
            }

            return video;
        }

        // PUT: api/Videos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, Video video)
        {
            if (id != video.VideoId)
            {
                return BadRequest();
            }

            _context.Entry(video).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _cacheService.ClearCachedVideosAsync(); // Clear cached videos after update
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VideoExists(id))
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

        // POST: api/Videos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo(Video video)
        {
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            await _cacheService.ClearCachedVideosAsync(); // Clear cached videos after creation

            return CreatedAtAction("GetVideo", new { id = video.VideoId }, video);
        }

        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
            await _cacheService.ClearCachedVideosAsync(); // Clear cached videos after deletion

            return NoContent();
        }

        private bool VideoExists(int id)
        {
            return _context.Videos.Any(e => e.VideoId == id);
        }
    }
}
