using Aptiverse.Api.Domain.Models.Students;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Aptiverse.Api.Infrastructure.Data
{
    public static class SubjectSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Subjects.AnyAsync())
                return;

            var subjects = new List<Subject>
            {
                // ================= CORE SUBJECTS =================
                CreateSubject("Mathematics", "MAT", "Algebra, Calculus, Geometry and Mathematical Literacy", "bg-blue-500", "text-blue-600", "border-blue-200"),
                CreateSubject("Physical Sciences", "PSC", "Physics and Chemistry fundamentals", "bg-purple-500", "text-purple-600", "border-purple-200"),
                CreateSubject("Life Sciences", "LSC", "Biology, Ecology and Human Anatomy", "bg-green-500", "text-green-600", "border-green-200"),
                CreateSubject("English Home Language", "ENG", "Language, Literature and Communication", "bg-red-500", "text-red-600", "border-red-200"),
                
                // ================= LANGUAGES =================
                CreateSubject("Afrikaans FAL", "AFR", "First Additional Language - Reading and Writing", "bg-orange-500", "text-orange-600", "border-orange-200"),
                CreateSubject("isiZulu Home Language", "ZUL", "isiZulu language and literature", "bg-yellow-500", "text-yellow-600", "border-yellow-200"),
                CreateSubject("isiXhosa Home Language", "XHO", "isiXhosa language and literature", "bg-lime-500", "text-lime-600", "border-lime-200"),
                CreateSubject("Sepedi Home Language", "SEP", "Sepedi language and literature", "bg-teal-500", "text-teal-600", "border-teal-200"),
                
                // ================= HUMANITIES =================
                CreateSubject("Geography", "GEO", "Physical and Human Geography", "bg-cyan-500", "text-cyan-600", "border-cyan-200"),
                CreateSubject("History", "HIS", "World History and South African History", "bg-gray-500", "text-gray-600", "border-gray-200"),
                CreateSubject("Religion Studies", "REL", "Comparative Religion and Ethics", "bg-indigo-500", "text-indigo-600", "border-indigo-200"),
                
                // ================= COMMERCE =================
                CreateSubject("Accounting", "ACC", "Financial Accounting and Business Principles", "bg-violet-500", "text-violet-600", "border-violet-200"),
                CreateSubject("Business Studies", "BUS", "Business Management and Entrepreneurship", "bg-emerald-500", "text-emerald-600", "border-emerald-200"),
                CreateSubject("Economics", "ECO", "Micro and Macroeconomics", "bg-amber-500", "text-amber-600", "border-amber-200"),
                
                // ================= SCIENCES & TECHNOLOGY =================
                CreateSubject("Agricultural Sciences", "AGR", "Agricultural Management and Plant Studies", "bg-green-600", "text-green-700", "border-green-300"),
                CreateSubject("Information Technology", "ICT", "Computer Systems and Programming", "bg-slate-500", "text-slate-600", "border-slate-200"),
                CreateSubject("Computer Applications Technology", "CAT", "Computer Literacy and Office Applications", "bg-zinc-500", "text-zinc-600", "border-zinc-200"),
                CreateSubject("Engineering Graphics and Design", "EGD", "Technical Drawing and Engineering Design", "bg-stone-500", "text-stone-600", "border-stone-200"),
                
                // ================= LIFE ORIENTATION & ARTS =================
                CreateSubject("Life Orientation", "LOR", "Personal and Social Well-being", "bg-pink-500", "text-pink-600", "border-pink-200"),
                CreateSubject("Mathematical Literacy", "MLT", "Practical Mathematics for Daily Life", "bg-sky-500", "text-sky-600", "border-sky-200"),
                CreateSubject("Visual Arts", "ART", "Drawing, Painting and Art Theory", "bg-rose-500", "text-rose-600", "border-rose-200"),
                CreateSubject("Dramatic Arts", "DRA", "Theatre and Performance Studies", "bg-fuchsia-500", "text-fuchsia-600", "border-fuchsia-200"),
                
                // ================= SERVICES & TOURISM =================
                CreateSubject("Tourism", "TOU", "Travel, Tourism and Hospitality", "bg-blue-400", "text-blue-500", "border-blue-100"),
                CreateSubject("Hospitality Studies", "HOS", "Food Preparation and Service", "bg-orange-400", "text-orange-500", "border-orange-100"),
                CreateSubject("Consumer Studies", "CON", "Consumer Rights and Home Economics", "bg-red-400", "text-red-500", "border-red-100"),
                
                // ================= ADDITIONAL LANGUAGES =================
                CreateSubject("Sesotho Home Language", "SES", "Sesotho language and literature", "bg-emerald-400", "text-emerald-500", "border-emerald-100"),
                CreateSubject("Setswana Home Language", "SET", "Setswana language and literature", "bg-cyan-400", "text-cyan-500", "border-cyan-100"),
                CreateSubject("Music", "MUS", "Music Theory and Performance", "bg-purple-400", "text-purple-500", "border-purple-100"),
                CreateSubject("Design", "DES", "Graphic and Product Design", "bg-amber-400", "text-amber-500", "border-amber-100")
            };

            await context.Subjects.AddRangeAsync(subjects);
            await context.SaveChangesAsync();
        }

        private static Subject CreateSubject(string name, string code, string description,
            string color, string textColor, string borderColor)
        {
            return new Subject
            {
                Id = GenerateSlug(name),
                Name = name,
                Code = code,
                Description = description,
                Color = color,
                TextColor = textColor,
                BorderColor = borderColor
            };
        }

        /// <summary>
        /// Converts "Physical Sciences" → "physical-sciences"
        /// </summary>
        private static string GenerateSlug(string input)
        {
            input = input.ToLowerInvariant().Trim();

            // Remove invalid chars
            input = Regex.Replace(input, @"[^a-z0-9\s-]", "");

            // Convert spaces to hyphens
            input = Regex.Replace(input, @"\s+", "-");

            return input;
        }
    }
}