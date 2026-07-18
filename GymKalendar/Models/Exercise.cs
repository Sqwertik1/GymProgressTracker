namespace GymKalendar.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string NameOfExercise { get; set; } = string.Empty;
        public int? Reps { get; set; }
        public double? Weight { get; set; }

        // Связь с тренировкой (Внешний ключ)
        public int WorkoutId { get; set; }
        public Workout? Workout { get; set; }
    }
}