using Aptiverse.Api.Domain.Models.Goals;

namespace Aptiverse.Api.Domain.Models.Psychology
{
    public class DiaryGoal
    {
        public long Id { get; set; }
        public long DiaryEntryId { get; set; }
        public long GoalId { get; set; }
        public string ConnectionType { get; set; } // "Reflection", "ProgressUpdate", "Setback", "Achievement"
        public string Notes { get; set; }

        // Navigation properties
        public virtual DiaryEntry DiaryEntry { get; set; }
        public virtual Goal Goal { get; set; }
    }
}
