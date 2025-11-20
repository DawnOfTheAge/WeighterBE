using System.ComponentModel.DataAnnotations;

namespace WeighterBE.Models
{
    public class User
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(50)]
        public string Role { get; set; } = "User"; // Default role

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation property for user's weight records
        public ICollection<WeightRecord>? WeightRecords { get; set; }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"User Id = {Id}\nUsername = {Username}\nEmail = {Email}\nCreated At = {CreatedAt}\nLast Login At = {LastLoginAt}";
        }

        #endregion
    }
}
