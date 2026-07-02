using GymKalendar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GymKalendar.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // ОБЯЗАТЕЛЬНО добавь этот using наверх
using Microsoft.AspNetCore.Authentication; // И этот тоже

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
        public IActionResult Register(RegisterViewModel dto)
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
        public async Task<IActionResult> Login(LoginViewModel dto)
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


            // --- МАГИЯ АВТОРИЗАЦИИ MVC НАЧИНАЕТСЯ ТУТ ---

            // 1. Создаем список "Клеймов" (Утверждений о пользователе). 
            // Это инфа, которую сервер зашьет внутрь куки, чтобы всегда знать, кто залогинен.
            var claims = new List<Claim>
    {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Зашиваем ID
            new Claim(ClaimTypes.Name, user.FirstName)               // Зашиваем Имя
    };

            // 2. Создаем удостоверение личности и говорим, что оно работает на куках
            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

            // 3. Выписываем "паспорт" браузеру! 
            // Движок ASP.NET Core сам создаст зашифрованный куки-файл и отправит его в браузер пользователя.
            await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

            // --- КОНЕЦ МАГИИ ---

            // Теперь перенаправляем в личный кабинет!
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
