
namespace MinimalAPIsMovies.Repositories
{
    public class LocalFileStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor) : IFileStorage
    {
        public Task Delete(string? route, string container)
        {
            if (string.IsNullOrEmpty(route))
            {
                return Task.CompletedTask;
            }
            var fileName = Path.GetFileName(route);
            var fileDirectory = Path.Combine(env.WebRootPath,container, fileName);
            if (!File.Exists(fileDirectory)) 
            {
                File.Delete(fileDirectory);
            }
            return Task.CompletedTask;
        }

        public async Task<string> Store(string container, IFormFile formFile)
        {
           var extension = Path.GetExtension(formFile.FileName);
           var filename = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, container);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string route = Path.Combine(folder, filename);
            using (var ms = new MemoryStream())
            {
                await formFile.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }
            var scheme = httpContextAccessor.HttpContext!.Request.Scheme;
            var host = httpContextAccessor.HttpContext!.Request.Host;
            var url = $"{scheme}://{host}";
            var urlfile = Path.Combine(url, container, filename).Replace("\\", "/");
            return urlfile;
        }
    }
}
