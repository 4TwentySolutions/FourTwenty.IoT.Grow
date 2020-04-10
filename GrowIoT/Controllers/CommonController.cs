using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace GrowIoT.Controllers
{
    public class CommonController : Controller
    {
        public async Task<IActionResult> DownloadLogFile(string name)
        {
            using var fs = System.IO.File.Open(Path.Combine(Constants.Constants.LogsDirectory, name), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var mem = new MemoryStream();
            await fs.CopyToAsync(mem);
            var result = new FileContentResult(mem.ToArray(), "text/plain") { FileDownloadName = name };
            return result;
        }
    }
}