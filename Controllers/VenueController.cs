using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using EventEase_Management.Entity;
using EventEase_Management.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase_Management.Service;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EventEase_Management.Controllers
{
    public class VenueController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ImageService _imageService;

        public VenueController(AppDbContext context, IWebHostEnvironment webHostEnvironment, ImageService imageService)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _imageService = imageService;
        }

        public IActionResult Venuepg()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVenue(VenueModelView model, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        try
                        {
                            if (ImageFile.Length > (50 * 1024 * 1024)) // 50 MB
                            {
                                ModelState.AddModelError("ImageFile", "Image file is too large. Max size is 50 MB.");
                                return View("Venuepg", model);
                            }

                            model.ImageUrl = await _imageService.UploadImageToBlobStorage(ImageFile);
                        }
                        catch (Azure.RequestFailedException ex)
                        {
                            Console.WriteLine($"Azure Request Failed - Code: {ex.ErrorCode}");
                            Console.WriteLine($"Message: {ex.Message}");
                            Console.WriteLine($"Status: {ex.Status}");
                            Console.WriteLine($"Details: {ex.StackTrace}");

                            ModelState.AddModelError("ImageFile", $"Azure upload failed: {ex.Message}");
                            return View("Venuepg", model);
                        }
                    }

                    var venueModel = new Venue
                    {
                        Name = model.Name,
                        Location = model.Location,
                        Description = model.Description,
                        Capacity = model.Capacity,
                        Status = model.Status,
                        ImageUrl = model.ImageUrl,
                    };

                    _context.VenueManager.Add(venueModel);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = $"Venue '{venueModel.Name}' was successfully added!";
                    return RedirectToAction("Venuepg");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("ImageFile problem", ex.Message);
                    return View("Venuepg", model);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unhandled error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                }
            }

            return View("Venuepg", model);
        }

        public IActionResult Venuebook()
        {
            try
            {
                var venues = _context.VenueManager.ToList();

                foreach (var venue in venues)
                {
                    if (!string.IsNullOrEmpty(venue.ImageUrl))
                    {
                        venue.ImageUrl = _imageService.GenerateBlobSasUrl(
                            venue.ImageUrl,
                            TimeSpan.FromHours(1)
                        );
                    }
                }

                if (venues == null || !venues.Any())
                {
                    ViewBag.Message = "No venues available.";
                }

                return View(venues);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching venues: {ex.Message}");
                ViewBag.ErrorMessage = "Something went wrong while fetching the venue data.";
                return View(new List<Venue>());
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            try
            {
                var v = await _context.VenueManager.FindAsync(id.Value);
                if (v == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(v.ImageUrl))
                {
                    v.ImageUrl = _imageService.GenerateBlobSasUrl(v.ImageUrl, TimeSpan.FromHours(1));
                }

                return View(v);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching venue details: {ex.Message}");
                ViewBag.ErrorMessage = "Something went wrong while fetching venue details.";
                return View();
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var venue = _context.VenueManager.Find(id);
                if (venue == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(venue.ImageUrl))
                {
                    venue.ImageUrl = _imageService.GenerateBlobSasUrl(venue.ImageUrl, TimeSpan.FromHours(1));
                }

                return View(venue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while editing venue: {ex.Message}");
                ViewBag.ErrorMessage = "Something went wrong while editing the venue.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VenueModelView venuemodel, IFormFile ImageFile)
        {
            if (id != venuemodel.VenueID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var venue = await _context.VenueManager.FindAsync(id);
                    if (venue != null)
                    {
                        if (ImageFile != null && ImageFile.Length > 0)
                        {
                            venue.ImageUrl = await _imageService.UploadImageToBlobStorage(ImageFile);
                        }

                        venue.Name = venuemodel.Name;
                        venue.Description = venuemodel.Description;
                        venue.Location = venuemodel.Location;
                        venue.Capacity = venuemodel.Capacity;
                        venue.Status = venuemodel.Status;

                        _context.VenueManager.Update(venue);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Venuebook));
                    }
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("ImageFile", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                }
            }

            return View(venuemodel);
        }
        

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var venue = await _context.VenueManager.FirstOrDefaultAsync(v => v.VenueID == id.Value);
            if (venue == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(venue.ImageUrl))
            {
                venue.ImageUrl = _imageService.GenerateBlobSasUrl(venue.ImageUrl, TimeSpan.FromHours(1));
            }

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var venue = await _context.VenueManager
                    .Include(v => v.Events) // Load related events
                    .FirstOrDefaultAsync(v => v.VenueID == id);

                if (venue == null)
                {
                    return NotFound();
                }

                if (!string.IsNullOrEmpty(venue.ImageUrl))
                {
                    var imageName = venue.ImageUrl.Split('/').Last();
                    await _imageService.DeleteImageFromBlobStorage(imageName);
                }

                _context.VenueManager.Remove(venue);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Venuebook));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while deleting venue: {ex.Message}");
                ViewBag.ErrorMessage = "An unexpected error occurred while deleting the venue.";
                return RedirectToAction(nameof(Venuebook));
            }
        }



    }
}
