using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace HiliTechChallenge.Controllers
{
    [Route("/file")]
    public class FileController : HiliController
    {
        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery, Required] string url)
        {
            
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if(!provider.TryGetContentType(url, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return new FileStreamResult(System.IO.File.OpenRead(url), contentType);
        }
        
    }
}