using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventPlanning.Models
{
    public class Setting
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Email адресс используемый SMTP клиентом: ")]
        public string SendEmail { get; set; }

        [Required]
        [Display(Name = "Пароль: ")]
        public string SendPassword { get; set; }

        [Required]
        [Display(Name = "За сколько часов оповещать о событиях: ")]
        public int AutocastingHours { get; set; }

        [Required]
        [Display(Name = "Через сколько часов удалять событие?: ")]
        public int HoursAtRemove { get; set; }

        [HiddenInput(DisplayValue = false)]
        public ICollection<UserEmail> EmailsInBlackList { get; set; }

        public Setting()
        {
            EmailsInBlackList = new List<UserEmail>();
        }
    }

    public class UserEmail
    {
        public int Id { get; set; }
        public string Email { get; set; }
        
        public int? SettingId { get; set; }
        public Setting Setting { get; set; }
    }
}