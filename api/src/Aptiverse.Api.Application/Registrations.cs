using Aptiverse.Api.Application.Admins.Services;
using Aptiverse.Api.Application.AdminStudents.Services;
using Aptiverse.Api.Application.AssessmentBreakdowns.Services;
using Aptiverse.Api.Application.Assessments.Services;
using Aptiverse.Api.Application.CourseEnrollments.Services;
using Aptiverse.Api.Application.CourseModules.Services;
using Aptiverse.Api.Application.Courses.Services;
using Aptiverse.Api.Application.DiaryEntries.Services;
using Aptiverse.Api.Application.DiaryGoals.Services;
using Aptiverse.Api.Application.DiaryMoodTrackings.Services;
using Aptiverse.Api.Application.FeaturePurchases.Services;
using Aptiverse.Api.Application.Features.Services;
using Aptiverse.Api.Application.GoalMilestones.Services;
using Aptiverse.Api.Application.Goals.Services;
using Aptiverse.Api.Application.GradeDistributions.Services;
using Aptiverse.Api.Application.GrowthTrackings.Services;
using Aptiverse.Api.Application.ImprovementTips.Services;
using Aptiverse.Api.Application.KnowledgeGaps.Services;
using Aptiverse.Api.Application.ModuleLessons.Services;
using Aptiverse.Api.Application.ParentStudents.Services;
using Aptiverse.Api.Application.PeerComparisons.Services;
using Aptiverse.Api.Application.PointsTransactions.Services;
using Aptiverse.Api.Application.PredictionMetricss.Services;
using Aptiverse.Api.Application.ResourceDownloads.Services;
using Aptiverse.Api.Application.Resources.Services;
using Aptiverse.Api.Application.RewardFeatures.Services;
using Aptiverse.Api.Application.Rewards.Services;
using Aptiverse.Api.Application.RoleFeatures.Services;
using Aptiverse.Api.Application.StudentPointss.Services;
using Aptiverse.Api.Application.StudentRewards.Services;
using Aptiverse.Api.Application.Students.Services;
using Aptiverse.Api.Application.StudentSubjectAnalyticss.Services;
using Aptiverse.Api.Application.StudentSubjects.Services;
using Aptiverse.Api.Application.StudySessions.Services;
using Aptiverse.Api.Application.Subjects.Services;
using Aptiverse.Api.Application.TeacherAdmins.Services;
using Aptiverse.Api.Application.Teachers.Services;
using Aptiverse.Api.Application.TeacherStudents.Services;
using Aptiverse.Api.Application.TeacherSubjects.Services;
using Aptiverse.Api.Application.Topics.Services;
using Aptiverse.Api.Application.TutorAvailabilities.Services;
using Aptiverse.Api.Application.Tutors.Services;
using Aptiverse.Api.Application.TutorStudents.Services;
using Aptiverse.Api.Application.TutorSubjects.Services;
using Aptiverse.Api.Application.UserFeatures.Services;
using Aptiverse.Api.Application.WeeklyStudyHours.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aptiverse.Api.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IAdminStudentService, AdminStudentService>();
            services.AddScoped<IAssessmentService, AssessmentService>();
            services.AddScoped<IAssessmentBreakdownService, AssessmentBreakdownService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICourseEnrollmentService, CourseEnrollmentService>();
            services.AddScoped<ICourseModuleService, CourseModuleService>();
            services.AddScoped<IDiaryEntryService, DiaryEntryService>();
            services.AddScoped<IDiaryGoalService, DiaryGoalService>();
            services.AddScoped<IDiaryMoodTrackingService, DiaryMoodTrackingService>();
            services.AddScoped<IFeatureService, FeatureService>();
            services.AddScoped<IFeaturePurchaseService, FeaturePurchaseService>();
            services.AddScoped<IGoalService, GoalService>();
            services.AddScoped<IGoalMilestoneService, GoalMilestoneService>();
            services.AddScoped<IGradeDistributionService, GradeDistributionService>();
            services.AddScoped<IGrowthTrackingService, GrowthTrackingService>();
            services.AddScoped<IImprovementTipService, ImprovementTipService>();
            services.AddScoped<IKnowledgeGapService, KnowledgeGapService>();
            services.AddScoped<IPeerComparisonService, PeerComparisonService>();
            services.AddScoped<IPredictionMetricsService, PredictionMetricsService>();
            services.AddScoped<IStudentSubjectService, StudentSubjectService>();
            services.AddScoped<IStudentSubjectAnalyticsService, StudentSubjectAnalyticsService>();
            services.AddScoped<IStudySessionService, StudySessionService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IWeeklyStudyHourService, WeeklyStudyHourService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ITeacherAdminService, TeacherAdminService>();
            services.AddScoped<ITeacherStudentService, TeacherStudentService>();
            services.AddScoped<ITeacherSubjectService, TeacherSubjectService>();
            services.AddScoped<ITutorSubjectService, TutorSubjectService>();
            services.AddScoped<ITutorService, TutorService>();
            services.AddScoped<ITutorAvailabilityService, TutorAvailabilityService>();
            services.AddScoped<ITutorStudentService, TutorStudentService>();
            services.AddScoped<IModuleLessonService, ModuleLessonService>();
            services.AddScoped<IParentStudentService, ParentStudentService>();
            services.AddScoped<IPointsTransactionService, PointsTransactionService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IResourceDownloadService, ResourceDownloadService>();
            services.AddScoped<IRewardService, RewardService>();
            services.AddScoped<IRewardFeatureService, RewardFeatureService>();
            services.AddScoped<IRoleFeatureService, RoleFeatureService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IStudentPointsService, StudentPointsService>();
            services.AddScoped<IStudentRewardService, StudentRewardService>();
            services.AddScoped<IUserFeatureService, UserFeatureService>();

            return services;
        }
    }
}