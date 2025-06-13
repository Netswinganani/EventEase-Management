using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEase_Management.Entity
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [Required, StringLength(100)]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Event is required")]
        public int EventBooking { get; set; }
        [ForeignKey("EventID")]
        public Event? Eventss { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public int VenueBooking { get; set; }
        [ForeignKey("VenueId")]
        public Event? Venuess { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public string? Status { get; set; }
    }
}
