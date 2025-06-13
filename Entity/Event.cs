using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEase_Management.Entity
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Event type is required")]
        [MaxLength(30)]
        public int EventTypeID { get; set; }

        [ForeignKey("EventTypeID")]
        public EventType? EventType { get; set; }

        [Required(ErrorMessage = "Event Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; } // Scheduled, Completed, Cancelled

        [Required(ErrorMessage = "Venue is required")]
        public int VenueID { get; set; }

        [ForeignKey("VenueID")]
        public Venue? Venue { get; set; }


        public string? ImageUrl { get; set; }
       // public ICollection<Event>? Events { get; set; }
    }
    public class EventType
    {
        [Key]
        public int EventTypeID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Event>? Events { get; set; }
    }

}
