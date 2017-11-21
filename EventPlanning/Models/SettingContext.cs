using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EventPlanning.Models
{
    public class SettingContext : DbContext
    {
        public DbSet<Setting> Settings { get; set; }
        public DbSet<UserRegisterData> Users { get; set; }
        public DbSet<UserEmail> UsersEmails { get; set; }
    }
}