using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

// Ensure Uploads folder exists
var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "Uploads");
Directory.CreateDirectory(uploadPath);

app.UseHttpsRedirection();

// Default static files
app.UseStaticFiles();

// Serve uploads folder
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadPath),
    RequestPath = "/uploads"
});

app.UseAuthorization();

app.MapControllers();

app.Run();
