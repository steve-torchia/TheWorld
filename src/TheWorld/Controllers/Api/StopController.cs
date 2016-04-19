using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Interfaces;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;


namespace TheWorld.Controllers.Api
{
    [Authorize]
    [Route("api/trips/{tripName}/stops")]
    public class StopController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<StopController> _logger;
        private CoordService _coordService;

        public StopController(IWorldRepository respository, ILogger<StopController> logger, CoordService coordService)
        {
            _repository = respository;
            _logger = logger;
            _coordService = coordService;
        }

        [HttpGet("")]
        public JsonResult Get(string tripName)
        {
            try
            {
                var results = _repository.GetTripByName(tripName, User.Identity.Name);

                if (results == null)
                {
                    return Json(null);
                }

                return Json(Mapper.Map<IEnumerable<StopViewModel>>(results.Stops.OrderBy(s=>s.Order)));
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get stos for trip {tripName}", e);
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(new { ExceptionMsg = e.Message });
            }
        }

        [HttpPost("")]
        public async Task<JsonResult> Post(string tripName, [FromBody] StopViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newStop = Mapper.Map<Stop>(vm); 
                    
                    // look up Geo Coordinates
                    var result = await _coordService.Lookup(newStop.Name);

                    if (!result.Success)
                    {
                        Response.StatusCode = (int) HttpStatusCode.BadRequest;
                        return Json(result.Message);
                    }

                    newStop.Longitude = result.Longitude;
                    newStop.Latitude = result.Latitude;


                    // Save to DB 
                    _logger.LogInformation("Attempting to Save New Stop to DB");
                    _repository.AddStop(tripName, newStop, User.Identity.Name);

                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int) HttpStatusCode.Created;
                        return Json(Mapper.Map<StopViewModel>(newStop));
                    }

                }

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = "failed", ModelState = ModelState });
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to Save new Stop", e);

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { ExceptionMsg = e.Message });
            }
        }
    }
}
