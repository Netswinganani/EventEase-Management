using EventEase_Management.Entity;

namespace EventEase_Management.Models
{
    public class EventListModelView
    {

        public List<Event> UpcomingEvents { get; set; } = new List<Event>();
        public List<Event> PastEvents { get; set; } = new List<Event>();

    }
}
