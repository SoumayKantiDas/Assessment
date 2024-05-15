using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Assessment.Data;
using Assessment.Models;

namespace Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Videos1Controller : ControllerBase
    {
        private readonly AssessmentContext _context;

        public Videos1Controller(AssessmentContext context)
        {
            _context = context;
        }

        // GET: api/Videos1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Video>>> GetVideos()
        {
            return await _context.Videos.ToListAsync();
        }

        // GET: api/Videos1/5
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

        // PUT: api/Videos1/5
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

        // POST: api/Videos1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)       {

           
            return Ok(new UploadHandler().Upload(file));
        }
        public class UploadHandler
        {
            public string Upload(IFormFile file)
            {
                List<string> validExtensions = new List<string>() { ".mp4", ".avi", ".mkv", ".jpg", "png" };
                string extention = Path.GetExtension(file.FileName);
                if (!validExtensions.Contains(extention))
                {
                    return $"Extension is not valid({string.Join(',', validExtensions)})";
                }
                long size = file.Length;
                if (size > (5 * 1024 * 1024))
                {
                    return "Maximum size can be 5mb";
                }
                string fileName = Guid.NewGuid().ToString() + extention;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Uplaods");
                using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
                file.CopyTo(stream);
                return fileName;
            }
        }
        // DELETE: api/Videos1/5
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

            return NoContent();
        }

        private bool VideoExists(int id)
        {
            return _context.Videos.Any(e => e.VideoId == id);
        }
    }
}
