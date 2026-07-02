using GymKalendar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GymKalendar.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GymKalendar.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _db;


        public HomeController(AppDbContext db)
        {
            _db = db;
        }


        [HttpPost]
        public IActionResult IActionResult(RegisterViewModel dto)
        {
            if(!ModelState.IsValid)
            {
                return View(dto);
            }

            bool isEmailTaken = _db.Users.Any(u => u.Email == dto.Email);
            if (isEmailTaken)
            {
                 ModelState.AddModelError("Email", "Email is already taken.");
                return View(dto);
            }

            User newUser = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Age = dto.Age,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = dto.Password // In a real application, make sure to hash the password before storing it
            };
            _db.Users.Add(newUser);
            _db.SaveChanges();

            return RedirectToAction("Login");

        }


        [HttpPost]
        public IActionResult Login(LoginViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            var user = _db.Users.FirstOrDefault(u => (u.Email == dto.LoginInput || u.Phone == dto.LoginInput) && u.Password == dto.Password);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(dto);
            }
            return RedirectToAction("Index", "Dashboard");
        }






        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        
    }

}
