using System.IO;
using System.Threading.Tasks;
using AzStorage.CoreAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AzStorage.MVC.Controllers
{
    public class DemoController : Controller
    {
        private readonly IConfiguration _configuration;
        public DemoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var files = await AzBlob.GetAllFilesFromBlobContainer();
            return View(files);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile files)
        {
            string fileName = files.FileName;
            var data = files.OpenReadStream();

            await AzBlob.UploadFileToStorage(data, fileName);

            return await Create();
        }

        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var resultDownloaded = await AzBlob.DownloadFile(fileName);
            return File(resultDownloaded.FileStream, resultDownloaded.ContentType, fileName);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string fileName)
        {
            await AzBlob.DeleteFile(fileName);
            return RedirectToAction("Create", "Demo");
        }
    }
}
