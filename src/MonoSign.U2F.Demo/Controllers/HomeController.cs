using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Internal;
using MonoSign.U2F.Demo.Models;

namespace MonoSign.U2F.Demo.Controllers
{
    public class HomeController : Controller
    {
        private FidoAppId AppId { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AppId = new FidoAppId(string.Format("{0}://{1}", Request.Scheme, Request.Host));
            
            base.OnActionExecuting(context);
        }

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
        public IActionResult RegisterDeviceRequest()
        {
            if (App.CurrentUser == null)
            {
                return BadRequest(new {error = "You must login.", code = 401});
            }

            return Ok(GetRegistrationModel());
        }

        [HttpPost]
        public IActionResult RegisterDevice(RegisterDeviceModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.AppId) || string.IsNullOrEmpty(model.Challenge) || string.IsNullOrEmpty(model.RawRegisterResponse))
            {
                return BadRequest(new {error = "Invalid Data", code = 400});
            }

            try
            {
                var device = App.CurrentUser.Devices.FirstOrDefault(x => x.Name.Equals(model.DeviceName));

                if (device != null)
                {
                    return BadRequest(new {error = "This name already used on another device.", code = 400});
                }

                var u2F = new FidoUniversalTwoFactor();

                App.Registrations.TryGetValue(model.Challenge, out var startedRegistration);

                if (startedRegistration == null)
                {
                    return BadRequest(new {error = "Invalid Started Registration.", code = 400});
                }

                var facetIds = new List<FidoFacetId> {new FidoFacetId(startedRegistration.AppId.ToString())};
                var deviceRegistration = u2F.FinishRegistration(startedRegistration, model.RawRegisterResponse, facetIds);

                if (deviceRegistration == null)
                {
                    return BadRequest(new {error = "Invalid Device Registration.", code = 400});
                }

                App.AddDeviceRegistration(model.DeviceName, deviceRegistration);
                App.Registrations.Remove(model.Challenge);

                return Ok(new {message = "Device has been registered.", code = 200, redirect = Url.Action("CurrentUser")});
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return BadRequest(new {error = exception.Message, code = 500});
            }
        }

        [HttpPost]
        public IActionResult AuthenticateDeviceRequest(AuthenticateDeviceModel model)
        {
            if (App.CurrentUser == null)
            {
                return BadRequest(new {error = "You must login.", code = 401});
            }

            if (model == null || string.IsNullOrEmpty(model.KeyHandle))
                return BadRequest(new {error = "Invalid device id.", code = 401});

            var device = App.CurrentUser.Devices.FirstOrDefault(x => x.Identifier.Equals(model.KeyHandle));

            if (device == null)
                return BadRequest(new {error = "Device not found.", code = 401});

            return Ok(GetAuthenticationModel(device));
        }

        [HttpPost]
        public IActionResult AuthenticateDevice(AuthenticateDeviceModel model)
        {
            if (App.CurrentUser == null)
            {
                return BadRequest(new {error = "You must login.", code = 401});
            }

            if (model == null || string.IsNullOrEmpty(model.KeyHandle))
                return BadRequest(new {error = "Invalid device id.", code = 400});

            var device = App.CurrentUser.Devices.FirstOrDefault(x => x.Identifier.Equals(model.KeyHandle));

            if (device == null)
                return BadRequest(new {error = "Device not found.", code = 400});


            var u2F = new FidoUniversalTwoFactor();

            var deviceRegistration = FidoDeviceRegistration.FromJson(device.Data);
            if (deviceRegistration == null)
            {
                return BadRequest(new {error = "Unknown key handle.", code = 400});
            }

            var challenge = model.Challenge;

            var startedAuthentication = new FidoStartedAuthentication(AppId, challenge, FidoKeyHandle.FromWebSafeBase64(model.KeyHandle ?? ""));
            var facetIds = new List<FidoFacetId> {new FidoFacetId(AppId.ToString())};

            var counter = u2F.FinishAuthentication(startedAuthentication, model.RawAuthenticateResponse, deviceRegistration, facetIds);
            deviceRegistration.Counter = counter;
            device.Usage++;

            return Ok(new {message = "Device has been authenticated.", code = 200, redirect = Url.Action("CurrentUser")});
        }

        [HttpPost]
        public IActionResult RemoveDevice(Device value)
        {
            if (App.CurrentUser == null)
            {
                return BadRequest(new {error = "You must login.", code = 401});
            }

            if (value == null || string.IsNullOrEmpty(value.Identifier))
            {
                return BadRequest(new {error = "You must send device identifier.", code = 401});
            }

            var device = App.CurrentUser.Devices.FirstOrDefault(x => x.Identifier.Equals(value.Identifier));

            if (device != null)
            {
                App.CurrentUser.Devices.Remove(device);
            }

            return Ok(new {message = "Device has been removed.", code = 200, redirect = Url.Action("CurrentUser")});
        }

        public IActionResult CurrentUser()
        {
            if (App.CurrentUser == null)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        public AuthenticateDeviceModel GetAuthenticationModel(Device device)
        {
            var u2F = new FidoUniversalTwoFactor();
            var deviceRegistration = FidoDeviceRegistration.FromJson(device.Data);
            var authentication = u2F.StartAuthentication(AppId, deviceRegistration);

            var model = new AuthenticateDeviceModel
            {
                AppId = authentication.AppId.ToString(),
                Challenge = authentication.Challenge,
                KeyHandle = device.Identifier
            };

            return model;
        }

        public RegisterDeviceModel GetRegistrationModel()
        {
            var u2F = new FidoUniversalTwoFactor();
            var startedRegistration = u2F.StartRegistration(AppId);

            var model = new RegisterDeviceModel
            {
                AppId = startedRegistration.AppId.ToString(),
                Challenge = startedRegistration.Challenge
            };

            if (App.CurrentUser.Devices.Any())
                model.RegisteredKeys.AddRange(App.CurrentUser.Devices.Select(x => x.Identifier).ToList());

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