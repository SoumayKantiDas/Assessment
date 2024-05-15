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
using NuGet.Packaging.Signing;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;





namespace Assessment.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly AssessmentContext _context;
        private readonly ICacheService _cacheService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public VideosController(AssessmentContext context, ICacheService cacheService, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _cacheService = cacheService;
            _hostingEnvironment = hostingEnvironment;
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

       

        private bool IsValidExtension(string fileName)
        {
            List<string> validExtensions = new List<string>() { ".mp4", ".avi", ".mkv",".jpg","png" }; // Add more valid extensions if needed
            string extension = Path.GetExtension(fileName);
            return validExtensions.Contains(extension.ToLower());
        }

        // POST: api/Videos
        [HttpPost]
        public async Task<ActionResult<Video>> PostVideo([FromForm] VideoUploadModel model)
        {
            if (model.VideoFile == null || model.VideoFile.Length == 0)
            {
                return BadRequest("Please provide a valid video file.");
            }

            if (!IsValidExtension(model.VideoFile.FileName))
            {
                return BadRequest("Extension is not valid. Valid extensions are: .mp4, .avi, .mkv");
            }

            if (model.VideoFile.Length > (5 * 1024 * 1024))
            {
                return BadRequest("Maximum size can be 5mb.");
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.VideoFile.FileName);

            string uploadFolder = Path.Combine(_hostingEnvironment.WebRootPath, "Uplaods"); // Assuming Uploads is the folder name
            string filePath2 = Path.Combine(uploadFolder, fileName);

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

    
            using (var stream = new FileStream(filePath2, FileMode.Create))
            {
                await model.VideoFile.CopyToAsync(stream);
            }

            var video = new Video
            {
                // Assuming ProductId is auto-incremented in the database
                Title = model.Title,
                FileName = fileName,
                FilePath = filePath2,
                SizeInBytes = (int)model.VideoFile.Length,
                UploadedAt = DateTime.Now
            };

            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            await _cacheService.ClearCachedVideosAsync(); // Clear cached videos after creation

            return CreatedAtAction("GetVideo", new { id = video.VideoId }, video);
        }


        // Other methods remain unchanged

        public class VideoUploadModel
        {
           
            public string Title { get; set; }
            public IFormFile VideoFile { get; set; }
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
