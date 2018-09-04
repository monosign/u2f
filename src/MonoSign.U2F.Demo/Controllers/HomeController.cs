using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MonoSign.U2F.Demo.Models;

namespace MonoSign.U2F.Demo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel value)
        {
            if (value == null || string.IsNullOrEmpty(value.UserName) || string.IsNullOrEmpty(value.Password))
            {
                ViewBag.Error = "Please fill all fields.";
                return View("Index");
            }

            var user = App.Users.FirstOrDefault(x => x.UserName.Equals(value.UserName));

            if (user == null || !user.Password.Equals(value.Password))
            {
                ViewBag.Error = "User not found.";
                return View("Index");
            }

            App.CurrentUser = user;

            return RedirectToAction("CurrentUser");
        }

        [HttpPost]
        public IActionResult Register(LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Please fill all fields.";
                return View("Index");
            }

            var user = App.Users.FirstOrDefault(x => x.UserName.Equals(model.UserName));
            if (user != null)
            {
                ViewBag.Error = "This user is already registered.";
                return View("Index");
            }

            user = new User
            {
                UserName = model.UserName,
                Password = model.Password
            };

            App.CurrentUser = user;
            return RedirectToAction("CurrentUser");
        }

        [HttpPost]
        public IActionResult RegisterDevice(RegisterDeviceModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.AppId) || string.IsNullOrEmpty(model.Challenge) || string.IsNullOrEmpty(model.RawRegisterResponse))
            {
                ViewBag.Error = "Invalid Data";
                return View("CurrentUser", GetRegistrationModel());
            }

            try
            {
                var u2F = new FidoUniversalTwoFactor();

                App.Registrations.TryGetValue(model.Challenge, out var startedRegistration);

                if (startedRegistration == null)
                {
                    ViewBag.Error = "Invalid Started Registration.";
                    return View("CurrentUser", GetRegistrationModel());
                }

                var facetIds = new List<FidoFacetId> {new FidoFacetId(startedRegistration.AppId.ToString())};
                var deviceRegistration = u2F.FinishRegistration(startedRegistration, model.RawRegisterResponse, facetIds);

                if (deviceRegistration == null)
                {
                    ViewBag.Error = "Invalid Device Registration.";
                    return View("CurrentUser", GetRegistrationModel());
                }
                
                App.AddDeviceRegistration(App.CurrentUser.UserName, deviceRegistration);
                App.Registrations.Remove(model.Challenge);

                ViewBag.Message = "Device has been registered.";

                return View("CurrentUser", GetRegistrationModel());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ViewBag.Error = exception.Message;
            }

            return View("CurrentUser", GetRegistrationModel());
        }

        public IActionResult CurrentUser()
        {
            if (App.CurrentUser == null)
            {
                return RedirectToAction("Index");
            }

            return View(GetRegistrationModel());
        }

        public RegisterDeviceModel GetRegistrationModel()
        {
            var u2F = new FidoUniversalTwoFactor();
            var url = new FidoAppId(string.Format("{0}://{1}", Request.Scheme, Request.Host));
            var startedRegistration = u2F.StartRegistration(url);

            var model = new RegisterDeviceModel
            {
                AppId = startedRegistration.AppId.ToString(),
                Challenge = startedRegistration.Challenge
            };

            App.AddRegistration(model.Challenge, startedRegistration);
            return model;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}