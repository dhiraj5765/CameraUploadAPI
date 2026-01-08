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

        // ============================
        // 📸 Upload Image API
        // ============================
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var uploadFolder = Path.Combine(_env.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var fileName = "photo" + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var aiResult = await AnalyzeImage(filePath);

            return Ok(new
            {
                folder = "Uploads",
                savedAs = fileName,
                aiDescription = aiResult
            });
        }

        // ============================
        // 🤖 HuggingFace AI (FREE + WORKING)
        // ============================
        private async Task<string> AnalyzeImage(string filePath)
        {
            string hfToken = Environment.GetEnvironmentVariable("HF_TOKEN");

            if (string.IsNullOrWhiteSpace(hfToken))
                return "AI Error: HuggingFace token not found";

            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", hfToken);

            // ✅ FREE-TIER SUPPORTED MODEL
            var url = "https://router.huggingface.co/hf-inference/models/nlpconnect/vit-gpt2-image-captioning";

            byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            using var content = new ByteArrayContent(imageBytes);
            content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");

            var response = await client.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"AI Error: {response.StatusCode} - {json}";

            dynamic data = JsonConvert.DeserializeObject(json);
            return data[0].generated_text.ToString();
        }
    }
}
