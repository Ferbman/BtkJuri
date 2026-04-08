using System.ComponentModel.DataAnnotations;

namespace BtkJuri.Models;

public class Vote
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    [Range(1, 5)]
    public int Score { get; set; }

    public DateTime VotedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Project? Project { get; set; }
}
