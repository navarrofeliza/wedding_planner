using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("")]

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Welcome")]
        public IActionResult Welcome()
        {
            var weddings = _context.Weddings.Include(wed => wed.RSVPs).OrderByDescending(wed => wed.Date);
            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");

            var responded = weddings.Where(wed => wed.RSVPs.Any(res => res.UserId == 1));

            return View("Welcome", weddings);
        }

        [HttpGet]
        [Route("Register")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(use => use.Email == use.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                }
                PasswordHasher<User> Hash = new PasswordHasher<User>();
                user.Password = Hash.HashPassword(user, user.Password);
                User NewUser = new User
                {
                    FirstName = @user.FirstName,
                    LastName = @user.LastName,
                    Email = @user.Email,
                    Password = @user.Password,
                };
                var userEntity = _context.Add(NewUser).Entity;
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", NewUser.UserId);
                return RedirectToAction("Welcome");
            }
            return View("Index");
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(User userSubmit)
        {
            var userDB = _context.Users.FirstOrDefault(u = u.Email == userSubmit.Email);

            if (userDB == null)
            {
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }
            var Hash = new PasswordHasher<User>();
            var result = Hash.VerifyHashedPassword(userSubmit, userDB.Password, userSubmit.Password);

            if (result == 0)
            {
                Console.WriteLine("Invalid Password");
                ModelState.AddModelError("Password", "Invalid Password");
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", userDB.UserId);
            return RedirectToAction("Welcome");
        }

        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("NewWedding")]
        public IActionResult NewWedding()
        {
            return View("NewWedding");
        }

        [HttpGet]
        [Route("CreateWedding")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            if (ModelState.IsValid)
            {
                if (newWedding.Date < DateTime.Today)
                {
                    ModelState.AddModelError("Date", "Please select a future date");
                    return View("NewWedding");
                }
                else
                {
                    Wedding thisWedding = new Wedding
                    {
                        Address = newWedding.Address,
                        Date = newWedding.Date,
                        ToBeNameOne = newWedding.ToBeNameOne,
                        ToBeNameTwo = newWedding.ToBeNameTwo,
                        UserId = (int)HttpContext.Session.GetInt32("UserId")
                    };
                    _context.Add(thisWedding);
                    _context.SaveChanges();
                    return RedirectToAction("Welcome");
                }
            }
            else
            {
                if (newWedding.Date < DateTime.Today)
                {
                    ModelState.AddModelError("Date", "Please set a future date!");
                }
                return View("NewWedding");
            }
        }

        [HttpGet]
        [Route("ViewWedding/{weddingId}")]
        public IActionResult ViewWedding(int weddingId)
        {
            Wedding wedding = _context.Weddings.Include(r => r.RSVPs).ThenInclude(use => use.User).Where(wed => wed.WeddingId == weddingId).SingleOrDefault();

            ViewBag.Wedding = wedding;
            ViewBag.Address = wedding.Address;
            return View("ViewWedding");
        }

        [Route("RSVP")]
        public IActionResult RSVP(int weddingId)
        {
            RSVP newRSVP = new RSVP
            {
                UserId = (int)HttpContext.Session.GetInt32("UserId"),
                WeddingId = weddingId
            };

            _context.Add(newRSVP);
            _context.SaveChanges();

            return RedirectToAction("Welcome");
        }

        [Route("UnRSVP")]
        public IActionResult UnRSVP(int weddingId)
        {
            RSVP attender = _context.RSVP
            .SingleOrDefault(u => u.UserId == HttpContext.Session
            .GetInt32("UserId") && u.WeddingId == weddingId);

            _context.RSVP.Remove(attender);
            _context.SaveChanges();

            return RedirectToAction("Welcome");
        }

        [Route("Delete")]
        public IActionResult Delete(int weddingId)
        {
            Wedding this_wedding = _context.Weddings
            .SingleOrDefault(w => w.WeddingId == weddingId);

            List<RSVP> rsvps = _context.RSVP
            .Where(a => a.WeddingId == weddingId)
            .ToList();

            foreach (var attender in rsvps)
            {
                _context.RSVP.Remove(attender);
            }

            _context.Weddings.Remove(this_wedding);
            _context.SaveChanges();

            return RedirectToAction("Welcome");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
