using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GymKalendar.Data;
using GymKalendar.Models;

namespace GymKalendar.Controllers
{
    [Authorize] // <-- Сюда не зайти без логина. Если куки нет, .NET сам перекинет на /Home/Login
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int userId = int.Parse(userIdStr ?? "0");


            // Извлекаем имя пользователя, которое мы зашили в куку при логине
            // (Помнишь строчку new Claim(ClaimTypes.Name, user.FirstName)?)
            string? userName = User.Identity?.Name;


            // Передаем имя в HTML-страницу через специальный мешок ViewBag
            ViewBag.SportsmanName = userName ?? "Атлет";



            var userWokrouts = _db.Workouts
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .ToList();

            return View(userWokrouts);
        }
    }
}
