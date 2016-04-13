using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using TheWorld.Interfaces;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("api/trips")] //root route
    public class TripController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<TripController> _logger;

        public TripController(IWorldRepository repository, ILogger<TripController> logger )
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")] //extend routes if necessary
        public JsonResult Get()
        {
            var results = Mapper.Map<IEnumerable<TripViewModel>>(_repository.GetAllTripsWithStops());
            return Json(results);
        }

        [HttpPost("")]
        public JsonResult Post([FromBody]TripViewModel tvm)  //[FromBody] binds data from body of request...else default assumes Query String  TripViewModel lets us validate/shape data
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newTrip = Mapper.Map<Trip>(tvm);

                    // Save to DB
                    _logger.LogInformation("Attempting to Save New Trip to DB");
                    _repository.AddTrip(newTrip);

                    if (_repository.SaveAll())
                    {
                        Response.StatusCode = (int) HttpStatusCode.Created;
                        return Json(Mapper.Map<TripViewModel>(newTrip));
                    }
                   
                }

                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(new {Message = "failed", ModelState = ModelState});
            }
            catch (Exception e)
            {
                _logger.LogError("Failed to Save new trip", e);

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new {ExceptionMsg = e.Message});
            }
        }
    }
}
