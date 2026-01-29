namespace Aptiverse.Api.Application.DiaryEntries.Dtos
{
    public record UpdateDiaryEntryDto
    {
        public string Title { get; init; }
        public string Content { get; init; }
        public string Mood { get; init; }
        public int MoodIntensity { get; init; }
        public string EntryType { get; init; }
        public string Tags { get; init; }
        public bool IsPrivate { get; init; }
        public DateTime EntryDate { get; init; }
        public string SentimentAnalysis { get; init; }
        public double SentimentScore { get; init; }
        public string KeyThemes { get; init; }
        public string AiInsights { get; init; }
        public bool NeedsFollowUp { get; init; }
        public string FollowUpAction { get; init; }
    }
}