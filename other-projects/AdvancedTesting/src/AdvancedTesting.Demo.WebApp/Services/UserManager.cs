using System.Data.Common;
using AdvancedTesting.Demo.WebApp.Domain;
using AdvancedTesting.Demo.WebApp.Repositories;

namespace AdvancedTesting.Demo.WebApp.Services;

public class UserManager
{
    private readonly IUserRepository repository;

    public UserManager(IUserRepository repository)
    {
        this.repository = repository;
    }

    public async Task<User?> GetUser(string name)
    {
        return await repository.GetUserByName(name);
    }

    public bool SaveUser(User user)
    {
        try
        {
            repository.SaveUser(user);
            return true;
        }
        catch (DbException e)
        {
            return false;
        }
    }

    public void SaveUsers(params User[] users)
    {
        foreach (User user in users)
        {
            repository.SaveUser(user);
        }
    }

    public void ReadUser(Action<User> action)
    {
        var user = new User { Name = "User" };
        action.Invoke(user);
    }
}