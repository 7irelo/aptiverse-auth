using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Psychology
{
    public class DiaryEntry
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Mood { get; set; } // "Happy", "Sad", "Anxious", "Excited", "Tired", "Motivated"
        public int MoodIntensity { get; set; } // 1-10 scale
        public string EntryType { get; set; } // "Academic", "Personal", "Reflection", "Goal"
        public string Tags { get; set; } // JSON array of tags
        public bool IsPrivate { get; set; } = true;
        public DateTime EntryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // AI Analysis Fields
        public string SentimentAnalysis { get; set; } // "Positive", "Negative", "Neutral"
        public double SentimentScore { get; set; }
        public string KeyThemes { get; set; } // JSON array of detected themes
        public string AiInsights { get; set; }
        public bool NeedsFollowUp { get; set; }
        public string FollowUpAction { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual ICollection<DiaryGoal> RelatedGoals { get; set; } // Fixed this line
    }
}
