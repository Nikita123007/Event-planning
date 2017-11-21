using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventPlanning.Models
{
    public class User
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Имя пользователя:")]
        public string Name { get; set; }

        [HiddenInput(DisplayValue = false)]
        public bool IsNotifiedAboutEvent { get; set; }


        [HiddenInput(DisplayValue = false)]
        public int? EventId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public Event Event { get; set; }

        public User()
        {
            IsNotifiedAboutEvent = false;
        }
    }

    public class UserRegisterData
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Suname { get; set; }
        
        public string Age { get; set; }
        
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class EventAndUsersModel
    {
        public Event Event { get; set; }
        public ICollection<UserRegisterData> Users { get; set; }

        public EventAndUsersModel()
        {
            Users = new List<UserRegisterData>();
        }
    }

    public class SettingAndUsersModel
    {
        public Setting Setting { get; set; }
        public ICollection<UserRegisterData> Users { get; set; }

        public SettingAndUsersModel()
        {
            Users = new List<UserRegisterData>();
        }
    }
}