using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EventPlanning.Models
{
    public class EventDbInitializer : DropCreateDatabaseAlways<EventContext>
    {
        protected override void Seed(EventContext db)
        {
            Event event1 = new Event { Name = "Событие1", Date = (DateTime.Now).AddDays(-1) };
            Event event2 = new Event { Name = "Событие2", Date = (DateTime.Now).AddDays(2) };
            Event event3 = new Event { Name = "Событие1", Date = (DateTime.Now).AddDays(1) };
            Event event4 = new Event { Name = "Событие2", Date = (DateTime.Now).AddDays(-2) };
            Event event5 = new Event { Name = "Событие1", Date = (DateTime.Now).AddDays(1) };
            Event event6 = new Event { Name = "Событие2", Date = (DateTime.Now).AddDays(2) };
            Event event7 = new Event { Name = "Событие1", Date = (DateTime.Now).AddDays(-1) };
            Event event8 = new Event { Name = "Событие2", Date = (DateTime.Now).AddDays(2) };
            Event event9 = new Event { Name = "Событие1", Date = (DateTime.Now).AddDays(1) };
            Event event10 = new Event { Name = "Событие1", Date = (DateTime.Now).AddDays(-1) };
            Event event11 = new Event { Name = "Событие2", Date = (DateTime.Now).AddDays(2) };
            Event event12 = new Event { Name = "Событие2", Date = (DateTime.Now).AddDays(-2) };
            EventDescription evD1 = new EventDescription { Name = "Расположение", Description = "Минск", Event = event1 };

            event1.EventDescriptions.Add(evD1);

            db.Events.Add(event1);
            db.Events.Add(event2);
            db.Events.Add(event3);
            db.Events.Add(event4);
            db.Events.Add(event5);
            db.Events.Add(event6);
            db.Events.Add(event7);
            db.Events.Add(event8);
            db.Events.Add(event9);
            db.Events.Add(event10);
            db.Events.Add(event11);
            db.Events.Add(event12);

            base.Seed(db);
        }
    }
}