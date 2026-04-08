namespace BtkJuri.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Developer { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation property
    public List<Vote> Votes { get; set; } = new();
}
