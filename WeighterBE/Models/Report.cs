using System.ComponentModel.DataAnnotations;

namespace WeighterBE.Models
{
    public class Report
    {
        #region Properties

        [Key]
        public int Id { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #endregion
    }
}
