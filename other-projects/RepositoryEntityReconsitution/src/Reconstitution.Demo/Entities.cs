namespace Reconstitution.Demo;

public class Post
{
    public Guid Id { get; set; }
    public User Author { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTimeOffset? PublishDate { get; set; }
}

public class User
{
    private readonly List<Claim> claims;

    public User(string username)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
        Id = Guid.NewGuid();
        
        Username = username;
        claims = new List<Claim>();
    }

    public Guid Id { get; }
    public string Username { get; private set; }
    public IEnumerable<Claim> Claims => claims.AsReadOnly();
    
    // Methods to grant/revoke claims
}

public class Claim
{
    private readonly List<User> users;

    public Claim(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        Id = Guid.NewGuid();
        Name = name;

        users = new List<User>();
    }
    
    public Guid Id { get; }
    public string Name { get; }
}