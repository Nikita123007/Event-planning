using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EventPlanning.Models
{
    public class SettingDbInitializer : DropCreateDatabaseAlways<SettingContext>
    {
        protected override void Seed(SettingContext db)
        {
            db.Settings.Add(new Setting { SendEmail = "nikitos.zabeyda@gmail.com", SendPassword = "nikita123007", AutocastingHours = 24, HoursAtRemove = 1});
            base.Seed(db);
        }
    }
}