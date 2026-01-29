using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Rewards
{
    public class GrowthTracking
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public DateTime TrackingDate { get; set; }
        public decimal AcademicGrowth { get; set; } // 0-100 scale
        public decimal StudyHabitGrowth { get; set; } // 0-100 scale
        public decimal EmotionalGrowth { get; set; } // 0-100 scale
        public decimal OverallGrowth { get; set; } // 0-100 scale
        public string GrowthFactors { get; set; } // JSON array of contributing factors
        public string AreasForImprovement { get; set; } // JSON array

        // Navigation properties
        public virtual Student Student { get; set; }
    }
}
