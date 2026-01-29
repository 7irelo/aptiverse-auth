using System;
using System.Collections.Generic;
using System.Text;

namespace Aptiverse.Api.Domain.Models.Students
{
    public class StudySession
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public string SubjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public string SessionType { get; set; } // "reading", "practice", "revision", "homework"
        public string TopicsCovered { get; set; } // JSON array or comma-separated
        public double EfficiencyScore { get; set; }
        public int ConcentrationLevel { get; set; } // 1-10 scale
        public string Notes { get; set; }
        public string ResourcesUsed { get; set; } // JSON array

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
