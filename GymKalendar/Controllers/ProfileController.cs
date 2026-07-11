using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GymKalendar.Data;
using GymKalendar.Models;


namespace GymKalendar.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _db;

        public ProfileController(AppDbContext db)
        {
            _db = db;
        }


        [HttpGet]
        public IActionResult Index()
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge(); // Если кука протухла, кидаем на логин

            int currentUserId = int.Parse(userIdStr);

            
            var user = _db.Users.FirstOrDefault(u => u.Id == currentUserId);
            if (user == null)
            {
                return NotFound();
            }

            var fitnessData = _db.UsersData.FirstOrDefault(ud => ud.UserId == currentUserId);

            int userAge = user.Age;
            double userHeight = fitnessData?.Height ?? 0;
            double userWeight = fitnessData?.Weight ?? 0;
            int activityLevel = fitnessData?.ActivityLevel ?? 1;

            double calculatedCalories = 0;

            // Считаем только если рост и вес заполнены
            if (userHeight > 0 && userWeight > 0 && userAge > 0)
            {
                // Базовый обмен веществ (BMR) для мужчин
                double bmr = (10 * userWeight) + (6.25 * userHeight) - (5 * userAge) + 5;

                // Превращаем твой уровень 1-5 в реальный коэфф активности
                double activityMultiplier = activityLevel switch
                {
                    1 => 1.2,  // Сидячий
                    2 => 1.375, // Легкий спорт
                    3 => 1.55,  // Средний спорт
                    4 => 1.725, // Тяжелый спорт
                    5 => 1.9,   // Хардкор
                    _ => 1.2
                };

                calculatedCalories = bmr * activityMultiplier;
            }




            var viewModel = new ProfileViewModel
            {
                Name = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Email = user.Email,
                Phone = user.Phone,


                Height = fitnessData?.Height ?? 0,
                Weight = fitnessData?.Weight ?? 0,
                ActivityLevel = fitnessData?.ActivityLevel ?? 1,
                Calories = calculatedCalories
            };

            return View(viewModel);

        }


        [HttpPost]
        public IActionResult Save(ProfileViewModel model)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge(); // Если кука протухла, кидаем на логин

            int currentUserId = int.Parse(userIdStr);

            var fitnessData = _db.UsersData.FirstOrDefault(ud => ud.UserId == currentUserId);

            if (fitnessData == null) 
            { 
                fitnessData = new UserData { UserId = currentUserId };

                _db.UsersData.Add(fitnessData);
            }    


            fitnessData.Height = model.Height;
            fitnessData.Weight = model.Weight;
            fitnessData.ActivityLevel = model.ActivityLevel;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }


        [HttpGet]
        // Пример маршрута, если ID передается через URL (например, /Profile/Save/5)
        public IActionResult Save()
        {

            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Challenge(); // Если кука протухла, кидаем на логин

            int currentUserId = int.Parse(userIdStr);

            // 1. Получаем данные из базы (замени на свой контекст данных/репозиторий)
            var userData = _db.UsersData.FirstOrDefault(u => u.UserId == currentUserId);
            var mainProfile = _db.Users.FirstOrDefault(u => u.Id == currentUserId); // Твоя основная таблица юзеров

            if (userData == null || mainProfile == null)
            {
                return NotFound();
            }

            // 2. Заполняем ProfileViewModel
            var viewModel = new ProfileViewModel
            {
                // Данные, которые будут скрыты, но важны для контекста
                Name = mainProfile.FirstName,
                LastName = mainProfile.LastName,
                Age = mainProfile.Age,
                Email = mainProfile.Email,
                Phone = mainProfile.Phone,

                // Данные, которые пользователь БУДЕТ редактировать в Save.cshtml
                Height = userData.Height,
                Weight = userData.Weight,
                ActivityLevel = userData.ActivityLevel
            };

            // 3. Открываем страницу Save.cshtml с заполненными полями
            return View("Save", viewModel);
        }
    }

}
