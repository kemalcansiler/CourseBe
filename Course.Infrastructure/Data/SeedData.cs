using Course.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Course.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedCoursesAsync(ApplicationDbContext context)
    {
        // Check if courses already exist
        if (await context.Courses.AnyAsync())
        {
            Console.WriteLine("Database already contains courses. Skipping seed.");
            return;
        }

        Console.WriteLine("Seeding course data...");

        // Ensure we have categories
        var categories = await EnsureCategoriesExist(context);

        var courses = GetCourses(categories);

        await context.Courses.AddRangeAsync(courses);
        await context.SaveChangesAsync();

        Console.WriteLine($"Successfully seeded {courses.Count} courses!");
    }

    private static async Task<Dictionary<string, Category>> EnsureCategoriesExist(ApplicationDbContext context)
    {
        var categoryNames = new[] { "Web Development", "Programming", "Mobile Development", "Data Science", "Business" };
        var categories = new Dictionary<string, Category>();

        foreach (var name in categoryNames)
        {
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Name == name);
            if (category == null)
            {
                category = new Category
                {
                    Name = name,
                    Description = $"{name} courses",
                    CreatedAt = DateTime.UtcNow
                };
                context.Categories.Add(category);
            }
            categories[name] = category;
        }

        await context.SaveChangesAsync();
        return categories;
    }

    private static List<Core.Entities.Course> GetCourses(Dictionary<string, Category> categories)
    {
        var webDev = categories["Web Development"];
        var programming = categories["Programming"];
        var mobile = categories["Mobile Development"];
        var dataScience = categories["Data Science"];
        var business = categories["Business"];
        
        var random = new Random(42);
        var courses = new List<Core.Entities.Course>();
        
        // Course data templates
        var webDevCourses = new[]
        {
            ("Complete Web Development Bootcamp", "Full-stack web development from scratch", 1200),
            ("Advanced CSS and Sass", "Master modern CSS including Flexbox, Grid, Sass", 600),
            ("JavaScript - The Complete Guide", "Modern JavaScript from beginner to advanced", 2400),
            ("TypeScript Fundamentals", "Learn TypeScript for modern development", 800),
            ("Vue.js Complete Course", "Build reactive applications with Vue.js", 1800),
            ("Svelte and SvelteKit Course", "Modern frontend with Svelte", 1400),
            ("Tailwind CSS Mastery", "Utility-first CSS framework course", 500),
            ("Bootstrap 5 Complete Course", "Responsive design with Bootstrap 5", 700),
            ("Web Design for Developers", "Learn design principles and UI/UX", 900),
            ("Web Accessibility (a11y) Course", "Build accessible websites for everyone", 600),
            ("Progressive Web Apps (PWA)", "Build offline-first web applications", 1000),
            ("GraphQL Complete Guide", "Modern API development with GraphQL", 1200),
            ("REST API Design Best Practices", "Design scalable RESTful APIs", 800),
            ("WebAssembly Fundamentals", "High-performance web with WebAssembly", 1100),
            ("Three.js and WebGL", "3D graphics on the web", 1500),
            ("Electron Desktop Apps", "Build cross-platform desktop apps", 1300),
            ("Chrome Extension Development", "Create browser extensions", 700),
            ("Web Performance Optimization", "Make your websites blazing fast", 900),
            ("SEO for Developers", "Search engine optimization techniques", 600),
            ("Webpack and Module Bundlers", "Modern build tools and workflows", 800),
            ("Vite Build Tool Course", "Next generation frontend tooling", 500),
            ("Microservices Architecture", "Design and build microservices", 2000),
            ("Docker for Web Developers", "Containerize your applications", 1100),
            ("Kubernetes Essentials", "Container orchestration basics", 1400),
            ("CI/CD Pipeline Setup", "Continuous integration and deployment", 900),
            ("Testing JavaScript Applications", "Jest, Testing Library, E2E tests", 1200),
            ("Web Security Fundamentals", "Secure your web applications", 1000),
            ("OAuth and JWT Authentication", "Modern authentication methods", 700),
            ("Serverless Applications", "Build with AWS Lambda and more", 1300),
            ("JAMstack Development", "Modern web architecture", 1000)
        };

        var programmingCourses = new[]
        {
            ("Python Complete Course", "From basics to advanced Python", 3000),
            ("Java Programming Masterclass", "Complete Java development", 4000),
            ("C# and .NET Development", "Build applications with C#", 3500),
            ("Go Programming Language", "Modern systems programming with Go", 1800),
            ("Rust Programming Course", "Safe and concurrent programming", 2200),
            ("C++ Complete Guide", "Modern C++ programming", 3200),
            ("Ruby and Ruby on Rails", "Web development with Ruby", 2400),
            ("PHP Modern Development", "PHP 8 features and best practices", 1600),
            ("Kotlin Programming", "Modern JVM language", 1400),
            ("Swift Programming", "iOS app development language", 1800),
            ("Scala Functional Programming", "FP with Scala", 2000),
            ("Elixir and Phoenix", "Concurrent programming with Elixir", 1900),
            ("Haskell Functional Programming", "Pure functional programming", 2100),
            ("Dart Programming Language", "Flutter development language", 1300),
            ("Perl Programming", "Text processing and scripting", 1200),
            ("Shell Scripting Mastery", "Bash and shell automation", 800),
            ("PowerShell for Developers", "Windows automation", 1000),
            ("R Programming for Beginners", "Statistical computing with R", 1500),
            ("Julia Programming", "High-performance computing", 1400),
            ("Clojure Programming", "Lisp for the JVM", 1600),
            ("F# Functional Programming", ".NET functional language", 1300),
            ("Assembly Language Programming", "Low-level programming", 1800),
            ("COBOL Programming", "Legacy systems development", 1400),
            ("Fortran for Scientists", "Scientific computing", 1200),
            ("Ada Programming", "Reliable systems programming", 1100),
            ("Groovy Programming", "Dynamic JVM language", 900),
            ("Lua Scripting", "Embedded scripting language", 700),
            ("Objective-C Programming", "iOS development legacy", 1600),
            ("Visual Basic .NET", "Windows application development", 1300),
            ("MATLAB Programming", "Numerical computing", 1700)
        };

        var mobileCourses = new[]
        {
            ("iOS App Development with Swift", "Build iPhone apps", 2800),
            ("Android Development with Kotlin", "Modern Android apps", 3000),
            ("Flutter Complete Course", "Cross-platform mobile apps", 2400),
            ("React Native Masterclass", "Build native apps with React", 2200),
            ("Ionic Framework Course", "Hybrid mobile applications", 1600),
            ("SwiftUI Complete Guide", "Modern iOS interfaces", 1800),
            ("Jetpack Compose for Android", "Declarative UI for Android", 1700),
            ("Mobile App Design", "UI/UX for mobile applications", 1200),
            ("Firebase for Mobile Apps", "Backend services for mobile", 1400),
            ("Mobile Game Development", "Build games for iOS and Android", 2500),
            ("ARKit and ARCore", "Augmented reality apps", 1900),
            ("Mobile App Testing", "Quality assurance for mobile", 1100),
            ("App Store Optimization", "Get more downloads", 600),
            ("Mobile App Monetization", "Revenue strategies", 700),
            ("Progressive Web Apps Mobile", "PWAs for mobile devices", 900),
            ("Xamarin Development", "Cross-platform with C#", 2000),
            ("NativeScript Course", "Native apps with JavaScript", 1500),
            ("Mobile App Security", "Secure your mobile apps", 1000),
            ("Push Notifications", "Engage users with notifications", 500),
            ("Mobile Analytics", "Track app usage and behavior", 600),
            ("Wearable App Development", "Apps for smartwatches", 1100),
            ("Mobile Backend Development", "Build APIs for mobile apps", 1600),
            ("Cordova and PhoneGap", "Hybrid app development", 1300),
            ("Mobile Performance Optimization", "Fast and responsive apps", 800),
            ("Bluetooth and IoT Apps", "Connect with devices", 1200),
            ("Mobile Camera and Media", "Work with photos and videos", 900),
            ("Location-Based Apps", "GPS and mapping applications", 1000),
            ("Mobile Payment Integration", "In-app purchases and payments", 700),
            ("Offline-First Mobile Apps", "Build apps that work offline", 1100),
            ("Mobile App Deployment", "Release to app stores", 500)
        };

        var dataScienceCourses = new[]
        {
            ("Machine Learning A-Z", "Complete ML course", 4000),
            ("Deep Learning Specialization", "Neural networks and AI", 3500),
            ("Data Science Complete Bootcamp", "From data to insights", 3800),
            ("Python for Data Science", "Pandas, NumPy, Matplotlib", 2400),
            ("R for Data Science", "Statistical analysis with R", 2200),
            ("SQL for Data Analysis", "Database querying skills", 1800),
            ("Tableau Complete Course", "Data visualization with Tableau", 1600),
            ("Power BI Masterclass", "Business intelligence with Power BI", 1700),
            ("Big Data with Hadoop", "Process large datasets", 2000),
            ("Apache Spark Course", "Distributed data processing", 1900),
            ("Natural Language Processing", "Text analysis and NLP", 2600),
            ("Computer Vision with OpenCV", "Image processing and analysis", 2300),
            ("TensorFlow Complete Guide", "Deep learning with TensorFlow", 2800),
            ("PyTorch for Deep Learning", "Neural networks with PyTorch", 2700),
            ("Time Series Analysis", "Forecasting and predictions", 1500),
            ("Statistics for Data Science", "Statistical foundations", 2000),
            ("A/B Testing and Experimentation", "Data-driven decisions", 1200),
            ("Data Engineering Essentials", "Build data pipelines", 2200),
            ("ETL Processes", "Extract, transform, load data", 1400),
            ("Data Warehousing", "Design data warehouses", 1600),
            ("MongoDB for Data Science", "NoSQL databases", 1300),
            ("Elasticsearch and Kibana", "Search and analytics", 1500),
            ("Recommender Systems", "Build recommendation engines", 1700),
            ("Reinforcement Learning", "AI that learns from actions", 2400),
            ("Generative AI and GPT", "Large language models", 2000),
            ("MLOps - ML Operations", "Deploy and monitor ML models", 1800),
            ("Feature Engineering", "Improve model performance", 1100),
            ("AutoML and Model Selection", "Automated machine learning", 1000),
            ("Kaggle Competition Strategies", "Win data science competitions", 1300),
            ("AI Ethics and Bias", "Responsible AI development", 600)
        };

        var businessCourses = new[]
        {
            ("Digital Marketing Complete Course", "Marketing strategies", 2000),
            ("SEO and SEM Mastery", "Search marketing", 1400),
            ("Social Media Marketing", "Grow your brand online", 1200),
            ("Email Marketing Course", "Build email campaigns", 900),
            ("Content Marketing Strategy", "Create valuable content", 1100),
            ("Copywriting Masterclass", "Write persuasive copy", 1000),
            ("Branding and Identity", "Build strong brands", 800),
            ("Product Management", "PM skills and strategies", 1600),
            ("Agile and Scrum", "Agile project management", 1200),
            ("Business Analytics", "Data-driven business decisions", 1500),
            ("Financial Analysis", "Understand company finances", 1300),
            ("Entrepreneurship Essentials", "Start your own business", 1800),
            ("Startup Fundraising", "Raise capital for your startup", 900),
            ("Business Plan Writing", "Create winning business plans", 700),
            ("Leadership and Management", "Lead effective teams", 1400),
            ("Negotiation Skills", "Win-win negotiations", 800),
            ("Public Speaking", "Confident presentations", 600),
            ("Sales Techniques", "Close more deals", 1000),
            ("Customer Service Excellence", "Deliver great service", 700),
            ("E-commerce Business", "Build online stores", 1600),
            ("Amazon FBA Course", "Sell on Amazon", 1300),
            ("Dropshipping Business", "Start without inventory", 1100),
            ("Affiliate Marketing", "Earn commissions online", 900),
            ("Passive Income Strategies", "Build income streams", 1200),
            ("Real Estate Investing", "Property investment strategies", 1400),
            ("Stock Market Investing", "Build investment portfolio", 1600),
            ("Cryptocurrency Trading", "Trade digital currencies", 1100),
            ("Personal Finance Management", "Manage your money", 800),
            ("Productivity and Time Management", "Get more done", 600),
            ("Remote Work Mastery", "Thrive working remotely", 500)
        };

        // Generate 200 courses
        var allCourses = new[]
        {
            (webDevCourses, webDev, 60),
            (programmingCourses, programming, 60),
            (mobileCourses, mobile, 30),
            (dataScienceCourses, dataScience, 30),
            (businessCourses, business, 20)
        };

        var levels = new[] { "BEGINNER", "INTERMEDIATE", "ADVANCED", "ALL_LEVELS" };
        var languages = new[] { "en-US", "en-US", "en-US", "en-GB" }; // 75% en-US
        
        // Real Udemy course images to cycle through
        var imageUrls = new[]
        {
            "https://img-c.udemycdn.com/course/480x270/806922_6310_3.jpg",
            "https://img-c.udemycdn.com/course/480x270/461160_8d87_8.jpg",
            "https://img-c.udemycdn.com/course/480x270/1672410_9ff1_5.jpg",
            "https://img-c.udemycdn.com/course/480x270/1386294_cf10_3.jpg",
            "https://img-c.udemycdn.com/course/480x270/2153774_bef0_5.jpg",
            "https://img-c.udemycdn.com/course/480x270/1466612_bead_3.jpg",
            "https://img-c.udemycdn.com/course/480x270/1247828_32bb.jpg",
            "https://img-c.udemycdn.com/course/480x270/1010586_b622_3.jpg",
            "https://img-c.udemycdn.com/course/480x270/833442_b26e_5.jpg",
            "https://img-c.udemycdn.com/course/480x270/405282_27d2_3.jpg",
            "https://img-c.udemycdn.com/course/480x270/1455016_0b2d_2.jpg",
            "https://img-c.udemycdn.com/course/480x270/970600_68be_4.jpg",
            "https://img-c.udemycdn.com/course/480x270/3873464_403c_3.jpg",
            "https://img-c.udemycdn.com/course/480x270/1286908_1773_6.jpg",
            "https://img-c.udemycdn.com/course/480x270/4471614_361e_8.jpg",
            "https://img-c.udemycdn.com/course/480x270/289230_1056_16.jpg"
        };
        
        int courseIndex = 0;
        foreach (var (courseTemplates, category, count) in allCourses)
        {
            for (int i = 0; i < count && courseIndex < 200; i++)
            {
                var template = courseTemplates[i % courseTemplates.Length];
                var price = 49.99m + (decimal)(random.Next(0, 15) * 10);
                var rating = 3.5 + random.NextDouble() * 1.5;
                var enrollments = random.Next(5000, 50000);
                var duration = template.Item3 + random.Next(-200, 300);
                
                courses.Add(new Core.Entities.Course
                {
                    Title = template.Item1,
                    Description = $"{template.Item2}. Comprehensive course covering all essential topics with hands-on projects, real-world examples, and best practices. Perfect for both beginners and experienced developers looking to expand their skillset.",
                    ShortDescription = template.Item2,
                    InstructorId = null,
                    CategoryId = category.Id,
                    Price = price,
                    Level = levels[random.Next(levels.Length)],
                    Duration = Math.Max(300, duration),
                    Rating = Math.Round(rating, 2),
                    ReviewCount = (int)(enrollments * 0.15),
                    ImageUrl = imageUrls[courseIndex % imageUrls.Length], // Cycle through real images
                    EnrollmentCount = enrollments,
                    Language = languages[random.Next(languages.Length)],
                    IsFeatured = random.Next(0, 4) == 0, // 25% featured
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 365)),
                    UpdatedAt = DateTime.UtcNow.AddDays(-random.Next(0, 30))
                });
                
                courseIndex++;
            }
        }

        return courses;
    }
}
