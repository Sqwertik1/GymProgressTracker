namespace GymKalendar.Models
{
    public class ExerciseTemplate
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int WorkoutTemplateId { get; set; }

    }
}
