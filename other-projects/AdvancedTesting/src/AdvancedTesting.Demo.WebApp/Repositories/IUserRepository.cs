using AdvancedTesting.Demo.WebApp.Domain;

namespace AdvancedTesting.Demo.WebApp.Repositories;

public interface IUserRepository
{
    public Task<User> GetUserByName(string name);

    public void SaveUser(User user);
    
}