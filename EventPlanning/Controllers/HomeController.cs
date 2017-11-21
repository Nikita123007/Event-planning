using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EventPlanning.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Threading;

namespace EventPlanning.Controllers
{
    public class HomeController : Controller
    {
        EventContext db = new EventContext();
        SettingContext dbSetting = new SettingContext();
        
        public ActionResult Index(string registerEvent = null)
        {
            var events = db.Events;
            ViewData["admin"] = (IsAdmin(User.Identity.Name)) ? "true" : "false";
            ViewData["registerEvent"] = (registerEvent == null) ? "" : registerEvent;
            return View(events.ToList());
        }

        [Authorize(Roles = "user")]
        public ActionResult RegisterOnEvent(int id)
        {
            Event useEvent = db.Events.Include(p => p.Users).Include(p => p.BlackListUsers).Where(elem => elem.Id == id).FirstOrDefault();
            if (useEvent == null)
            {
                return HttpNotFound();
            }
            if ((useEvent.Users.Where(user => user.Name == User.Identity.Name).Count() == 0) && (useEvent.BlackListUsers.Where(user => user.Name == User.Identity.Name).Count() == 0) && (useEvent.MaxUsers > useEvent.Users.Count()))
            {
                useEvent.Users.Add(new User { Name = User.Identity.Name });
                db.Entry(useEvent).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("AboutEvent", "Home", new { id = id});
        }

        [Authorize(Roles = "user")]
        public ActionResult UnRegisterOnEvent(int id)
        {
            Event useEvent = db.Events.Include(p => p.Users).Where(elem => elem.Id == id).FirstOrDefault();
            if (useEvent == null)
            {
                return HttpNotFound();
            }
            if (useEvent.Users.Where(user => user.Name == User.Identity.Name).Count() != 0)
            {
                useEvent.Users.Remove(useEvent.Users.Where(user => user.Name == User.Identity.Name).First());
                db.Entry(useEvent).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("AboutEvent", "Home", new { id = id });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteEvent(int id)
        {
            Event delEvent = db.Events.Include(p => p.EventDescriptions).Include(p => p.Users).Where(elem => elem.Id == id).FirstOrDefault();
            if (delEvent == null)
            {
                return HttpNotFound();
            }
            db.Entry(delEvent).State = EntityState.Deleted;
            db.SaveChanges();
            return Redirect("/Home/Index");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult CreateNewEvent()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateNewEvent(string name, string date, string[] NameDescription, string[] Description)
        {
            string error = "";
            if ((name == null) || (name == ""))
            {
                error += "Поле имени не было заполненно.\n";
            }
            try
            {
                if ((date == null) || ((Convert.ToDateTime(date) - DateTime.Now).TotalSeconds < 0))
                {
                    error += "Поле даты заполненно не верно.\n";
                }
            }
            catch(Exception)
            {
                error += "Строка даты не соответствует формату";
            }
            if ((NameDescription != null) && (Description != null))
            {
                foreach (var NameDes in NameDescription)
                {
                    if ((NameDes == null) || (NameDes == ""))
                    {
                        error += "Наименование описания должны быть заполены";
                    }
                }
                foreach (var Des in Description)
                {
                    if ((Des == null) || (Des == ""))
                    {
                        error += "Описания должны быть заполены";
                    }
                }
            }
            if (error == "")
            {
                Event newEvent = new Event { Name = name, Date = Convert.ToDateTime(date) };
                if ((NameDescription != null) && (Description != null))
                {
                    for (int i = 0; i < NameDescription.Length; i++)
                    {
                        newEvent.EventDescriptions.Add(new EventDescription { Name = NameDescription[i], Description = Description[i] });
                    }
                }
                db.Events.Add(newEvent);
                db.SaveChanges();
                return RedirectToAction("Index", "Home", new { registerEvent = "Событие было успешно создано." });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { registerEvent = error });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult Setting(string result = "")
        {
            Setting setting = dbSetting.Settings.Include(p => p.EmailsInBlackList).First();
            ViewData["Message"] = result;
            SettingAndUsersModel settingAndUsers = new SettingAndUsersModel();
            settingAndUsers.Setting = setting;
            settingAndUsers.Users = dbSetting.Users.ToList();
            ViewData["admin"] = (IsAdmin(User.Identity.Name)) ? "true" : "false";
            return View(settingAndUsers);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteWithBLUserRegister(int id)
        {
            Setting setting = dbSetting.Settings.Include(p => p.EmailsInBlackList).FirstOrDefault();
            UserEmail userEmail = dbSetting.UsersEmails.Where(elem => elem.Id == id).FirstOrDefault();
            if (userEmail == null)
            {
                return HttpNotFound();
            }
            setting.EmailsInBlackList.Remove(userEmail);
            dbSetting.Entry(setting).State = EntityState.Modified;
            dbSetting.SaveChanges();
            return RedirectToAction("Setting", "Home", new { result = "Пользователь удалён из чёрного списка" });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteUserRegister(int id)
        {
            UserRegisterData userData = dbSetting.Users.Where(elem => elem.Id == id).FirstOrDefault();
            if (userData == null)
            {
                return HttpNotFound();
            }
            if (IsAdmin(userData.Email))
            {
                return RedirectToAction("Setting", "Home", new { result = "Пользователь не может быть удалён, так как он является администратором." });
            }
            dbSetting.Entry(userData).State = EntityState.Deleted;
            dbSetting.SaveChanges();
            return RedirectToAction("DeleteUser", "Account", new { name = userData.Email, password = userData.Password });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult AddInBlackListUserRegister(int id)
        {
            Setting setting = dbSetting.Settings.Include(p => p.EmailsInBlackList).FirstOrDefault();
            UserRegisterData userData = dbSetting.Users.Where(elem => elem.Id == id).FirstOrDefault();
            if (userData == null)
            {
                return HttpNotFound();
            }
            if (IsAdmin(userData.Email))
            {
                return RedirectToAction("Setting", "Home", new { result = "Пользователь не может быть добавлен в чс, так как он является администратором." });
            }
            UserEmail userEmail = new UserEmail { Email = userData.Email, Setting = setting };
            dbSetting.UsersEmails.Add(userEmail);
            setting.EmailsInBlackList.Add(userEmail);
            dbSetting.Entry(setting).State = EntityState.Modified;
            dbSetting.SaveChanges();
            
            return RedirectToAction("DeleteUserRegister", "Home", new { id = id });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult UpdateSetting(Setting setting)
        {
            string result = "";
            if ((setting == null) || (setting.SendEmail == null) || (setting.SendPassword == null) || (setting.AutocastingHours == default(int)))
            {
                result += "При обновлении возникла ошибка. Возможно вы не заполнили все поля.";
            }
            else
            {
                dbSetting.Entry(setting).State = EntityState.Modified;
                dbSetting.SaveChanges();
                result += "Данные обнавлены успешно.";
            }
            return RedirectToAction("Setting", "Home", new { result = result});
        }

        [HttpPost]
        public ActionResult EventSearch(string name, string date)
        {
            List<Event> events;
            if ((name == null) || (name == ""))
            {
                events = db.Events.ToList();
            }
            else
            {
                events = db.Events.Where(elem => elem.Name == name).ToList();
            }
            if (date != "")
            {
                string[] partDate = date.Split('-');
                date = partDate[2] + "." + partDate[1] + "." + partDate[0] + " 00:00:00";
                DateTime eventDay = Convert.ToDateTime(date);
                for (int i = 0; i < events.Count(); i++)
                {
                    if (((events[i].Date - eventDay).TotalDays > 1) || ((events[i].Date - eventDay).TotalDays < 0))
                    {
                        events.Remove(events[i]);
                        i--;
                    }
                }
            }
            return PartialView(events);
        }

        public ActionResult About()
        {
            ViewData["admin"] = (IsAdmin(User.Identity.Name)) ? "true" : "false";
            return View();
        }

        [Authorize(Roles = "admin")]
        public string Test()
        {
            return "Hello admin!";
        }

        [Authorize(Roles = "admin, user")]
        public ActionResult AboutEvent(int id)
        {
            Event aboutEvent = db.Events.Include(p => p.EventDescriptions).Include(p => p.BlackListUsers).Include(p => p.Users).Where(elem => elem.Id == id).FirstOrDefault();
            if (aboutEvent == null)
            {
                return HttpNotFound();
            }
            ViewData["admin"] = (IsAdmin(User.Identity.Name)) ? "true" : "false";
            ViewData["registered"] = (aboutEvent.Users.Where(user => user.Name == User.Identity.Name).Count() > 0) ? "true" : "false";
            ViewData["IsBlackList"] = (aboutEvent.BlackListUsers.Where(user => user.Name == User.Identity.Name).Count() > 0) ? "true" : "false";
            ViewData["LimitUsers"] = (aboutEvent.Users.Count() >= aboutEvent.MaxUsers) ? "true" : "false";

            EventAndUsersModel aboutEventAndUsers = new EventAndUsersModel();
            aboutEventAndUsers.Event = aboutEvent;
            /*foreach(var user in aboutEvent.BlackListUsers)
            {
                UserRegisterData userData = dbSetting.Users.Where(elem => elem.Email == user.Name).First();
                aboutEventAndUsers.Users.Add(userData);
            }*/
            /*foreach (var user in aboutEvent.Users)
            {
                UserRegisterData userData = dbSetting.Users.Where(elem => elem.Email == user.Name).First();
                aboutEventAndUsers.Users.Add(userData);
            }*/

            return View(aboutEventAndUsers);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult EditEvent(int id)
        {
            Event useEvent = db.Events.Include(p => p.EventDescriptions).Where(elem => elem.Id == id).FirstOrDefault();
            if (useEvent == null)
            {
                return HttpNotFound();
            }
            return View(useEvent);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult EditEvent(int id, string name, string date, int MaxUsers, string[] NameDescription, string[] Description)
        {
            string error = "";
            if ((name == null) || (name == ""))
            {
                error += "Поле имени не было заполненно.\n";
            }
            if (MaxUsers < 0)
            {
                error += "Максимальное кол-во подписанны пользователей не может быть отрицательным.";
            }
            try
            {
                if ((date == null) || ((Convert.ToDateTime(date) - DateTime.Now).TotalSeconds < 0))
                {
                    error += "Поле даты заполненно не верно.\n";
                }
            }
            catch (Exception)
            {
                error += "Строка даты не соответствует формату";
            }
            if ((NameDescription != null) && (Description != null))
            {
                foreach (var NameDes in NameDescription)
                {
                    if ((NameDes == null) || (NameDes == ""))
                    {
                        error += "Наименование описания должны быть заполены";
                    }
                }
                foreach (var Des in Description)
                {
                    if ((Des == null) || (Des == ""))
                    {
                        error += "Описания должны быть заполены";
                    }
                }
            }
            if (db.Events.Where(elem => elem.Id == id).Count() == 0)
            {
                return HttpNotFound();
            }
            if (error == "")
            {
                Event editEvent = db.Events.Include(p => p.EventDescriptions).Where(elem => elem.Id == id).First();
                editEvent.Name = name;
                editEvent.Date = Convert.ToDateTime(date);
                editEvent.MaxUsers = MaxUsers;
                List<EventDescription> listDesc = editEvent.EventDescriptions.ToList();
                foreach(EventDescription eventDesc in listDesc)
                {
                    db.Entry(eventDesc).State = EntityState.Deleted;
                    editEvent.EventDescriptions.Remove(eventDesc);
                }
                if ((NameDescription != null) && (Description != null))
                {
                    for (int i = 0; i < NameDescription.Length; i++)
                    {
                        editEvent.EventDescriptions.Add(new EventDescription { Name = NameDescription[i], Description = Description[i] });
                    }
                }
                db.Entry(editEvent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home", new { registerEvent = "Событие было успешно отредактированно." });
            }
            else
            {
                return RedirectToAction("Index", "Home", new { registerEvent = error });
            }
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteUser(int id)
        {
            EventPlanning.Models.User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            List<Event> events = db.Events.Include(p => p.Users).ToList();
            events = events.Where(elem => elem.Users.Contains(user)).ToList();
            Event useEvent = events.FirstOrDefault();
            if (useEvent == null)
            {
                return HttpNotFound();
            }
            useEvent.Users.Remove(user);
            db.Entry(user).State = EntityState.Deleted;
            db.Entry(useEvent).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AboutEvent", "Home", new { id = useEvent.Id });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteUserWithBlackList(int id)
        {
            EventPlanning.Models.User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            List<Event> events = db.Events.Include(p => p.BlackListUsers).ToList();
            events = events.Where(elem => elem.BlackListUsers.Contains(user)).ToList();
            Event useEvent = events.FirstOrDefault();
            if (useEvent == null)
            {
                return HttpNotFound();
            }
            useEvent.BlackListUsers.Remove(user);
            db.Entry(user).State = EntityState.Deleted;
            db.Entry(useEvent).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AboutEvent", "Home", new { id = useEvent.Id });
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public ActionResult AddInBlackListUser(int id)
        {
            EventPlanning.Models.User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            List<Event> events = db.Events.Include(p => p.Users).ToList();
            events = events.Where(elem => elem.Users.Contains(user)).ToList();
            Event useEvent = events.FirstOrDefault();
            if (useEvent == null)
            {
                return HttpNotFound();
            }
            useEvent.BlackListUsers.Add(user);
            useEvent.Users.Remove(user);
            db.Entry(useEvent).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("AboutEvent", "Home", new { id = useEvent.Id });
        }

        public ActionResult Contact()
        {
            ViewData["admin"] = (IsAdmin(User.Identity.Name)) ? "true" : "false";
            return View();
        }

        private bool IsAdmin(string userName)
        {
            ApplicationUserManager userManager = HttpContext.GetOwinContext()
            .GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(userName);
            if (user != null)
            {
                IList<string> roles = userManager.GetRoles(user.Id);
                if (roles.Contains("admin"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}