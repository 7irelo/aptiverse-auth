using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Aptiverse.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Admins");

            migrationBuilder.EnsureSchema(
                name: "Students");

            migrationBuilder.EnsureSchema(
                name: "Courses");

            migrationBuilder.EnsureSchema(
                name: "Psychology");

            migrationBuilder.EnsureSchema(
                name: "Features");

            migrationBuilder.EnsureSchema(
                name: "Goals");

            migrationBuilder.EnsureSchema(
                name: "Rewards");

            migrationBuilder.EnsureSchema(
                name: "Parents");

            migrationBuilder.EnsureSchema(
                name: "Resources");

            migrationBuilder.EnsureSchema(
                name: "Teachers");

            migrationBuilder.EnsureSchema(
                name: "Tutors");

            migrationBuilder.CreateTable(
                name: "Admins",
                schema: "Admins",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SchoolName = table.Column<string>(type: "text", nullable: false),
                    SchoolCode = table.Column<string>(type: "text", nullable: false),
                    ContactNumber = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                schema: "Features",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceCurrency = table.Column<string>(type: "text", nullable: false),
                    BillingCycle = table.Column<string>(type: "text", nullable: false),
                    ComplexityWeight = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                schema: "Parents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ContactNumber = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Occupation = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parents_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                schema: "Rewards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RewardType = table.Column<string>(type: "text", nullable: false),
                    PointsCost = table.Column<int>(type: "integer", nullable: false),
                    DifficultyTier = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    StockQuantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rewards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    TextColor = table.Column<string>(type: "text", nullable: false),
                    BorderColor = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tutors",
                schema: "Tutors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Qualification = table.Column<string>(type: "text", nullable: false),
                    Specialization = table.Column<string>(type: "text", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    TeachingStyle = table.Column<string>(type: "text", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    TotalReviews = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tutors_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: true),
                    Grade = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Admins_AdminId",
                        column: x => x.AdminId,
                        principalSchema: "Admins",
                        principalTable: "Admins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Students_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                schema: "Teachers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Qualification = table.Column<string>(type: "text", nullable: false),
                    Specialization = table.Column<string>(type: "text", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "integer", nullable: false),
                    Bio = table.Column<string>(type: "text", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "numeric", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teachers_Admins_AdminId",
                        column: x => x.AdminId,
                        principalSchema: "Admins",
                        principalTable: "Admins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Teachers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeaturePurchases",
                schema: "Features",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    BillingCycle = table.Column<string>(type: "text", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturePurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturePurchases_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "Features",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleFeatures",
                schema: "Features",
                columns: table => new
                {
                    RoleName = table.Column<string>(type: "text", nullable: false),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleFeatures", x => new { x.RoleName, x.FeatureId });
                    table.ForeignKey(
                        name: "FK_RoleFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "Features",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFeatures",
                schema: "Features",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GrantType = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "Features",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RewardFeatures",
                schema: "Features",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RewardId = table.Column<long>(type: "bigint", nullable: false),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false),
                    DurationDays = table.Column<int>(type: "integer", nullable: false),
                    FeatureWeight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RewardFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RewardFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalSchema: "Features",
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RewardFeatures_Rewards_RewardId",
                        column: x => x.RewardId,
                        principalSchema: "Rewards",
                        principalTable: "Rewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TutorAvailabilities",
                schema: "Tutors",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TutorId = table.Column<long>(type: "bigint", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorAvailabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutorAvailabilities_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalSchema: "Tutors",
                        principalTable: "Tutors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TutorSubjects",
                schema: "Tutors",
                columns: table => new
                {
                    TutorId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ProficiencyLevel = table.Column<int>(type: "integer", nullable: false),
                    CustomHourlyRate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorSubjects", x => new { x.TutorId, x.SubjectId });
                    table.ForeignKey(
                        name: "FK_TutorSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TutorSubjects_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalSchema: "Tutors",
                        principalTable: "Tutors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assessments",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    MaxScore = table.Column<double>(type: "double precision", nullable: false),
                    DateTaken = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Grade = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assessments_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assessments_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiaryEntries",
                schema: "Psychology",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Mood = table.Column<string>(type: "text", nullable: false),
                    MoodIntensity = table.Column<int>(type: "integer", nullable: false),
                    EntryType = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SentimentAnalysis = table.Column<string>(type: "text", nullable: false),
                    SentimentScore = table.Column<double>(type: "double precision", nullable: false),
                    KeyThemes = table.Column<string>(type: "text", nullable: false),
                    AiInsights = table.Column<string>(type: "text", nullable: false),
                    NeedsFollowUp = table.Column<bool>(type: "boolean", nullable: false),
                    FollowUpAction = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiaryEntries_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiaryMoodTrackings",
                schema: "Psychology",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    TrackingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OverallMood = table.Column<string>(type: "text", nullable: false),
                    EnergyLevel = table.Column<int>(type: "integer", nullable: false),
                    StressLevel = table.Column<int>(type: "integer", nullable: false),
                    MotivationLevel = table.Column<int>(type: "integer", nullable: false),
                    FactorsAffectingMood = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryMoodTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiaryMoodTrackings_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Goals",
                schema: "Goals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    GoalType = table.Column<string>(type: "text", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    TargetValue = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentValue = table.Column<decimal>(type: "numeric", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    DifficultyWeight = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Goals_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Goals_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrowthTrackings",
                schema: "Rewards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    TrackingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcademicGrowth = table.Column<decimal>(type: "numeric", nullable: false),
                    StudyHabitGrowth = table.Column<decimal>(type: "numeric", nullable: false),
                    EmotionalGrowth = table.Column<decimal>(type: "numeric", nullable: false),
                    OverallGrowth = table.Column<decimal>(type: "numeric", nullable: false),
                    GrowthFactors = table.Column<string>(type: "text", nullable: false),
                    AreasForImprovement = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrowthTrackings_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParentStudents",
                schema: "Parents",
                columns: table => new
                {
                    ParentId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Relationship = table.Column<string>(type: "text", nullable: false),
                    IsPrimaryContact = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentStudents", x => new { x.ParentId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_ParentStudents_Parents_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Parents",
                        principalTable: "Parents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParentStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentPoints",
                schema: "Rewards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    TotalPoints = table.Column<int>(type: "integer", nullable: false),
                    AvailablePoints = table.Column<int>(type: "integer", nullable: false),
                    UsedPoints = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    CurrentRank = table.Column<string>(type: "text", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentPoints_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentSubjects",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Progress = table.Column<int>(type: "integer", nullable: true),
                    Target = table.Column<int>(type: "integer", nullable: true),
                    AverageScore = table.Column<double>(type: "double precision", nullable: true),
                    StudyHours = table.Column<int>(type: "integer", nullable: true),
                    AssignmentsCompleted = table.Column<int>(type: "integer", nullable: true),
                    UpcomingDeadlines = table.Column<int>(type: "integer", nullable: true),
                    Strength = table.Column<string>(type: "text", nullable: true),
                    Weakness = table.Column<string>(type: "text", nullable: true),
                    LastActivity = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PerformanceTrend = table.Column<string>(type: "text", nullable: true),
                    StudyEfficiency = table.Column<double>(type: "double precision", nullable: true),
                    PredictedScore = table.Column<double>(type: "double precision", nullable: true),
                    DifficultyLevel = table.Column<double>(type: "double precision", nullable: true),
                    ConfidenceLevel = table.Column<double>(type: "double precision", nullable: true),
                    LearningVelocity = table.Column<double>(type: "double precision", nullable: true),
                    RetentionRate = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSubjects_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudySessions",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    SessionType = table.Column<string>(type: "text", nullable: false),
                    TopicsCovered = table.Column<string>(type: "text", nullable: false),
                    EfficiencyScore = table.Column<double>(type: "double precision", nullable: false),
                    ConcentrationLevel = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    ResourcesUsed = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySessions_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudySessions_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TutorStudents",
                schema: "Tutors",
                columns: table => new
                {
                    TutorId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    SessionsPerWeek = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorStudents", x => new { x.TutorId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_TutorStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TutorStudents_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalSchema: "Tutors",
                        principalTable: "Tutors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                schema: "Courses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    TeacherId = table.Column<long>(type: "bigint", nullable: true),
                    TutorId = table.Column<long>(type: "bigint", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Level = table.Column<string>(type: "text", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: false),
                    PreviewVideoUrl = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    TotalStudents = table.Column<int>(type: "integer", nullable: false),
                    TotalLessons = table.Column<int>(type: "integer", nullable: false),
                    TotalHours = table.Column<decimal>(type: "numeric", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "Teachers",
                        principalTable: "Teachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courses_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalSchema: "Tutors",
                        principalTable: "Tutors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TeacherAdmins",
                schema: "Teachers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeacherId = table.Column<long>(type: "bigint", nullable: false),
                    AdminId = table.Column<long>(type: "bigint", nullable: false),
                    AssociatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherAdmins_Admins_AdminId",
                        column: x => x.AdminId,
                        principalSchema: "Admins",
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherAdmins_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "Teachers",
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherStudents",
                schema: "Teachers",
                columns: table => new
                {
                    TeacherId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherStudents", x => new { x.TeacherId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_TeacherStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherStudents_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "Teachers",
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSubjects",
                schema: "Teachers",
                columns: table => new
                {
                    TeacherId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ProficiencyLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjects", x => new { x.TeacherId, x.SubjectId });
                    table.ForeignKey(
                        name: "FK_TeacherSubjects_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherSubjects_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "Teachers",
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiaryGoals",
                schema: "Psychology",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiaryEntryId = table.Column<long>(type: "bigint", nullable: false),
                    GoalId = table.Column<long>(type: "bigint", nullable: false),
                    ConnectionType = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryGoals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiaryGoals_DiaryEntries_DiaryEntryId",
                        column: x => x.DiaryEntryId,
                        principalSchema: "Psychology",
                        principalTable: "DiaryEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiaryGoals_Goals_GoalId",
                        column: x => x.GoalId,
                        principalSchema: "Goals",
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoalMilestones",
                schema: "Goals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GoalId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TargetValue = table.Column<decimal>(type: "numeric", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RewardPoints = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoalMilestones_Goals_GoalId",
                        column: x => x.GoalId,
                        principalSchema: "Goals",
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GoalRewards",
                schema: "Goals",
                columns: table => new
                {
                    GoalId = table.Column<long>(type: "bigint", nullable: false),
                    RewardId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalRewards", x => new { x.GoalId, x.RewardId });
                    table.ForeignKey(
                        name: "FK_GoalRewards_Goals_GoalId",
                        column: x => x.GoalId,
                        principalSchema: "Goals",
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalRewards_Rewards_RewardId",
                        column: x => x.RewardId,
                        principalSchema: "Rewards",
                        principalTable: "Rewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentRewards",
                schema: "Rewards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    RewardId = table.Column<long>(type: "bigint", nullable: false),
                    GoalId = table.Column<long>(type: "bigint", nullable: true),
                    EarnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PointsEarned = table.Column<int>(type: "integer", nullable: false),
                    AchievementContext = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentRewards_Goals_GoalId",
                        column: x => x.GoalId,
                        principalSchema: "Goals",
                        principalTable: "Goals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentRewards_Rewards_RewardId",
                        column: x => x.RewardId,
                        principalSchema: "Rewards",
                        principalTable: "Rewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentRewards_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointsTransactions",
                schema: "Rewards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentPointsId = table.Column<long>(type: "bigint", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    RelatedGoalId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedRewardId = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointsTransactions_Goals_RelatedGoalId",
                        column: x => x.RelatedGoalId,
                        principalSchema: "Goals",
                        principalTable: "Goals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PointsTransactions_Rewards_RelatedRewardId",
                        column: x => x.RelatedRewardId,
                        principalSchema: "Rewards",
                        principalTable: "Rewards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PointsTransactions_StudentPoints_StudentPointsId",
                        column: x => x.StudentPointsId,
                        principalSchema: "Rewards",
                        principalTable: "StudentPoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssessmentBreakdowns",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    AssessmentType = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false),
                    Average = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssessmentBreakdowns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssessmentBreakdowns_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradeDistributions",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    Grade = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradeDistributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradeDistributions_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImprovementTips",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    Tip = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImprovementTips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImprovementTips_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KnowledgeGaps",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    Concept = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<string>(type: "text", nullable: false),
                    LastTested = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnowledgeGaps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KnowledgeGaps_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeerComparisons",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    ClassAverage = table.Column<double>(type: "double precision", nullable: false),
                    Percentile = table.Column<int>(type: "integer", nullable: false),
                    Ranking = table.Column<int>(type: "integer", nullable: false),
                    TrendComparison = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeerComparisons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeerComparisons_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredictionMetrics",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    FinalGradeProbabilityA = table.Column<int>(type: "integer", nullable: false),
                    FinalGradeProbabilityB = table.Column<int>(type: "integer", nullable: false),
                    FinalGradeProbabilityC = table.Column<int>(type: "integer", nullable: false),
                    FinalGradeProbabilityD = table.Column<int>(type: "integer", nullable: false),
                    RiskLevel = table.Column<string>(type: "text", nullable: false),
                    InterventionNeeded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionMetrics_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrerequisiteMasteries",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    Prerequisite = table.Column<string>(type: "text", nullable: false),
                    MasteryLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrerequisiteMasteries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrerequisiteMasteries_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentSubjectAnalytics",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    MorningPercentage = table.Column<int>(type: "integer", nullable: false),
                    AfternoonPercentage = table.Column<int>(type: "integer", nullable: false),
                    EveningPercentage = table.Column<int>(type: "integer", nullable: false),
                    Consistency = table.Column<int>(type: "integer", nullable: false),
                    PreferredDays = table.Column<string>(type: "text", nullable: false),
                    SessionLength = table.Column<int>(type: "integer", nullable: false),
                    ClassesAttended = table.Column<int>(type: "integer", nullable: false),
                    TotalClasses = table.Column<int>(type: "integer", nullable: false),
                    AttendanceRate = table.Column<double>(type: "double precision", nullable: false),
                    TextbookUsage = table.Column<int>(type: "integer", nullable: false),
                    VideoTutorials = table.Column<int>(type: "integer", nullable: false),
                    PracticeProblems = table.Column<int>(type: "integer", nullable: false),
                    GroupStudy = table.Column<int>(type: "integer", nullable: false),
                    OnlinePlatforms = table.Column<int>(type: "integer", nullable: false),
                    QuestionsAsked = table.Column<int>(type: "integer", nullable: false),
                    ParticipationRate = table.Column<int>(type: "integer", nullable: false),
                    ResourceDownloads = table.Column<int>(type: "integer", nullable: false),
                    ForumActivity = table.Column<int>(type: "integer", nullable: false),
                    WorkloadThisWeek = table.Column<double>(type: "double precision", nullable: false),
                    StressLevel = table.Column<double>(type: "double precision", nullable: false),
                    SleepQuality = table.Column<double>(type: "double precision", nullable: false),
                    MotivationLevel = table.Column<double>(type: "double precision", nullable: false),
                    Importance = table.Column<int>(type: "integer", nullable: false),
                    InterestLevel = table.Column<double>(type: "double precision", nullable: false),
                    Alignment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSubjectAnalytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentSubjectAnalytics_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentSubjectTopics",
                schema: "Students",
                columns: table => new
                {
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    TopicId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    Trend = table.Column<string>(type: "text", nullable: false),
                    LastTested = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentSubjectTopics", x => new { x.StudentSubjectId, x.TopicId });
                    table.ForeignKey(
                        name: "FK_StudentSubjectTopics_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentSubjectTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalSchema: "Students",
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyStudyHours",
                schema: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentSubjectId = table.Column<long>(type: "bigint", nullable: false),
                    WeekNumber = table.Column<int>(type: "integer", nullable: false),
                    Hours = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyStudyHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeeklyStudyHours_StudentSubjects_StudentSubjectId",
                        column: x => x.StudentSubjectId,
                        principalSchema: "Students",
                        principalTable: "StudentSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseEnrollments",
                schema: "Courses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    EnrolledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    Progress = table.Column<decimal>(type: "numeric", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseModules",
                schema: "Courses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DurationHours = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseModules_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                schema: "Resources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    SubjectId = table.Column<string>(type: "text", nullable: false),
                    TeacherId = table.Column<long>(type: "bigint", nullable: true),
                    TutorId = table.Column<long>(type: "bigint", nullable: true),
                    CourseId = table.Column<long>(type: "bigint", nullable: true),
                    ResourceType = table.Column<string>(type: "text", nullable: false),
                    S3Key = table.Column<string>(type: "text", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<string>(type: "text", nullable: false),
                    FileFormat = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    IsFree = table.Column<bool>(type: "boolean", nullable: false),
                    DownloadCount = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    GradeLevel = table.Column<string>(type: "text", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "Courses",
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Resources_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Students",
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resources_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "Teachers",
                        principalTable: "Teachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Resources_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalSchema: "Tutors",
                        principalTable: "Tutors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ModuleLessons",
                schema: "Courses",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuleId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    VideoUrl = table.Column<string>(type: "text", nullable: false),
                    ResourceUrls = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    DurationMinutes = table.Column<decimal>(type: "numeric", nullable: false),
                    IsFreePreview = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleLessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleLessons_CourseModules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "Courses",
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceDownloads",
                schema: "Resources",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ResourceId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    DownloadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceDownloads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceDownloads_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "Resources",
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceDownloads_Students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Students",
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                schema: "Admins",
                table: "Admins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssessmentBreakdowns_StudentSubjectId",
                schema: "Students",
                table: "AssessmentBreakdowns",
                column: "StudentSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_StudentId",
                schema: "Students",
                table: "Assessments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_SubjectId",
                schema: "Students",
                table: "Assessments",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_CourseId",
                schema: "Courses",
                table: "CourseEnrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_StudentId",
                schema: "Courses",
                table: "CourseEnrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_CourseId",
                schema: "Courses",
                table: "CourseModules",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubjectId",
                schema: "Courses",
                table: "Courses",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                schema: "Courses",
                table: "Courses",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TutorId",
                schema: "Courses",
                table: "Courses",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryEntries_StudentId_EntryDate",
                schema: "Psychology",
                table: "DiaryEntries",
                columns: new[] { "StudentId", "EntryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DiaryGoals_DiaryEntryId",
                schema: "Psychology",
                table: "DiaryGoals",
                column: "DiaryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryGoals_GoalId",
                schema: "Psychology",
                table: "DiaryGoals",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_DiaryMoodTrackings_StudentId_TrackingDate",
                schema: "Psychology",
                table: "DiaryMoodTrackings",
                columns: new[] { "StudentId", "TrackingDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeaturePurchases_FeatureId",
                schema: "Features",
                table: "FeaturePurchases",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalMilestones_GoalId",
                schema: "Goals",
                table: "GoalMilestones",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalRewards_RewardId",
                schema: "Goals",
                table: "GoalRewards",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_StudentId_Status",
                schema: "Goals",
                table: "Goals",
                columns: new[] { "StudentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_SubjectId",
                schema: "Goals",
                table: "Goals",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GradeDistributions_StudentSubjectId",
                schema: "Students",
                table: "GradeDistributions",
                column: "StudentSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GrowthTrackings_StudentId_TrackingDate",
                schema: "Rewards",
                table: "GrowthTrackings",
                columns: new[] { "StudentId", "TrackingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ImprovementTips_StudentSubjectId",
                schema: "Students",
                table: "ImprovementTips",
                column: "StudentSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeGaps_StudentSubjectId",
                schema: "Students",
                table: "KnowledgeGaps",
                column: "StudentSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleLessons_ModuleId",
                schema: "Courses",
                table: "ModuleLessons",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Parents_UserId",
                schema: "Parents",
                table: "Parents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentStudents_StudentId",
                schema: "Parents",
                table: "ParentStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PeerComparisons_StudentSubjectId",
                schema: "Students",
                table: "PeerComparisons",
                column: "StudentSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsTransactions_RelatedGoalId",
                schema: "Rewards",
                table: "PointsTransactions",
                column: "RelatedGoalId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsTransactions_RelatedRewardId",
                schema: "Rewards",
                table: "PointsTransactions",
                column: "RelatedRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsTransactions_StudentPointsId",
                schema: "Rewards",
                table: "PointsTransactions",
                column: "StudentPointsId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionMetrics_StudentSubjectId",
                schema: "Students",
                table: "PredictionMetrics",
                column: "StudentSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PrerequisiteMasteries_StudentSubjectId",
                schema: "Students",
                table: "PrerequisiteMasteries",
                column: "StudentSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceDownloads_ResourceId",
                schema: "Resources",
                table: "ResourceDownloads",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceDownloads_StudentId",
                schema: "Resources",
                table: "ResourceDownloads",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_CourseId",
                schema: "Resources",
                table: "Resources",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_SubjectId",
                schema: "Resources",
                table: "Resources",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_TeacherId",
                schema: "Resources",
                table: "Resources",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_TutorId",
                schema: "Resources",
                table: "Resources",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardFeatures_FeatureId",
                schema: "Features",
                table: "RewardFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_RewardFeatures_RewardId",
                schema: "Features",
                table: "RewardFeatures",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleFeatures_FeatureId",
                schema: "Features",
                table: "RoleFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentPoints_StudentId",
                schema: "Rewards",
                table: "StudentPoints",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRewards_GoalId",
                schema: "Rewards",
                table: "StudentRewards",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRewards_RewardId",
                schema: "Rewards",
                table: "StudentRewards",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRewards_StudentId_Status",
                schema: "Rewards",
                table: "StudentRewards",
                columns: new[] { "StudentId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_AdminId",
                schema: "Students",
                table: "Students",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                schema: "Students",
                table: "Students",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectAnalytics_StudentSubjectId",
                schema: "Students",
                table: "StudentSubjectAnalytics",
                column: "StudentSubjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_StudentId",
                schema: "Students",
                table: "StudentSubjects",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_SubjectId",
                schema: "Students",
                table: "StudentSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjectTopics_TopicId",
                schema: "Students",
                table: "StudentSubjectTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_StudentId",
                schema: "Students",
                table: "StudySessions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_SubjectId",
                schema: "Students",
                table: "StudySessions",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAdmins_AdminId",
                schema: "Teachers",
                table: "TeacherAdmins",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherAdmins_TeacherId",
                schema: "Teachers",
                table: "TeacherAdmins",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_AdminId",
                schema: "Teachers",
                table: "Teachers",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserId",
                schema: "Teachers",
                table: "Teachers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherStudents_StudentId",
                schema: "Teachers",
                table: "TeacherStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_SubjectId",
                schema: "Teachers",
                table: "TeacherSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_SubjectId",
                schema: "Students",
                table: "Topics",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TutorAvailabilities_TutorId",
                schema: "Tutors",
                table: "TutorAvailabilities",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tutors_UserId",
                schema: "Tutors",
                table: "Tutors",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TutorStudents_StudentId",
                schema: "Tutors",
                table: "TutorStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_TutorSubjects_SubjectId",
                schema: "Tutors",
                table: "TutorSubjects",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFeatures_FeatureId",
                schema: "Features",
                table: "UserFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFeatures_UserId_FeatureId",
                schema: "Features",
                table: "UserFeatures",
                columns: new[] { "UserId", "FeatureId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyStudyHours_StudentSubjectId",
                schema: "Students",
                table: "WeeklyStudyHours",
                column: "StudentSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssessmentBreakdowns",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "Assessments",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "CourseEnrollments",
                schema: "Courses");

            migrationBuilder.DropTable(
                name: "DiaryGoals",
                schema: "Psychology");

            migrationBuilder.DropTable(
                name: "DiaryMoodTrackings",
                schema: "Psychology");

            migrationBuilder.DropTable(
                name: "FeaturePurchases",
                schema: "Features");

            migrationBuilder.DropTable(
                name: "GoalMilestones",
                schema: "Goals");

            migrationBuilder.DropTable(
                name: "GoalRewards",
                schema: "Goals");

            migrationBuilder.DropTable(
                name: "GradeDistributions",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "GrowthTrackings",
                schema: "Rewards");

            migrationBuilder.DropTable(
                name: "ImprovementTips",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "KnowledgeGaps",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "ModuleLessons",
                schema: "Courses");

            migrationBuilder.DropTable(
                name: "ParentStudents",
                schema: "Parents");

            migrationBuilder.DropTable(
                name: "PeerComparisons",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "PointsTransactions",
                schema: "Rewards");

            migrationBuilder.DropTable(
                name: "PredictionMetrics",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "PrerequisiteMasteries",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "ResourceDownloads",
                schema: "Resources");

            migrationBuilder.DropTable(
                name: "RewardFeatures",
                schema: "Features");

            migrationBuilder.DropTable(
                name: "RoleFeatures",
                schema: "Features");

            migrationBuilder.DropTable(
                name: "StudentRewards",
                schema: "Rewards");

            migrationBuilder.DropTable(
                name: "StudentSubjectAnalytics",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "StudentSubjectTopics",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "StudySessions",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "TeacherAdmins",
                schema: "Teachers");

            migrationBuilder.DropTable(
                name: "TeacherStudents",
                schema: "Teachers");

            migrationBuilder.DropTable(
                name: "TeacherSubjects",
                schema: "Teachers");

            migrationBuilder.DropTable(
                name: "TutorAvailabilities",
                schema: "Tutors");

            migrationBuilder.DropTable(
                name: "TutorStudents",
                schema: "Tutors");

            migrationBuilder.DropTable(
                name: "TutorSubjects",
                schema: "Tutors");

            migrationBuilder.DropTable(
                name: "UserFeatures",
                schema: "Features");

            migrationBuilder.DropTable(
                name: "WeeklyStudyHours",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "DiaryEntries",
                schema: "Psychology");

            migrationBuilder.DropTable(
                name: "CourseModules",
                schema: "Courses");

            migrationBuilder.DropTable(
                name: "Parents",
                schema: "Parents");

            migrationBuilder.DropTable(
                name: "StudentPoints",
                schema: "Rewards");

            migrationBuilder.DropTable(
                name: "Resources",
                schema: "Resources");

            migrationBuilder.DropTable(
                name: "Goals",
                schema: "Goals");

            migrationBuilder.DropTable(
                name: "Rewards",
                schema: "Rewards");

            migrationBuilder.DropTable(
                name: "Topics",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "Features",
                schema: "Features");

            migrationBuilder.DropTable(
                name: "StudentSubjects",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "Courses",
                schema: "Courses");

            migrationBuilder.DropTable(
                name: "Students",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "Subjects",
                schema: "Students");

            migrationBuilder.DropTable(
                name: "Teachers",
                schema: "Teachers");

            migrationBuilder.DropTable(
                name: "Tutors",
                schema: "Tutors");

            migrationBuilder.DropTable(
                name: "Admins",
                schema: "Admins");
        }
    }
}
