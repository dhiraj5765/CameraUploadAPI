using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SmartCameraPro.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public PhotosController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            // 📁 Create Upload Folder
            var uploadFolder = Path.Combine(_env.ContentRootPath, "Uploads");

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // 📸 Save Image
            var fileName = "photo" + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 🤖 AI Processing (HuggingFace)
            var aiResult = await AnalyzeImage(filePath);

            // 🔙 Send Response to client
            return Ok(new
            {
                folder = "Uploads",
                savedAs = fileName,
                aiDescription = aiResult
            });
        }

        // ============================
        // 🤖 AI via HuggingFace Router
        // ============================
        private async Task<string> AnalyzeImage(string filePath)
        {
            string hfToken ="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"; // <-- replace locally

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", hfToken);

            // ✅ NEW REQUIRED URL
            var url = "https://router.huggingface.co/models/Salesforce/blip-image-captioning-large";

            byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            using var content = new ByteArrayContent(imageBytes);
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            var response = await client.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"AI Error: {json}";

            dynamic data = JsonConvert.DeserializeObject(json);

            return data[0].generated_text.ToString();
        }
    }
}
