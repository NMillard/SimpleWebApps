using System.Collections.Generic;
using System.Threading.Tasks;
using Polymorphism.WebApi.DataLayer.Repositories;
using Polymorphism.WebApi.Users.Models;

namespace Polymorphism.WebApi.Users.Queries {
    public interface IGetAllUsersQuery {
        Task<IEnumerable<User>> ExecuteAsync();
    }

    internal class GetAllUsersQuery : IGetAllUsersQuery {
        private readonly IUserRepository repository;

        public GetAllUsersQuery(IUserRepository repository) {
            this.repository = repository;
        }

        public async Task<IEnumerable<User>> ExecuteAsync() => await repository.GetAllAsync();
    }
}