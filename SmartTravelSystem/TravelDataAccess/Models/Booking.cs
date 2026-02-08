using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelDataAccess.Models
{
    [Table("Booking")]
    public class Booking
    {
        [Key]
        [Column("BookingID")]
        public int ID { get; set; }

        [Required]
        [Column("TripID")]
        public int TripID { get; set; }

        [Required]
        [Column("CustomerID")]
        public int CustomerID { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        // Navigation properties
        [ForeignKey("TripID")]
        public virtual Trip? Trip { get; set; }

        [ForeignKey("CustomerID")]
        public virtual Customer? Customer { get; set; }
    }
}
