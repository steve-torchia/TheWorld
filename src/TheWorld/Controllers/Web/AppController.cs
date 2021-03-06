using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using TheWorld.Interfaces;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        private IMailService _mailService;
        private IWorldRepository _worldRepository;

        public AppController(IMailService service, IWorldRepository repository)
        {
            // Asp.net 5 out of the box DI is via constructor injection.  
            _mailService = service;

            _worldRepository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        public IActionResult Trips()
        {
            //var trips = _worldRepository.GetAllTrips();
            //return View(trips);
            return View();
        }


        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = Startup.Configuration["AppSettings:SiteEmailAddress"];

                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError("", "Could not send email, config problem");
                }

                if
                    (_mailService.SendMail(
                        "email",
                        email,
                        $"Contact Page from {model.Name} ({model.Email})",
                        model.Message))
                {
                    ModelState.Clear();

                    ViewBag.Message = "Mail Sent. Thanks";
                }
            }

            return View();
        }
    }
}
