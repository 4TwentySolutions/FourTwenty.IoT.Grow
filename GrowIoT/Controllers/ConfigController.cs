using System;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using GrowIoT.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrowIoT.Controllers
{
    [Route("api/config")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IIoTConfigService _ioTConfigService;
        private readonly IJobsService _jobsService;
        public ConfigController(IIoTConfigService ioTConfigService, IJobsService jobsService)
        {
            _ioTConfigService = ioTConfigService;
            _jobsService = jobsService;
        }

        [HttpGet]
        [Route("getConfig")]
        public async Task<IActionResult> GetConfig()
        {
            try
            {
                var currentConfig = await _ioTConfigService.GetConfig();
                return new JsonResult(currentConfig);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("getVersion")]
        public async Task<IActionResult> GetVersion()
        {
            try
            {
                var currentConfig = await _ioTConfigService.GetConfig();
                if (currentConfig != null)
                {
                    return new JsonResult(currentConfig.CurrentVersion);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("updateConfig")]
        public async Task<IActionResult> UpdateConfig(ConfigDto model)
        {
            try
            {
                var currentConfigVersion = await _ioTConfigService.UpdateConfig(model);
                if (currentConfigVersion > 0)
                {
                    await _jobsService.StopJobs();

                    var currentConfig = await _ioTConfigService.GetConfig();
                    _ioTConfigService.InitConfig(null, currentConfig);
                    await _jobsService.StartJobs(currentConfig);

                    return new JsonResult(currentConfigVersion);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

    }
}