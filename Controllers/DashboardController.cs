using EventEase_Management.Entity;
using EventEase_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase_Management.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // Action method for the dashboard view
        public IActionResult Dashboard()
        {
            try
            {
                // Get the total bookings
                int totalBookings = _context.BookingManager.Count();
                int activeEvents = _context.EventManager.Where(e => e.Date >= DateTime.Now).Count();

                // Get the venue IDs associated with either bookings or events
                var venueIdsWithBookingsOrEvents = _context.BookingManager
                    .Select(b => b.VenueBooking)
                    .Union(_context.EventManager.Select(e => e.VenueID))
                    .Distinct()
                    .ToList();

                int venuesWithBookingsOrEvents = venueIdsWithBookingsOrEvents.Count();

                // Get recent bookings (last 5) with venue included
                var recentBookings = _context.BookingManager
                    .Include(b => b.Venuess)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToList();

                // Get upcoming events (next 5) with venue included
                var upcomingEvents = _context.EventManager
                    .Where(e => e.Date >= DateTime.Now)
                    .Include(e => e.Venue)
                    .OrderBy(e => e.Date)
                    .Take(5)
                    .ToList();

                // Create a DashboardModelView object to pass to the view
                var dashboardModel = new DashboardModelView
                {
                    TotalBookings = totalBookings,
                    ActiveEvents = activeEvents,
                    AvailableVenues = venuesWithBookingsOrEvents,
                    RecentBookings = recentBookings,
                    UpcomingEvents = upcomingEvents
                };

                // Pass the model to the view
                return View(dashboardModel);
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database errors
                ViewBag.ErrorMessage = "A database error occurred while fetching data. Please try again later.";
                Console.WriteLine($"Database error: {dbEx.Message}");
                return View("Error");
            }
            catch (Exception ex)
            {
                // Handle general errors
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                Console.WriteLine($"General error: {ex.Message}");
                return View("Error");
            }
        }
    }
}
