using EventEase_Management.Entity;
using EventEase_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EventEase_Management.Controllers
{
    public class BookingController(AppDbContext Bcontext) : Controller
    {
        public IActionResult BookDetails(string searchTerm, string statusFilter)
        {
            try
            {
                var bookings = Bcontext.BookingManager
                    .Include(b => b.Eventss)
                    .Include(b => b.Venuess)
                    .AsQueryable();


                if (!string.IsNullOrEmpty(searchTerm))
                {
                    StringComparison comparison = StringComparison.OrdinalIgnoreCase;

                    bookings = bookings.Where(b =>
                        b.CustomerName.Contains(searchTerm, comparison) == true ||
                        b.Eventss.Name.Contains(searchTerm, comparison) == true ||
                        b.Status.Contains(searchTerm, comparison) == true
                    );


                }

                // Apply status filter
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    bookings = bookings.Where(b => b.Status == statusFilter);
                }

                ViewBag.StatusFilter = new List<SelectListItem>
                {
                    new() { Value = "", Text = "All" },
                    new() { Value = "Pending", Text = "Pending" },
                    new() { Value = "Confirmed", Text = "Confirmed" },
                    new() { Value = "Cancelled", Text = "Cancelled" }
                };

                ViewBag.EventId = new SelectList(Bcontext.EventManager, "EventID", "Name");
                ViewBag.VenueId = new SelectList(Bcontext.VenueManager, "VenueID", "Name");

                return View(bookings.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while fetching bookings: " + ex.Message;
                return View(new List<Booking>());
            }
        }
        //GET Book
        public IActionResult Book()
        {
            try
            {
                ViewBag.EventId = new SelectList(Bcontext.EventManager, "EventID", "Name");
                ViewBag.VenueId = new SelectList(Bcontext.VenueManager, "VenueID", "Name");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the booking form: " + ex.Message;
                return RedirectToAction("BookDetails");
            }
        }

        // Add a new booking (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBooking(BookingModelView model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var booking = new Booking
                    {
                        CustomerName = model.CustomerName,
                        EventBooking = model.EventID,
                        VenueBooking = model.VenueId,
                        BookingDate = model.BookingDate,
                        Status = model.Status
                    };

                    Bcontext.BookingManager.Add(booking);
                    Bcontext.SaveChanges();
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine("Debug88888888888777777777777666666666666output test", error.ErrorMessage); // or use a logger
                    }
                    TempData["SuccessMessage"] = "Booking created successfully!";
                    return RedirectToAction("BookDetails");
                }

                // Repopulate dropdowns if model state is invalid
                ViewBag.EventId = new SelectList(Bcontext.EventManager, "EventID", "Name", model.EventID);
                ViewBag.VenueId = new SelectList(Bcontext.VenueManager, "VenueID", "Name", model.VenueId);
                return View("Book", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the booking: " + ex.Message;
                return RedirectToAction("BookDetails");
            }
        }

        // Edit booking view (GET)
        public IActionResult Edit(int id)
        {
            try
            {
                var booking = Bcontext.BookingManager
                    .Include(b => b.Eventss)
                    .Include(b => b.Venuess)
                    .FirstOrDefault(b => b.BookingID == id);

                if (booking == null)
                {
                    return NotFound();
                }

                var model = new BookingModelView
                {
                    BookingID = booking.BookingID,
                    CustomerName = booking.CustomerName,
                    EventID = booking.EventBooking,
                    VenueId = booking.VenueBooking,
                    BookingDate = booking.BookingDate,
                    Status = booking.Status
                };

                ViewBag.EventId = new SelectList(Bcontext.EventManager, "EventID", "Name", model.EventID);
                ViewBag.VenueId = new SelectList(Bcontext.VenueManager, "VenueID", "Name", model.VenueId);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the edit page: " + ex.Message;
                return RedirectToAction("BookDetails");
            }
        }

        // Edit booking (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookingModelView model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var booking = Bcontext.BookingManager.FirstOrDefault(b => b.BookingID == model.BookingID);

                    if (booking == null)
                    {
                        return NotFound();
                    }

                    booking.CustomerName = model.CustomerName;
                    booking.EventBooking = model.EventID;
                    //booking.VenueBooking = model.BookingID;
                    booking.VenueBooking = model.VenueId;
                    booking.BookingDate = model.BookingDate;
                    booking.Status = model.Status;

                    Bcontext.SaveChanges();
                    TempData["SuccessMessage"] = "Booking updated successfully!";
                    return RedirectToAction("BookDetails");
                }

                ViewBag.EventId = new SelectList(Bcontext.EventManager, "EventID", "Name", model.EventID);
                ViewBag.VenueId = new SelectList(Bcontext.VenueManager, "VenueID", "Name", model.VenueId);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating the booking: " + ex.Message;
                return RedirectToAction("BookDetails");
            }
        }
        public IActionResult Delete()
        {
            return View();
        }
        //GET: Delete booking
        public IActionResult Delete(int id)
        {
            try
            {
                var booking = Bcontext.BookingManager.Find(id);
                if (booking == null)
                {
                    return NotFound();
                }

                Bcontext.BookingManager.Remove(booking);
                Bcontext.SaveChanges();
                TempData["SuccessMessage"] = "Booking deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the booking: " + ex.Message;
            }

            return RedirectToAction("BookDetails");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
