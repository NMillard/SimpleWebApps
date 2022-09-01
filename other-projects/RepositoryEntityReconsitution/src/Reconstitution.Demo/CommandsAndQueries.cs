using Microsoft.EntityFrameworkCore;

namespace Reconstitution.Demo;

internal class GetUserQuery
{
    private readonly AppDbContext context;

    public GetUserQuery(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.context = context;
    }

    public async Task<User?> ExecuteAsync(Guid userId)
        // Claims list will be empty because there's no "Include".
        => await context.Users.SingleOrDefaultAsync(u => u.Id.Equals(userId));
}

internal class GetUsersQuery
{
    private readonly AppDbContext context;

    public GetUsersQuery(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.context = context;
    }
    
    public IAsyncEnumerable<User> ExecuteAsync(int skip = 0, int take = 50)
        => context.Users
            .OrderBy(u => u.Username)
            .Skip(skip)
            .Take(take)
            .Include(u => u.Claims)
            .AsAsyncEnumerable();
}

internal class GetSlimUserQuery
{
    private readonly AppDbContext context;

    public GetSlimUserQuery(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.context = context;
    }
    
    public async Task<User?> ExecuteAsync(Guid userId)
    {
        // Making things awkward by having non-default constructor and private setters
        return await context.Users.Select(u => new User(u.Username))
            .SingleOrDefaultAsync(u => u.Id.Equals(userId));
    }
}

internal class GetPostsQuery
{
    private readonly AppDbContext context;

    public GetPostsQuery(AppDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        this.context = context;
    }
    
    // The client will experience adverse effects when accessing properties that aren't hydrated.
    public IAsyncEnumerable<Post> ExecuteAsync() => context.Posts
        .Select(p => new Post
        {
            Id = p.Id,
            Title = p.Title
        })
        .AsAsyncEnumerable();
}