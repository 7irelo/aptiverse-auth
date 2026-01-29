using Aptiverse.Api.Domain.Models.Rewards;
using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Goals
{
    public class Goal
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GoalType { get; set; } // "Academic", "Study", "Personal", "Skill"
        public string SubjectId { get; set; } // Optional: for academic goals
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string Unit { get; set; } // "Percentage", "Hours", "Points", "Assignments"
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public string Status { get; set; } // "NotStarted", "InProgress", "Completed", "Failed"
        public int Priority { get; set; } // 1-5 scale
        public int DifficultyWeight { get; set; } // 1-100 scale
        public decimal ProgressPercentage => TargetValue > 0 ? (CurrentValue / TargetValue) * 100 : 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }
        public virtual ICollection<GoalMilestone> Milestones { get; set; }
        public virtual ICollection<Reward> PotentialRewards { get; set; }
    }
}
