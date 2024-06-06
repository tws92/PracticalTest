using Newtonsoft.Json;
using PracticalTest.Models;
using PracticalTest.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PracticalTest.Controllers
{
    public class UserController : Controller
    {
        private UserRepository _userRepository;

        public UserController() { _userRepository = new UserRepository(); }

        // GET: User
        public ActionResult Index(string NRICOrName)
        {
            List<UsersModel> users = new List<UsersModel>();
            users = _userRepository.GetUser(NameORNRIC: NRICOrName, userId: 0);
            users.ForEach(u => u.SubjectCount = u.SelectedSubjectList.Count);
            return View(users);
        }

        // GET: User/Register
        public ActionResult Register()
        {
            ViewBag.Subjects = new SelectList(_userRepository.GetAllSubjects(), "Id", "Subject");
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UsersModel user)
        {
            if (ModelState.IsValid)
            {
                // If NRIC exists, display error message
                if (_userRepository.GetUser(NameORNRIC: user.NRIC, userId: 0).Count > 0)
                {
                    ModelState.AddModelError("NRIC", "NRIC already exists.");
                    ViewBag.Subjects = new SelectList(_userRepository.GetAllSubjects(), "Id", "Subject");
                    return View(user);
                }

                user.Age = AgeCalculator(user.Birthday);
                _userRepository.InsertUser(user);
                _userRepository.InsertAuditLog(_userRepository.GetUser(NameORNRIC: user.NRIC, userId: 0).FirstOrDefault().Id, string.Empty, JsonConvert.SerializeObject(user));
                return RedirectToAction("Index");
            }

            ViewBag.Subjects = new SelectList(_userRepository.GetAllSubjects(), "Id", "Subject");
            return View(user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            List<UsersModel> users = new List<UsersModel>();
            users = _userRepository.GetUser(NameORNRIC: string.Empty, userId: id);
            
            if (users.Count == 0)
            {
                return HttpNotFound();
            }
            UsersModel user = users.FirstOrDefault();
            user.Subjects = user.SelectedSubjectList.Select(x => x.Id).ToList();
            ViewBag.Subjects = new SelectList(_userRepository.GetAllSubjects(), "Id", "Subject", user.SelectedSubjectList.Select(x => x.Id).ToList());
            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UsersModel user)
        {
            if (ModelState.IsValid)
            {
                var prevVal = _userRepository.GetUser(NameORNRIC: user.NRIC, userId: user.Id).FirstOrDefault();
                user.Age = AgeCalculator(user.Birthday);
                _userRepository.UpdateUser(user);
                if (JsonConvert.SerializeObject(prevVal) != JsonConvert.SerializeObject(user))
                {
                    _userRepository.InsertAuditLog(user.Id, JsonConvert.SerializeObject(prevVal), JsonConvert.SerializeObject(user));
                }
                return RedirectToAction("Index");
            }

            user.Subjects = user.SelectedSubjectList.Select(x => x.Id).ToList();
            ViewBag.Subjects = new SelectList(_userRepository.GetAllSubjects(), "Id", "Subject", user.SelectedSubjectList.Select(x => x.Id).ToList());

            return View(user);
        }

        private int AgeCalculator(DateTime birthday)
        {
            // Save today's date.
            var today = DateTime.Today;

            // Calculate the age.
            var age = today.Year - birthday.Year;

            // Go back to the year in which the person was born in case of a leap year
            if (birthday.Date > today.AddYears(-age)) age--;

            return age;
        }
    }
}