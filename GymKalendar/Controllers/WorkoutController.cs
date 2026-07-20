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

        public IActionResult Create(int? templateId)
        {
            var dto = new CreateWorkoutDto
            {
                Exercises = new List<ExerciseDto>()
            };

            if (templateId.HasValue)
            {
                var template = _db.WorkoutTemplates
                    .Include(t => t.Exercises)
                    .FirstOrDefault(t => t.Id == templateId.Value);

                if (template != null)
                {
                    dto.Name = template.Name;

                    foreach (var templateEx in template.Exercises)
                    {
                        dto.Exercises.Add(new ExerciseDto
                        {
                            NameOfExercise = templateEx.Name,
                            Reps = "0",
                            Weight = "0"
                        });
                    }
                }
            }

      


            return View(dto);
        }


        [HttpPost]
        public IActionResult Create(CreateWorkoutDto dto)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdStr);


            var workout = new Workout
            {
                Name = dto.Name,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? "" : dto.Description,
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

        [HttpPost]
        public IActionResult DeleteTemplate(int templateId)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdStr ?? "0");


            var template = _db.WorkoutTemplates
                .Include(t => t.Exercises)
                .FirstOrDefault(w => (w.Id == templateId) && (w.UserId == userId));

            if (template == null) 
            {
                return NotFound();
            }

            _db.WorkoutTemplates.Remove(template);
            _db.SaveChanges();
            return RedirectToAction("Templates");

        }

        public IActionResult StartWorkoutfromTemplate(int templateId)
        {

            return RedirectToAction("Create", new { templateId = templateId });

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
