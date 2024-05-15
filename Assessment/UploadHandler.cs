using Assessment.Data;
using Assessment.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Assessment
{
    public class UploadHandler
    {
        private readonly AssessmentContext _context;

        public UploadHandler(AssessmentContext context)
        {
            _context = context;
        }

        public async Task<string> Upload(IFormFile file)
        {
            List<string> validExtensions = new List<string>() { ".mp4", ".avi", ".mkv", ".jpg", ".png" };
            string extension = Path.GetExtension(file.FileName);
            if (!validExtensions.Contains(extension))
            {
                return $"Extension is not valid({string.Join(',', validExtensions)})";
            }
            long size = file.Length;
            if (size > (5 * 1024 * 1024))
            {
                return "Maximum size can be 5mb";
            }
            string fileName = Guid.NewGuid().ToString() + extension;
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            using (FileStream stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save information about the uploaded file to the database
            var video = new Video
            {
                // Assuming VideoId is auto-incremented in the database
                FileName = fileName,
                FilePath = Path.Combine(uploadPath, fileName),
                SizeInBytes = (int)file.Length,
                UploadedAt = DateTime.Now
            };

            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            return fileName;
        }
    }
}
