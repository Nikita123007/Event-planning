using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EventPlanning.Models;
using System.Data.Entity;
using System.Threading;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace EventPlanning
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<ApplicationDbContext>(new AppDbInitializer());
            Database.SetInitializer(new EventDbInitializer());
            Database.SetInitializer(new SettingDbInitializer());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Task autocasting = new Task(new Action(AutocastEvent));
            //utocasting.Start();
        }

        private async void AutocastEvent()
        {
            EventContext db = new EventContext();
            SettingContext dbSetting = new SettingContext();
            while (true)
            {
                Thread.Sleep(1000 * 60 * 10);
                List<Event> events = db.Events.Include(p => p.Users).Include(p => p.BlackListUsers).Include(p => p.EventDescriptions).ToList();
                foreach (Event useEvent in events)
                {
                    try
                    {
                        if ((DateTime.Now - useEvent.Date).TotalHours > dbSetting.Settings.First().HoursAtRemove)
                        {
                            db.Entry(useEvent).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                        else
                        {
                            if ((useEvent.Date - DateTime.Now).TotalHours < dbSetting.Settings.First().AutocastingHours)
                            {
                                foreach (User user in useEvent.Users)
                                {
                                    if (!user.IsNotifiedAboutEvent)
                                    {
                                        IdentityMessage message = new IdentityMessage();
                                        message.Destination = user.Name;
                                        message.Subject = "Cобытие: " + useEvent.Name;
                                        message.Body = "Приближается событие: " + useEvent.Name + ". Событие произойдёт: " + useEvent.Date + ".";
                                        await EventPlanning.Models.EmailService.SendAsync(message);
                                        user.IsNotifiedAboutEvent = true;
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception er)
                    {

                    }
                }
            }
        }
    }
}
