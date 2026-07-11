using System;

namespace GymKalendar.Models
{
    public class WorkoutDay
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Чья тренировка
        public DateTime Date { get; set; } // В какой день (только дата, без времени)
    }
}
