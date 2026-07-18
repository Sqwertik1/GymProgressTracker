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
                        Reps = int.TryParse(exDto.Reps, out int reps) ? reps : 0,
                        Weight = double.TryParse(exDto.Weight, out double weight) ? weight : 0
                    };
                    workout.Exercises.Add(exercise);
                }
            }

            _db.Workouts.Add(workout);
            _db.SaveChanges();

            return RedirectToAction("Index", "Dashboard");

        }


        public IActionResult Details(int id)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdStr ?? "0");


            var workouts = _db.Workouts
                .Include(w => w.Exercises)
                .FirstOrDefault(w => w.Id == id);

            if(workouts == null)
                return NotFound();


            return View(workouts);


        } 

        public IActionResult CreateTemplateFromWorkout (int workoutId)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdStr ?? "0");


            var workout = _db.Workouts
                .Include(w => w.Exercises)
                .FirstOrDefault(w => w.Id == workoutId);

            if(workout == null)
            {
                return NotFound();
            }

            WorkoutTemplate newTemplate = new WorkoutTemplate
            {
                Name = workout.Name,
                UserId = workout.UserId
            };


            foreach(var realExercise in workout.Exercises)
            {
                ExerciseTemplate exerciseTemplate = new ExerciseTemplate
                {
                    Name = realExercise.NameOfExercise,
                };

                newTemplate.Exercises.Add(exerciseTemplate);
            }

            _db.WorkoutTemplates.Add(newTemplate);

            _db.SaveChanges();

            return RedirectToAction("Templates");
        }

        public IActionResult StartWorkoutfromTemplate(int templateId)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdStr ?? "0");

            var template = _db.WorkoutTemplates
                .Include(w => w.Exercises)
                .FirstOrDefault(w => w.Id == templateId);

            if(template == null)
            {
                return NotFound();
            }
            var newWorkout = new Workout
            {
                Name = template.Name,
                Date = DateTime.Now,
                UserId = template.UserId
            };

            foreach(var templateExercise in template.Exercises)
            {
                var realExercise = new Exercise 
                {
                    NameOfExercise = templateExercise.Name,
                    Weight = 0,
                    Reps = 0
                };
                newWorkout.Exercises.Add(realExercise);
            }

            _db.Workouts.Add(newWorkout);
            _db.SaveChanges();

            return RedirectToAction("Create", "Workout", new { id = newWorkout.Id });


        }

        public IActionResult Templates()
        {
            // 1. Достаем ID текущего пользователя
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdStr ?? "0");

            // 2. Вытаскиваем из базы только его шаблоны
            var userTemplates = _db.WorkoutTemplates
                .Where(t => t.UserId == userId)
                .ToList();

            // 3. Отдаем этот список в представление
            return View(userTemplates);
        }

    }
}   
