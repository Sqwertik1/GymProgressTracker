namespace GymKalendar.Models
{
    public class WorkoutTemplate
    {

        public int Id { get; set; }

        public string Name { get; set; }    

        public int UserId { get; set; }

        public List<ExerciseTemplate> Exercises { get; set; } = new List<ExerciseTemplate>();

    }
}
