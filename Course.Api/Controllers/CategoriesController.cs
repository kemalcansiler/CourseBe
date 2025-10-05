using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCategories()
        {
            var categories = new[]
            {
                new { Id = 1, Name = "Programming", Description = "Learn programming languages and frameworks" },
                new { Id = 2, Name = "Web Development", Description = "Frontend and backend web development" },
                new { Id = 3, Name = "Data Science", Description = "Data analysis and machine learning" },
                new { Id = 4, Name = "Mobile Development", Description = "iOS and Android app development" },
                new { Id = 5, Name = "DevOps", Description = "Deployment and infrastructure management" },
                new { Id = 6, Name = "Design", Description = "UI/UX and graphic design" },
                new { Id = 7, Name = "Business", Description = "Business skills and entrepreneurship" },
                new { Id = 8, Name = "Marketing", Description = "Digital marketing and SEO" }
            };

            return Ok(categories);
        }
    }
}
