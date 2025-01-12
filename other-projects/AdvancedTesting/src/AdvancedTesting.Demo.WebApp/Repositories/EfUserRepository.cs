using AdvancedTesting.Demo.WebApp.DataAccess;
using AdvancedTesting.Demo.WebApp.Domain;
using Microsoft.EntityFrameworkCore;

namespace AdvancedTesting.Demo.WebApp.Repositories;

public class EfUserRepository(UserDbContext context) : IUserRepository
{
    public Task<User> GetUserByName(string name)
    {
        throw new NotImplementedException();
    }

    public void SaveUser(User user)
    {
        try
        {
            context.Users.Add(user);
            context.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            // do something
            throw;
        }
    }
}