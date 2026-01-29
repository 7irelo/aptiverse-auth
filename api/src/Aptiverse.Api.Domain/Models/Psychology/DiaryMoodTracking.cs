using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Psychology
{
    public class DiaryMoodTracking
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public DateTime TrackingDate { get; set; }
        public string OverallMood { get; set; }
        public int EnergyLevel { get; set; } // 1-10 scale
        public int StressLevel { get; set; } // 1-10 scale
        public int MotivationLevel { get; set; } // 1-10 scale
        public string FactorsAffectingMood { get; set; } // JSON array
        public string Notes { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
    }
}
