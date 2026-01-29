namespace Aptiverse.Api.Domain.Models.Students
{
    public class StudentSubjectAnalytics
    {
        public long Id { get; set; }
        public long StudentSubjectId { get; set; }

        // Time of Day
        public int MorningPercentage { get; set; }
        public int AfternoonPercentage { get; set; }
        public int EveningPercentage { get; set; }

        // Study Patterns
        public int Consistency { get; set; }
        public string PreferredDays { get; set; } // JSON array: ["Monday", "Wednesday", "Friday"]
        public int SessionLength { get; set; }

        // Attendance
        public int ClassesAttended { get; set; }
        public int TotalClasses { get; set; }
        public double AttendanceRate { get; set; }

        // Learning Resources
        public int TextbookUsage { get; set; }
        public int VideoTutorials { get; set; }
        public int PracticeProblems { get; set; }
        public int GroupStudy { get; set; }
        public int OnlinePlatforms { get; set; }

        // Engagement Metrics
        public int QuestionsAsked { get; set; }
        public int ParticipationRate { get; set; }
        public int ResourceDownloads { get; set; }
        public int ForumActivity { get; set; }

        // External Factors
        public double WorkloadThisWeek { get; set; }
        public double StressLevel { get; set; }
        public double SleepQuality { get; set; }
        public double MotivationLevel { get; set; }

        // Career Relevance
        public int Importance { get; set; }
        public double InterestLevel { get; set; }
        public string Alignment { get; set; }

        public virtual StudentSubject StudentSubject { get; set; }
    }
}
