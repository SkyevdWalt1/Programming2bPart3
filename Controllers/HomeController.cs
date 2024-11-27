using Microsoft.AspNetCore.Mvc;
using Part_2.Models;
using System.Diagnostics;
using Part_2.Services;

namespace Part_2.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger) {
            _logger = logger;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Login() {
            return View();
        }

        public IActionResult LogUserIn(LoginModel model) {
            User user = new();
            if (ModelState.IsValid) {
                bool LoggedIn = new Db().LoginUser(model.Username, model.Password);
                if (LoggedIn) {
                    user = new Db().GetUser(model.Username);
                    HttpContext.Session.SetInt32("user_id", user.ID);
                    Navbar.isLoggedIn = true;
                    if (user.IsAdmin) {
                        string role = new Db().GetRole(user.ID);
                        RoleController.Role = role;
                        return RedirectToAction("Index", "Admin");
                    } else {
                        return RedirectToAction("Index", "User");
                    }
                } else {
                    return Redirect("Login");
                } 
            } else {
                return Redirect("Login");
            }
        }

        public IActionResult SignUp() {
            return View();
        }

        public IActionResult SignUserIn(SignInModel model) {
            if (ModelState.IsValid) {
                bool username_exists = new Db().UsernameExists(model.Username);
                if (username_exists) {
                    return Redirect("SignUp");
                }

                if (model.Password1 != model.Password2) {
                    return Redirect("SignUp");
                }

                new Db().SignUpUser(model.Username, model.Password1);
                return RedirectToAction("SignUpReceived", "Home", model);
            } else {
                return Redirect("SignUp");
            }
        }

        public IActionResult SignUpReceived(SignInModel model) {
            return View(model);
        }

        public IActionResult LogOut() {
            Navbar.isLoggedIn = false;
            HttpContext.Session.Remove("user_id");
            return Redirect("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
