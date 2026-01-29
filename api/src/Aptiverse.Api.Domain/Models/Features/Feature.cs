using System;
using System.Collections.Generic;
using System.Text;

namespace Aptiverse.Api.Domain.Models.Features
{
    public class Feature
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; } // Unique identifier e.g., "AI_TUTOR", "ADVANCED_ANALYTICS"
        public string Description { get; set; }
        public string Category { get; set; } // "Study", "Analytics", "AI", "Gamification", "Social"
        public decimal BasePrice { get; set; }
        public string PriceCurrency { get; set; } = "ZAR";
        public string BillingCycle { get; set; } // "OneTime", "Monthly", "Annual"
        public int ComplexityWeight { get; set; } // 1-100 scale
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<RoleFeature> RoleFeatures { get; set; }
        public virtual ICollection<UserFeature> UserFeatures { get; set; }
        public virtual ICollection<RewardFeature> RewardFeatures { get; set; }
    }
}
