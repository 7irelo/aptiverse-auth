using Aptiverse.Api.Domain.Models.Admins;
using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Models.Features;
using Aptiverse.Api.Domain.Models.Goals;
using Aptiverse.Api.Domain.Models.Parents;
using Aptiverse.Api.Domain.Models.Psychology;
using Aptiverse.Api.Domain.Models.Resources;
using Aptiverse.Api.Domain.Models.Rewards;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Models.Tutors;
using Microsoft.EntityFrameworkCore;

namespace Aptiverse.Api.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Tutor> Tutors { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<StudentSubjectAnalytics> StudentSubjectAnalytics { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<StudentSubjectTopic> StudentSubjectTopics { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<AssessmentBreakdown> AssessmentBreakdowns { get; set; }
        public DbSet<StudySession> StudySessions { get; set; }
        public DbSet<WeeklyStudyHour> WeeklyStudyHours { get; set; }
        public DbSet<ImprovementTip> ImprovementTips { get; set; }
        public DbSet<KnowledgeGap> KnowledgeGaps { get; set; }
        public DbSet<GradeDistribution> GradeDistributions { get; set; }
        public DbSet<PeerComparison> PeerComparisons { get; set; }
        public DbSet<PredictionMetrics> PredictionMetrics { get; set; }
        public DbSet<PrerequisiteMastery> PrerequisiteMasteries { get; set; }


        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseModule> CourseModules { get; set; }
        public DbSet<ModuleLesson> ModuleLessons { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceDownload> ResourceDownloads { get; set; }
        public DbSet<TutorAvailability> TutorAvailabilities { get; set; }

        public DbSet<Feature> Features { get; set; }
        public DbSet<RoleFeature> RoleFeatures { get; set; }
        public DbSet<UserFeature> UserFeatures { get; set; }
        public DbSet<FeaturePurchase> FeaturePurchases { get; set; }


        public DbSet<DiaryEntry> DiaryEntries { get; set; }
        public DbSet<DiaryMoodTracking> DiaryMoodTrackings { get; set; }
        public DbSet<DiaryGoal> DiaryGoals { get; set; }

        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalMilestone> GoalMilestones { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<RewardFeature> RewardFeatures { get; set; }
        public DbSet<StudentReward> StudentRewards { get; set; }
        public DbSet<StudentPoints> StudentPoints { get; set; }
        public DbSet<PointsTransaction> PointsTransactions { get; set; }
        public DbSet<GrowthTracking> GrowthTrackings { get; set; }

        public DbSet<TeacherSubject> TeacherSubjects { get; set; }
        public DbSet<TutorSubject> TutorSubjects { get; set; }
        public DbSet<TeacherStudent> TeacherStudents { get; set; }
        public DbSet<TutorStudent> TutorStudents { get; set; }
        public DbSet<ParentStudent> ParentStudents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureGhostUser(modelBuilder);
            ConfigureStudentSchema(modelBuilder);
            ConfigureAdminSchema(modelBuilder);
            ConfigureTeacherSchema(modelBuilder);
            ConfigureParentSchema(modelBuilder);
            ConfigureTutorSchema(modelBuilder);
            ConfigureFeatureSchema(modelBuilder);
            ConfigureGoalSchema(modelBuilder);
            ConfigureRewardSchema(modelBuilder);
            ConfigurePsychologySchema(modelBuilder);
            ConfigureCourseSchema(modelBuilder);
            ConfigureResourceSchema(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigureIndexes(modelBuilder);
            ConfigureManyToManyRelationships(modelBuilder);
        }

        private static void ConfigureGhostUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users", "Identity", t => t.ExcludeFromMigrations());
                entity.HasKey(u => u.Id);
            });
        }

