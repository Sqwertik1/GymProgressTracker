using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymKalendar.Data;
using GymKalendar.Models;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace GymKalendar.Controllers
{
    [Authorize]
    public class WorkoutController : Controller
    {
        private readonly AppDbContext _db;

        public WorkoutController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(CreateWorkoutDto dto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdStr);


            var workout = new Workout
            {
                Name = dto.Name,
                Description = dto.Description,
                Date = DateTime.Now,
                UserId = userId,
                Exercises = new List<Exercise>()
            };

            if(dto.Exercises != null)
            {
                foreach(var exDto in dto.Exercises)
                {
                    var exercise = new Exercise
                    {
                        NameOfExercise = exDto.NameOfExercise,
                        Reps = exDto.Reps,
                        Weight = exDto.Weight
                    };
                    workout.Exercises.Add(exercise);
                }
            }

            _db.Workouts.Add(workout);
            _db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");

        }


    }
}
