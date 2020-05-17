using System;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Server.Interfaces;
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
                var currentConfig = new ConfigDto();//_ioTConfigService.GetConfig();
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
                var currentConfig = new ConfigDto();//_ioTConfigService.GetConfig();
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
                var currentConfigVersion = -1;//await _ioTConfigService.UpdateConfig(model);
               // if (currentConfigVersion <= 0) return NoContent();

                await _jobsService.StopJobs();

                //var loadedConfig = await _ioTConfigService.LoadConfig();
                //_ioTConfigService.InitConfig(null, loadedConfig);
                var currentModules = _ioTConfigService.GetModules();
                
                await _jobsService.StartJobs(currentModules);

                return new JsonResult(currentConfigVersion);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


    }
}