using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Mjukvare.Cqrs.WebApi.BackgroundServices;
using Mjukvare.Cqrs.WebApi.DataLayer;
using Mjukvare.Cqrs.WebApi.Domain;
using Mjukvare.Cqrs.WebApi.Domain.ReadModels;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ReadUpdateNotifier>()
    .AddSingleton<MaterialViewUserCheckinUpdaterChannel>();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    string? connectionString = builder.Configuration.GetConnectionString("Sql");
    options.UseNpgsql(connectionString);

    options.LogTo(Console.WriteLine, new EventId[RelationalEventId.CommandExecuted.Id]);
    options.EnableSensitiveDataLogging();
});

builder.Services
    .AddHostedService<ChannelMaterializedViewUpdaterBackgroundService>();

WebApplication app = builder.Build();

app.MapOpenApi();

app.UseHttpsRedirection();

app.MapPost("/users/create", async (AppDbContext context, CreateUserRequest request) =>
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Results.Created($"/users/{user.Id}", new { UserId = user.Id });
    })
    .WithName("CreateUser");

app.MapPost("/users/{id:guid}/checkin",
    async (AppDbContext context, ReadUpdateNotifier notifier, MaterialViewUserCheckinUpdaterChannel channel, Guid id, CreateCheckinRequest request) =>
    {
        User? user = await context.Users
            .Include(u => u.Checkins)
            .SingleOrDefaultAsync(u => u.Id == id);
        if (user is null) return Results.BadRequest();

        var checkin = new Checkin
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            Created = DateTimeOffset.UtcNow
        };
        user.Checkins.Add(checkin);

        context.Attach(user).State = EntityState.Unchanged;
        context.Attach(checkin).State = EntityState.Added;
        await context.SaveChangesAsync();

        notifier.NotifyCheckinAdded();
        await channel.NotifyMaterializedViewUpdate(user.Id);

        return Results.Ok();
    }).WithName("AddUserCheckin");

app.MapGet("/users/{id:guid}/checkins", (AppDbContext context, Guid id) =>
{
    // Reading the materialized view model manually by joining and
    // aggregating data ad-hoc.
    var result = context.Users
        .Include(u => u.Checkins)
        .Select(u => new UserCheckinDisplay
        {
            UserId = u.Id,
            Username = u.Username,
            TotalCheckins = u.Checkins.Count,
            LatestCheckinDate = u.Checkins.OrderByDescending(c => c.Created).Select(c => c.Created).FirstOrDefault(),
            Checkins = u.Checkins.Select(c => new CheckinDisplay
            {
                CheckinId = c.Id,
                Text = c.Text,
                Created = c.Created
            }).ToList()
        })
        .AsNoTracking()
        .SingleOrDefault(u => u.UserId == id);
    return Results.Ok(result);
});

app.MapGet("/users/{id:guid}/materialized/checkins", async (AppDbContext context, Guid id) =>
{
    UserCheckinDisplay? result = await context
        .UserCheckinDisplays
        .AsNoTracking()
        .SingleOrDefaultAsync(u => u.UserId == id);
    return Results.Ok(result);
});

app.Run();


public sealed record CreateUserRequest(string Username);

public sealed record CreateCheckinRequest(string Text);