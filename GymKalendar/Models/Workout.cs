using GymKalendar.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GymKalendar.Models;
using GymKalendar.Controllers;


namespace GymKalendar.Models
{
    public class Workout
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public int UserId { get; set; } // Foreign key to the User


        public List<Exercise> Exercises { get; set; } = new List<Exercise>();

    }
}
