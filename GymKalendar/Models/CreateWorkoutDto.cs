namespace GymKalendar.Models
{
    public class CreateWorkoutDto
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Список упражнений, который прилетит из формы
        public List<ExerciseDto> Exercises { get; set; } = new List<ExerciseDto>();
    }

    public class ExerciseDto
    {
        public string NameOfExercise { get; set; }
        public string Reps { get; set; }
        public string Weight { get; set; }
    }
}
