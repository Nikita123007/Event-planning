using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventPlanning.Models
{
    public class Event
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование события:")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Дата события:")]
        public DateTime Date { get; set; }

        [HiddenInput(DisplayValue = false)]
        public ICollection<User> Users { get; set; }

        [Required]
        [Display(Name = "Максимально кол-во подписчиков:")]
        public int MaxUsers { get; set; }

        [HiddenInput(DisplayValue = false)]
        public ICollection<User> BlackListUsers { get; set; }

        [HiddenInput(DisplayValue = false)]
        public ICollection<EventDescription> EventDescriptions { get; set; }

        public Event()
        {
            Users = new List<User>();
            EventDescriptions = new List<EventDescription>();
            BlackListUsers = new List<User>();
            MaxUsers = 99999;
        }
    }
}