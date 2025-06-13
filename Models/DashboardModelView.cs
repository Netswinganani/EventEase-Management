using EventEase_Management.Entity;

namespace EventEase_Management.Models
{
    public class DashboardModelView
    {

        public int TotalBookings { get; set; }
        public int ActiveEvents { get; set; }
        public int AvailableVenues { get; set; }


        public List<Event> UpcomingEvents { get; set; } = new List<Event>();
        public List<Booking> RecentBookings { get; set; } = new List<Booking>();
    }
}
