using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventPlanning.Models
{
    public class EventDescription
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование описания:")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание:")]
        public string Description { get; set; }


        [HiddenInput(DisplayValue = false)]
        public int? EventId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public Event Event { get; set; }
    }
}