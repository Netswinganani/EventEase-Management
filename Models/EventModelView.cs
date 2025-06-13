using EventEase_Management.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEase_Management.Models
{
    public class EventModelView
    {
        [Key]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Event type is required")]
        public int EventTypeID { get; set; }

        [ForeignKey("EventTypeID")]
        public EventType? EventType { get; set; }


        [Required(ErrorMessage = "Event Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; } // Scheduled, Completed, Cancelled

        public int VenueId { get; set; }
        [ForeignKey("VenueId")]
        public Venue? Venues { get; set; }

        public string? ImageUrl { get; set; }
    }
}
