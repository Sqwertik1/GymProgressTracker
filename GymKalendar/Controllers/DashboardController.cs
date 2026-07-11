using GymKalendar.Data;
using GymKalendar.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

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


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Удаляем куку авторизации "CookieAuth"
            await HttpContext.SignOutAsync("CookieAuth");

            // Перенаправляем на действие Register контроллера Home
            return RedirectToAction("Register", "Home");
        }

        [HttpGet]
        public IActionResult Calendar()
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = int.Parse(userIdStr ?? "0");

            // Вытаскиваем из базы даты всех тренировок ТЕКУЩЕГО юзера 
            // и превращаем их в список строк формата "YYYY-MM-DD" для удобства JS
            var trainedDays = _db.WorkoutDays
                .Where(w => w.UserId == currentUserId)
                .Select(w => w.Date.ToString("yyyy-MM-dd"))
                .ToList();

            // Передаем этот список в HTML через ViewBag
            ViewBag.TrainedDays = trainedDays;

            return View();
        }

        [HttpPost]
        public IActionResult ToggleDay(string dateString)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int currentUserId = int.Parse(userIdStr ?? "0");

            if (!DateTime.TryParse(dateString, out DateTime targetDate))
            {
                return BadRequest();
            }

            // Ищем, есть ли уже такая тренировка у юзера в этот день
            var existingWorkout = _db.WorkoutDays
                .FirstOrDefault(w => w.UserId == currentUserId && w.Date == targetDate.Date);

            if (existingWorkout != null)
            {
                // Если была — удаляем (сняли зеленую отметку)
                _db.WorkoutDays.Remove(existingWorkout);
            }
            else
            {
                // Если не было — добавляем (поставили зеленую отметку)
                _db.WorkoutDays.Add(new WorkoutDay
                {
                    UserId = currentUserId,
                    Date = targetDate.Date
                });
            }

            _db.SaveChanges();
            return Ok(); // Возвращаем статус "всё прошло успешно"
        }
    }
}
