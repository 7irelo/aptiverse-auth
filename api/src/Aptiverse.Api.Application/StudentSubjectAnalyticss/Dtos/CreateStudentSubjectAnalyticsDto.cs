namespace Aptiverse.Api.Application.StudentSubjectAnalyticss.Dtos
{
    public record CreateStudentSubjectAnalyticsDto
    {
        public long StudentSubjectId { get; init; }
        public int MorningPercentage { get; init; }
        public int AfternoonPercentage { get; init; }
        public int EveningPercentage { get; init; }
        public int Consistency { get; init; }
        public string PreferredDays { get; init; }
        public int SessionLength { get; init; }
        public int ClassesAttended { get; init; }
        public int TotalClasses { get; init; }
        public double AttendanceRate { get; init; }
        public int TextbookUsage { get; init; }
        public int VideoTutorials { get; init; }
        public int PracticeProblems { get; init; }
        public int GroupStudy { get; init; }
        public int OnlinePlatforms { get; init; }
        public int QuestionsAsked { get; init; }
        public int ParticipationRate { get; init; }
        public int ResourceDownloads { get; init; }
        public int ForumActivity { get; init; }
        public double WorkloadThisWeek { get; init; }
        public double StressLevel { get; init; }
        public double SleepQuality { get; init; }
        public double MotivationLevel { get; init; }
        public int Importance { get; init; }
        public double InterestLevel { get; init; }
        public string Alignment { get; init; }
    }
}