namespace GymKalendar.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string NameOfExercise { get; set; } = string.Empty;
        public string? Reps { get; set; }
        public string? Weight { get; set; }

        // Связь с тренировкой (Внешний ключ)
        public int WorkoutId { get; set; }
        public Workout? Workout { get; set; }
    }
}