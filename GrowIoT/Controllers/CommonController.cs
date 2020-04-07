using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace GrowIoT.Controllers
{
    public class CommonController : Controller
    {
        public IActionResult DownloadLogFile(string name)
        {
            var fs = System.IO.File.Open(Path.Combine(Constants.Constants.LogsDirectory, name), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var result = new FileStreamResult(fs, "text/plain") {FileDownloadName = name};
            return result;
        }
    }
}