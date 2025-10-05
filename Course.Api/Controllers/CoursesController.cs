using Course.Application.DTOs;
using Course.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Course.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<CourseDto>>> GetCourses([FromQuery] CourseListRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _courseService.GetCoursesAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDetailDto>> GetCourseById(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _courseService.GetCourseByIdAsync(id, cancellationToken);
            if (result == null)
            {
                return NotFound(new { message = "Course not found" });
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IReadOnlyList<CourseDto>>> GetFeaturedCourses(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _courseService.GetFeaturedCoursesAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _courseService.GetCategoriesAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("filters")]
    public async Task<ActionResult<IReadOnlyList<FilterOptionDto>>> GetFilters(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _courseService.GetFiltersAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}