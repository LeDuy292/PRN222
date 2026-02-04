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
        public int TripID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled

        // Navigation properties
        [ForeignKey("TripID")]
        public virtual Trip Trip { get; set; } = null!;

        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; } = null!;
    }
}
