using System;
using System.Collections.Generic;
using System.Linq;
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
        public ConfigController(IIoTConfigService ioTConfigService)
        {
            _ioTConfigService = ioTConfigService;
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