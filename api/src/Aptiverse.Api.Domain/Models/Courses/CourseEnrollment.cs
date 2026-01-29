using Aptiverse.Api.Domain.Models.Students;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aptiverse.Api.Domain.Models.Courses
{
    public class CourseEnrollment
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public long StudentId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentStatus { get; set; } // "Pending", "Completed", "Refunded"
        public decimal Progress { get; set; } // 0-100
        public DateTime? CompletedAt { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Student { get; set; }
    }
}
