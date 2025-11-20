using System.ComponentModel.DataAnnotations;

namespace WeighterBE.Data.DTOs
{
    public class CreateReportDto
    {
        [MaxLength(1000)]
        public string? Description { get; set; }
    }

    public class UpdateReportDto
    {
        [MaxLength(1000)]
        public string? Description { get; set; }
    }

    public class ReportResponseDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
    }
}