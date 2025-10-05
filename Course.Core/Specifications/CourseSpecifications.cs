using Ardalis.Specification;
using Course.Core.Entities;

namespace Course.Core.Specifications;

public class CourseWithDetailsSpec : Specification<Entities.Course>
{
    public CourseWithDetailsSpec()
    {
        Query
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => c.IsPublished);
    }
}

public class CourseByIdWithDetailsSpec : Specification<Entities.Course>
{
    public CourseByIdWithDetailsSpec(int courseId)
    {
        Query
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => c.Id == courseId && c.IsPublished);
    }
}

public class CourseByCategorySpec : Specification<Entities.Course>
{
    public CourseByCategorySpec(int categoryId)
    {
        Query
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => c.CategoryId == categoryId && c.IsPublished);
    }
}

public class CourseByLevelSpec : Specification<Entities.Course>
{
    public CourseByLevelSpec(string level)
    {
        Query
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => c.Level == level && c.IsPublished);
    }
}

public class CourseBySearchSpec : Specification<Entities.Course>
{
    public CourseBySearchSpec(string searchTerm)
    {
        Query
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => c.IsPublished && 
                       (c.Title.Contains(searchTerm) || 
                        c.Description.Contains(searchTerm) ||
                        c.ShortDescription.Contains(searchTerm)));
    }
}

public class FeaturedCoursesSpec : Specification<Entities.Course>
{
    public FeaturedCoursesSpec()
    {
        Query
            .Include(c => c.Category)
            .Include(c => c.Instructor)
            .Where(c => c.IsPublished && c.IsFeatured)
            .OrderByDescending(c => c.CreatedAt);
    }
}
