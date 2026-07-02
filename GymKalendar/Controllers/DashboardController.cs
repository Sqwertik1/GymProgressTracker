using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GymKalendar.Controllers
{
    [Authorize] // <-- Сюда не зайти без логина. Если куки нет, .NET сам перекинет на /Home/Login
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Извлекаем имя пользователя, которое мы зашили в куку при логине
            // (Помнишь строчку new Claim(ClaimTypes.Name, user.FirstName)?)
            string? userName = User.Identity?.Name;

            // Извлекаем ID пользователя (если понадобится для базы данных)
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Передаем имя в HTML-страницу через специальный мешок ViewBag
            ViewBag.SportsmanName = userName ?? "Атлет";

            return View();
        }
    }
}
