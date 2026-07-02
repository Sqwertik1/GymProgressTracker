using GymKalendar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using GymKalendar.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GymKalendar.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Register()
        {
            return View();
        }
        
    }
}
