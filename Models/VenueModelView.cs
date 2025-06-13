using EventEase_Management.Entity;
using System.ComponentModel.DataAnnotations;

namespace EventEase_Management.Models
{
    public class VenueModelView
    {

        [Key]
  
        public int VenueID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [MaxLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 10000, ErrorMessage = "Capacity must be between 1 and 10,000")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("Available|Booked|Maintenance",
            ErrorMessage = "Status must be 'Available', 'Booked', or 'Maintenance'")]
        public string Status { get; set; } = "Available";

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }

        // Navigation properties (if you have relationships)
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
 