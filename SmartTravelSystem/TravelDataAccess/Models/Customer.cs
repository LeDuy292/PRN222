using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelDataAccess.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key]
        [Column("CustomerID")]
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Email { get; set; }

        public int? Age { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [StringLength(20)]
        public string Role { get; set; } = "User"; // User, Admin

        // Navigation property
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
