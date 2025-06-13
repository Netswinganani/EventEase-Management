using System.Diagnostics;
using EventEase_Management.Entity;
using EventEase_Management.Models;
using EventEase_Management.Service;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ImageService _imageService;

        public HomeController(AppDbContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        public IActionResult Dashboard()
        {
            try
            {
                var totalBookings = _context.BookingManager.Count();

                var activeEvents = _context.EventManager
                    .Count(e => e.Date >= DateTime.Now);

                var venueIdsWithBookingsOrEvents = _context.VenueManager
                    .Select(v => v.VenueID)
                    .Distinct()
                    .ToList();

                var venuesWithBookingsOrEvents = venueIdsWithBookingsOrEvents.Count;

                var recentBookings = _context.BookingManager
                    .Include(b => b.Venuess)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToList();

                var upcomingEvents = _context.EventManager
                    .Where(e => e.Date >= DateTime.Now)
                    .Include(e => e.Venue)
                    .OrderBy(e => e.Date)
                    .Take(5)
                    .ToList();

                var dashboardModel = new DashboardModelView
                {
                    TotalBookings = totalBookings,
                    ActiveEvents = activeEvents,
                    AvailableVenues = venuesWithBookingsOrEvents,
                    RecentBookings = recentBookings,
                    UpcomingEvents = upcomingEvents
                };

                return View(dashboardModel);
            }
            catch (DbUpdateException dbEx)
            {
                ViewBag.ErrorMessage = "A database error occurred while fetching data. Please try again later.";
                Console.WriteLine($"Database error: {dbEx.Message}");
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                Console.WriteLine($"General error: {ex.Message}");
                return RedirectToAction("Error");
            }
        }


        public IActionResult Search(string searchTerm, string statusFilter, string locationFilter)
        {
            try
            {
                // Start querying the venues
                var venuesQuery = _context.VenueManager.AsQueryable();

                // Filter by search term (name, description, or location)
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    venuesQuery = venuesQuery.Where(v =>
                        v.Name.Contains(searchTerm) ||
                        v.Description.Contains(searchTerm) ||
                        v.Location.Contains(searchTerm));
                }

                // Filter by status (only if statusFilter is not null or empty)
                if (!string.IsNullOrWhiteSpace(statusFilter))
                {
                    venuesQuery = venuesQuery.Where(v => v.Status == statusFilter);
                }

                // Filter by location (only if locationFilter is not null or empty)
                if (!string.IsNullOrWhiteSpace(locationFilter))
                {
                    venuesQuery = venuesQuery.Where(v => v.Location.Contains(locationFilter));
                }

                // Get the filtered list of venues
                var venues = venuesQuery.ToList();

                // Generate SAS URLs for images in the search results
                foreach (var venue in venues)
                {
                    if (!string.IsNullOrEmpty(venue.ImageUrl))
                    {
                        venue.ImageUrl = _imageService.GenerateBlobSasUrl(
                            venue.ImageUrl,
                            TimeSpan.FromHours(1) // Set the SAS URL to expire in 1 hour
                        );
                    }
                }

                // Prepare status options for the dropdown (ensure a default is selected)
                ViewBag.StatusOptions = new List<SelectListItem>
        {
            new SelectListItem { Text = "All", Value = "" },  // Option to show all statuses
            new SelectListItem { Text = "Available", Value = "Available" },
            new SelectListItem { Text = "Booked", Value = "Booked" },
            new SelectListItem { Text = "Maintenance", Value = "Maintenance" }
        };

                // Prepare location options for the dropdown (distinct locations)
                ViewBag.LocationOptions = _context.VenueManager
                    .Select(v => v.Location)
                    .Distinct()
                    .Select(loc => new SelectListItem { Text = loc, Value = loc })
                    .ToList();

                // Pass the filtered venues to the view
                return View(venues);
            }
            catch (Exception ex)
            {
                // If there is an error, display the message and return an empty list
                ViewBag.ErrorMessage = "An error occurred while fetching venues.";
                Console.WriteLine(ex.Message);  // Log error (use proper logging mechanism)
                return View(new List<Venue>());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            if (exceptionFeature?.Error is Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;

                if (HttpContext.RequestServices
                    .GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    ViewData["ExceptionDetails"] = ex.ToString();
                }
            }

            return View(model);
        }
    }
}