        private static void ConfigureStudentSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students", "Students");
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey("UserId")
                      .HasPrincipalKey("Id")
                      .HasConstraintName("FK_Students_Users_UserId");
            });

            modelBuilder.Entity<Subject>(entity => entity.ToTable("Subjects", "Students"));
            modelBuilder.Entity<Topic>(entity => entity.ToTable("Topics", "Students"));
            modelBuilder.Entity<Assessment>(entity => entity.ToTable("Assessments", "Students"));
            modelBuilder.Entity<StudySession>(entity => entity.ToTable("StudySessions", "Students"));
            modelBuilder.Entity<StudentSubject>(entity => entity.ToTable("StudentSubjects", "Students"));
            modelBuilder.Entity<StudentSubjectAnalytics>(entity => entity.ToTable("StudentSubjectAnalytics", "Students"));
            modelBuilder.Entity<StudentSubjectTopic>(entity => entity.ToTable("StudentSubjectTopics", "Students"));
            modelBuilder.Entity<AssessmentBreakdown>(entity => entity.ToTable("AssessmentBreakdowns", "Students"));
            modelBuilder.Entity<WeeklyStudyHour>(entity => entity.ToTable("WeeklyStudyHours", "Students"));
            modelBuilder.Entity<ImprovementTip>(entity => entity.ToTable("ImprovementTips", "Students"));
            modelBuilder.Entity<KnowledgeGap>(entity => entity.ToTable("KnowledgeGaps", "Students"));
            modelBuilder.Entity<GradeDistribution>(entity => entity.ToTable("GradeDistributions", "Students"));
            modelBuilder.Entity<PeerComparison>(entity => entity.ToTable("PeerComparisons", "Students"));
            modelBuilder.Entity<PredictionMetrics>(entity => entity.ToTable("PredictionMetrics", "Students"));
            modelBuilder.Entity<PrerequisiteMastery>(entity => entity.ToTable("PrerequisiteMasteries", "Students"));
        }

        private static void ConfigureAdminSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.ToTable("Admins", "Admins");
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey("UserId")
                      .HasPrincipalKey("Id")
                      .HasConstraintName("FK_Admins_Users_UserId");
            });
        }

        private static void ConfigureTeacherSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teachers", "Teachers");
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey("UserId")
                      .HasPrincipalKey("Id")
                      .HasConstraintName("FK_Teachers_Users_UserId");
            });

            modelBuilder.Entity<TeacherAdmin>(entity => entity.ToTable("TeacherAdmins", "Teachers"));
            modelBuilder.Entity<TeacherSubject>(entity => entity.ToTable("TeacherSubjects", "Teachers"));
            modelBuilder.Entity<TeacherStudent>(entity => entity.ToTable("TeacherStudents", "Teachers"));
        }

        private static void ConfigureParentSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Parent>(entity =>
            {
                entity.ToTable("Parents", "Parents");
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey("UserId")
                      .HasPrincipalKey("Id")
                      .HasConstraintName("FK_Parents_Users_UserId");
            });
            modelBuilder.Entity<ParentStudent>(entity => entity.ToTable("ParentStudents", "Parents"));
        }

        private static void ConfigureTutorSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tutor>(entity =>
            {
                entity.ToTable("Tutors", "Tutors");
                entity.HasOne<User>()
                      .WithMany()
                      .HasForeignKey("UserId")
                      .HasPrincipalKey("Id")
                      .HasConstraintName("FK_Tutors_Users_UserId");
            });

            modelBuilder.Entity<TutorAvailability>(entity => entity.ToTable("TutorAvailabilities", "Tutors"));
            modelBuilder.Entity<TutorSubject>(entity => entity.ToTable("TutorSubjects", "Tutors"));
            modelBuilder.Entity<TutorStudent>(entity => entity.ToTable("TutorStudents", "Tutors"));
        }

        private static void ConfigureFeatureSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Feature>(entity => entity.ToTable("Features", "Features"));
            modelBuilder.Entity<RoleFeature>(entity => entity.ToTable("RoleFeatures", "Features"));
            modelBuilder.Entity<UserFeature>(entity => entity.ToTable("UserFeatures", "Features"));
            modelBuilder.Entity<FeaturePurchase>(entity => entity.ToTable("FeaturePurchases", "Features"));
            modelBuilder.Entity<RewardFeature>(entity => entity.ToTable("RewardFeatures", "Features"));
        }

        private static void ConfigureGoalSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Goal>(entity =>
            {
                entity.ToTable("Goals", "Goals");
                entity.HasIndex(g => new { g.StudentId, g.Status });
            });

            modelBuilder.Entity<GrowthTracking>(entity => entity.ToTable("GrowthTrackings", "Rewards"));
            modelBuilder.Entity<GoalMilestone>(entity => entity.ToTable("GoalMilestones", "Goals"));
        }

        private static void ConfigureRewardSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reward>(entity => entity.ToTable("Rewards", "Rewards"));

            modelBuilder.Entity<StudentReward>(entity =>
            {
                entity.ToTable("StudentRewards", "Rewards");
                entity.HasIndex(sr => new { sr.StudentId, sr.Status });
            });

            modelBuilder.Entity<StudentPoints>(entity => entity.ToTable("StudentPoints", "Rewards"));
            modelBuilder.Entity<PointsTransaction>(entity => entity.ToTable("PointsTransactions", "Rewards"));
        }

        private static void ConfigurePsychologySchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiaryEntry>(entity =>
            {
                entity.ToTable("DiaryEntries", "Psychology");
                entity.HasIndex(de => new { de.StudentId, de.EntryDate });
            });

            modelBuilder.Entity<DiaryMoodTracking>(entity =>
            {
                entity.ToTable("DiaryMoodTrackings", "Psychology");
                entity.HasIndex(dmt => new { dmt.StudentId, dmt.TrackingDate }).IsUnique();
            });

            modelBuilder.Entity<DiaryGoal>(entity =>
            {
                entity.ToTable("DiaryGoals", "Psychology");
                entity.HasKey(dg => dg.Id);
            });
        }

        private static void ConfigureCourseSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity => entity.ToTable("Courses", "Courses"));
            modelBuilder.Entity<CourseModule>(entity => entity.ToTable("CourseModules", "Courses"));
            modelBuilder.Entity<ModuleLesson>(entity => entity.ToTable("ModuleLessons", "Courses"));
            modelBuilder.Entity<CourseEnrollment>(entity => entity.ToTable("CourseEnrollments", "Courses"));
        }

        private static void ConfigureResourceSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(entity => entity.ToTable("Resources", "Resources"));
            modelBuilder.Entity<ResourceDownload>(entity => entity.ToTable("ResourceDownloads", "Resources"));
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentSubject>(entity =>
            {
                entity.HasOne(ss => ss.Analytics)
                      .WithOne(a => a.StudentSubject)
                      .HasForeignKey<StudentSubjectAnalytics>(a => a.StudentSubjectId);
            });

            modelBuilder.Entity<StudySession>(entity =>
            {
                entity.HasOne(ss => ss.Student)
                      .WithMany(s => s.StudySessions)
                      .HasForeignKey(ss => ss.StudentId);

                entity.HasOne(ss => ss.Subject)
                      .WithMany()
                      .HasForeignKey(ss => ss.SubjectId);
            });

            modelBuilder.Entity<DiaryGoal>(entity =>
            {
                entity.HasOne(dg => dg.DiaryEntry)
                      .WithMany(de => de.RelatedGoals)
                      .HasForeignKey(dg => dg.DiaryEntryId);

                entity.HasOne(dg => dg.Goal)
                      .WithMany()
                      .HasForeignKey(dg => dg.GoalId);
            });
        }

        private static void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GrowthTracking>(entity =>
            {
                entity.HasIndex(gt => new { gt.StudentId, gt.TrackingDate });
            });

            modelBuilder.Entity<UserFeature>(entity =>
            {
                entity.HasIndex(uf => new { uf.UserId, uf.FeatureId }).IsUnique();
            });
        }

        private static void ConfigureManyToManyRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentSubjectTopic>(entity =>
            {
                entity.HasKey(sst => new { sst.StudentSubjectId, sst.TopicId });
            });

            modelBuilder.Entity<TeacherSubject>(entity =>
            {
                entity.HasKey(ts => new { ts.TeacherId, ts.SubjectId });
            });

            modelBuilder.Entity<TutorSubject>(entity =>
            {
                entity.HasKey(ts => new { ts.TutorId, ts.SubjectId });
            });

            modelBuilder.Entity<TeacherStudent>(entity =>
            {
                entity.HasKey(ts => new { ts.TeacherId, ts.StudentId });
            });

            modelBuilder.Entity<TutorStudent>(entity =>
            {
                entity.HasKey(ts => new { ts.TutorId, ts.StudentId });
            });

            modelBuilder.Entity<ParentStudent>(entity =>
            {
                entity.HasKey(ps => new { ps.ParentId, ps.StudentId });
            });

            modelBuilder.Entity<RoleFeature>(entity =>
            {
                entity.HasKey(rf => new { rf.RoleName, rf.FeatureId });
            });

            modelBuilder.Entity<Goal>(entity =>
            {
                entity.HasMany(g => g.PotentialRewards)
                      .WithMany(r => r.ApplicableGoals)
                      .UsingEntity<Dictionary<string, object>>(
                          "GoalRewards",
                          j => j.HasOne<Reward>().WithMany().HasForeignKey("RewardId"),
                          j => j.HasOne<Goal>().WithMany().HasForeignKey("GoalId"),
                          j => j.ToTable("GoalRewards", "Goals")
                      );
            });
        }
    }
}