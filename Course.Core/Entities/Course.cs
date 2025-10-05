namespace Course.Core.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? VideoUrl { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int Duration { get; set; } // in minutes
    public string Level { get; set; } = string.Empty; // Beginner, Intermediate, Advanced
    public string Language { get; set; } = string.Empty;
    public bool IsPublished { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public double Rating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public int EnrollmentCount { get; set; } = 0;
    
    // Foreign Keys
    public int CategoryId { get; set; }
    public int? InstructorId { get; set; }
    
    // Navigation Properties
    public Category Category { get; set; } = null!;
    public User? Instructor { get; set; }
}
