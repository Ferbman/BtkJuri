using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BtkJuri.Data;

namespace BtkJuri.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Tüm projeleri oylarıyla birlikte döndürür.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _db.Projects
            .Include(p => p.Votes)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Developer,
                p.Description,
                AverageScore = p.Votes.Any() ? Math.Round(p.Votes.Average(v => v.Score), 2) : 0,
                TotalVotes = p.Votes.Count
            })
            .ToListAsync();

        return Ok(projects);
    }

    /// <summary>
    /// Yeni proje ekler.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var project = new Models.Project
        {
            Name = dto.Name,
            Developer = dto.Developer,
            Description = dto.Description
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return Ok(new { project.Id, project.Name, project.Developer, project.Description });
    }

    /// <summary>
    /// Proje siler.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project == null) return NotFound();

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Tüm sonuçları (skorboard) döndürür.
    /// </summary>
    [HttpGet("scoreboard")]
    public async Task<IActionResult> Scoreboard()
    {
        var results = await _db.Projects
            .Include(p => p.Votes)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Developer,
                p.Description,
                AverageScore = p.Votes.Any() ? Math.Round(p.Votes.Average(v => v.Score), 2) : 0,
                TotalVotes = p.Votes.Count
            })
            .OrderByDescending(p => p.AverageScore)
            .ToListAsync();

        return Ok(results);
    }
}

public class CreateProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
