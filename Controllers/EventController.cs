using EventEase_Management.Entity;
using EventEase_Management.Models;
using EventEase_Management.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase_Management.Controllers
{
    public class EventController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageService _imageService;

        public EventController(AppDbContext context, IWebHostEnvironment webHostEnvironment, ImageService imageService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            ViewBag.EventTypes = new SelectList(_context.EventTypes, "EventTypeID", "Name");
            ViewBag.VenueId = new SelectList(_context.VenueManager, "VenueID", "Name");
            ViewBag.Statuses = new SelectList(new List<SelectListItem>
    {
        new SelectListItem { Value = "Upcoming", Text = "Upcoming" },
        new SelectListItem { Value = "Ongoing", Text = "Ongoing" },
        new SelectListItem { Value = "Completed", Text = "Completed" }
    }, "Value", "Text");

            return View(new EventModelView());
        }

        // New GET action for showing add event form

        [HttpPost]
        public async Task<IActionResult> AddEvent(EventModelView model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                var eventModel = new Event
                {
                    Name = model.Name,
                    Date = model.Date,
                    Description = model.Description,
                    Status = model.Status,
                    VenueID = model.VenueId,
                    ImageUrl = model.ImageUrl,
                    EventTypeID = model.EventTypeID,
                };

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    try
                    {
                        string imageUrl = await _imageService.UploadImageToBlobStorage(ImageFile);
                        eventModel.ImageUrl = imageUrl;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Image upload failed: " + ex.Message);
                        return ReturnToIndexWithErrorModel();
                    }
                }

                try
                {
                    _context.EventManager.Add(eventModel);
                    await _context.SaveChangesAsync();
                    ModelState.Clear();
                    TempData["Message"] = $"Event '{eventModel.Name}' was successfully added!";
                    return RedirectToAction("ListEvent");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "We cannot process your event now, please try again later.");
                }
            }

            return ReturnToIndexWithErrorModel();

            // Helper method to avoid repeating the re-population logic
            IActionResult ReturnToIndexWithErrorModel()
            {
                // Repopulate dropdowns
                ViewBag.EventTypes = new SelectList(_context.EventTypes.ToList(), "EventTypeID", "Name");
                ViewBag.VenueId = new SelectList(_context.VenueManager.ToList(), "VenueID", "Name");
                ViewBag.Statuses = new SelectList(new List<SelectListItem>
           {
               new SelectListItem { Value = "Upcoming", Text = "Upcoming" },
               new SelectListItem { Value = "Ongoing", Text = "Ongoing" },
               new SelectListItem { Value = "Completed", Text = "Completed" }
           }, "Value", "Text");

                return View("Index", model); // 
            }

               }

        public IActionResult ListEvent(int? eventTypeId, DateTime? startDate, DateTime? endDate, string? venueStatus)
        {
            try
            {
                var eventsQuery = _context.EventManager
                    .Include(e => e.Venue)
                    .Include(e => e.EventType)
                    .AsQueryable();

                // Apply filters
                if (eventTypeId.HasValue)
                    eventsQuery = eventsQuery.Where(e => e.EventTypeID == eventTypeId.Value);

                if (startDate.HasValue)
                    eventsQuery = eventsQuery.Where(e => e.Date >= startDate.Value);

                if (endDate.HasValue)
                    eventsQuery = eventsQuery.Where(e => e.Date <= endDate.Value);

                if (!string.IsNullOrEmpty(venueStatus))
                    eventsQuery = eventsQuery.Where(e => e.Venue.Status == venueStatus);

                var allEvents = eventsQuery.ToList();

                // ✅ Apply image URL signing
                foreach (var ev in allEvents)
                {
                    if (!string.IsNullOrEmpty(ev.ImageUrl))
                    {
                        ev.ImageUrl = _imageService.GenerateBlobSasUrl(ev.ImageUrl, TimeSpan.FromHours(1));
                    }
                }

                // Split upcoming vs past
                var currentDate = DateTime.Now;
                var upcomingEvents = allEvents.Where(e => e.Date >= currentDate).ToList();
                var pastEvents = allEvents.Where(e => e.Date < currentDate).ToList();

                var eventViewModel = new EventListModelView
                {
                    UpcomingEvents = upcomingEvents,
                    PastEvents = pastEvents
                };

                // ✅ Populate filter dropdowns
                ViewBag.EventTypes = new SelectList(_context.EventTypes.ToList(), "EventTypeID", "Name", eventTypeId);
                ViewBag.VenueStatuses = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Value = "Available", Text = "Available" },
            new SelectListItem { Value = "Booked", Text = "Booked" },
            new SelectListItem { Value = "Unavailable", Text = "Unavailable" }
        }, "Value", "Text", venueStatus);

                return View(eventViewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching events: {ex.Message}");
                ViewBag.ErrorMessage = "Something went wrong while fetching the event data.";

                // Optional: repopulate dropdowns even on error
                ViewBag.EventTypes = new SelectList(_context.EventTypes.ToList(), "EventTypeID", "Name");
                ViewBag.VenueStatuses = new SelectList(new List<SelectListItem>
        {
            new SelectListItem { Value = "Available", Text = "Available" },
            new SelectListItem { Value = "Booked", Text = "Booked" },
            new SelectListItem { Value = "Unavailable", Text = "Unavailable" }
        }, "Value", "Text");

                return View(new EventListModelView());
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var e = _context.EventManager.Include(ev => ev.Venue).FirstOrDefault(ev => ev.EventID == id);
            if (e == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(e.ImageUrl))
            {
                e.ImageUrl = _imageService.GenerateBlobSasUrl(e.ImageUrl, TimeSpan.FromHours(1));
            }
            var eventTypes = _context.EventTypes.Select(et => new SelectListItem
            {
                Value = et.EventTypeID.ToString(),
                Text = et.Name
            }).ToList();

            var venues = _context.VenueManager.Select(v => new SelectListItem
            {
                Value = v.VenueID.ToString(),
                Text = v.Name
            }).ToList();

            ViewBag.EventTypes = eventTypes;
            ViewBag.VenueId = venues;
            return View(e);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Event eventModel, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    try
                    {
                        string imageUrl = await _imageService.UploadImageToBlobStorage(ImageFile);
                        eventModel.ImageUrl = imageUrl;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Image upload failed: " + ex.Message);
                        return View(eventModel);
                    }
                }

                try
                {
                    _context.Update(eventModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ListEvent");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "We cannot process your event now, please try again later.");
                }
            }
            ViewBag.VenueId = _context.VenueManager.Select(v => new SelectListItem
            {
                Value = v.VenueID.ToString(),
                Text = v.Name
            }).ToList();

            ViewBag.Statuses = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Scheduled", Text = "Scheduled" },
                    new SelectListItem { Value = "Completed", Text = "Completed" },
                    new SelectListItem { Value = "Cancelled", Text = "Cancelled" }
                };

            ViewBag.EventTypes = _context.EventTypes.Select(et => new SelectListItem
            {
                Value = et.EventTypeID.ToString(),
                Text = et.Name
            }).ToList();

            return View(eventModel);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var l = _context.EventManager.Include(e => e.Venue).FirstOrDefault(v => v.EventID == id);
            if (l == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(l.ImageUrl))
            {
                l.ImageUrl = _imageService.GenerateBlobSasUrl(l.ImageUrl, TimeSpan.FromHours(1));
            }

            return View(l);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var k = _context.EventManager.Find(id);
            if (k == null)
            {
                return NotFound();
            }

            _context.EventManager.Remove(k);
            _context.SaveChanges();

            return RedirectToAction(nameof(ListEvent));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var o = await _context.EventManager.Include(e => e.Venue).FirstOrDefaultAsync(ev => ev.EventID == id);
            if (o == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(o.ImageUrl))
            {
                o.ImageUrl = _imageService.GenerateBlobSasUrl(o.ImageUrl, TimeSpan.FromHours(1));
            }

            return View(o);
        }
    }
}
