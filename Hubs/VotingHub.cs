using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BtkJuri.Data;
using BtkJuri.Models;

namespace BtkJuri.Hubs;

public class VotingHub : Hub
{
    private readonly AppDbContext _db;

    public VotingHub(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Admin tarafından tetiklenir. Tüm bağlı istemcilere projenin oylamaya açıldığını bildirir.
    /// </summary>
    public async Task StartProjectVote(int projectId)
    {
        var project = await _db.Projects.FindAsync(projectId);
        if (project == null) return;

        await Clients.All.SendAsync("VoteStarted", new
        {
            projectId = project.Id,
            projectName = project.Name,
            developer = project.Developer,
            description = project.Description
        });
    }

    /// <summary>
    /// Kullanıcılar tarafından tetiklenir. Veritabanına oy kaydeder.
    /// </summary>
    public async Task SubmitVote(int projectId, int score)
    {
        if (score < 1 || score > 5) return;

        var vote = new Vote
        {
            ProjectId = projectId,
            Score = score,
            VotedAt = DateTime.UtcNow
        };

        _db.Votes.Add(vote);
        await _db.SaveChangesAsync();

        // Anlık ortalama ve oy sayısını admin'e bildir
        var stats = await _db.Votes
            .Where(v => v.ProjectId == projectId)
            .GroupBy(v => v.ProjectId)
            .Select(g => new
            {
                projectId = g.Key,
                average = Math.Round(g.Average(v => v.Score), 2),
                totalVotes = g.Count()
            })
            .FirstOrDefaultAsync();

        if (stats != null)
        {
            await Clients.All.SendAsync("VoteReceived", stats);
        }
    }

    /// <summary>
    /// Admin veya sunucu tarafından tetiklenir. Oylamayı kapatır.
    /// </summary>
    public async Task StopProjectVote()
    {
        await Clients.All.SendAsync("VoteStopped");
    }
}
