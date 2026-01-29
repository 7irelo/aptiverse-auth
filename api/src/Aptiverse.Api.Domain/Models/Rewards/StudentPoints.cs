using Aptiverse.Api.Domain.Models.Students;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aptiverse.Api.Domain.Models.Rewards
{
    public class StudentPoints
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public int TotalPoints { get; set; }
        public int AvailablePoints { get; set; }
        public int UsedPoints { get; set; }
        public int Level { get; set; } = 1;
        public string CurrentRank { get; set; } = "Beginner";
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual ICollection<PointsTransaction> Transactions { get; set; }
    }
}
