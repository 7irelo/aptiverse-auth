namespace Aptiverse.Api.Domain.Models.Tutors
{
    public class TutorAvailability
    {
        public long Id { get; set; }
        public long TutorId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;

        public virtual Tutor Tutor { get; set; }
    }
}
