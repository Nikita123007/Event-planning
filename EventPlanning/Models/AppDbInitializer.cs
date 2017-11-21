using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using EventPlanning.Controllers;

using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections;
using Microsoft.Owin.Security;

namespace EventPlanning.Models
{
    public class AppDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // создаем две роли
            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "user" };

            // добавляем роли в бд
            roleManager.Create(role1);
            roleManager.Create(role2);
            
            // создаем пользователей
            var admin = new ApplicationUser { Email = "adminmail@mail.ru", UserName = "adminmail@mail.ru" };
            string passwordAdmin = "123_abc_ABC";
            var resultAdmin = userManager.Create(admin, passwordAdmin);

            var user = new ApplicationUser { Email = "usermail@mail.ru", UserName = "usermail@mail.ru" };
            string passwordUser = "123_abc_ABC";
            var resultUser = userManager.Create(user, passwordUser);

            // если создание пользователя прошло успешно
            if (resultAdmin.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
                admin.EmailConfirmed = true;
            }

            if (resultUser.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(user.Id, role2.Name);
                user.EmailConfirmed = true;
            }

            base.Seed(context);
        }
    }
}