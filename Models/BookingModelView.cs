using EventEase_Management.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEase_Management.Models
{
    public class BookingModelView
    {
        [Key]
        public int BookingID { get; set; }

        [Required, StringLength(100)]
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Event is required")]
        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public Event? Events { get; set; }

        [Required(ErrorMessage = "Venue is required")]
        public int VenueId { get; set; }
        [ForeignKey("VenueId")]
        public Venue? Venue { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public string? Status { get; set; }
    }
}
