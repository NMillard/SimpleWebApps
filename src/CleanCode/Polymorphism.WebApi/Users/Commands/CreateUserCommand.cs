using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polymorphism.WebApi.DataLayer.Repositories;
using Polymorphism.WebApi.Users.Models;

namespace Polymorphism.WebApi.Users.Commands {

    public interface ICreateUserCommand {
        Task<bool> ExecuteAsync(UserInput input);
        Task<bool> ExecuteAsync(ICreateUserInput input);
    }
    
    public class CreateUserCommand : ICreateUserCommand {
        private readonly IUserRepository repository;
        private readonly string[] validUserTypes = { "regular", "premium" };

        public CreateUserCommand(IUserRepository repository) {
            this.repository = repository;
        }
        
        public async Task<bool> ExecuteAsync(UserInput input) {
            User user;
            switch (input.UserType) {
                case "regular":
                    user = new User(input.Username);
                    break;
                case "premium":
                    user = new User(input.Username, new List<Permission> {
                        "PremiumFeature.Read",
                        "PremiumFeature.Create",
                    }) {
                        IsPremium = true,
                    };
                    break;
                case "trial":
                    user = new User(input.Username) {
                        IsOnTrial = true,
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Invalid user type. Must be one of the following {string.Join(" ", validUserTypes)}",
                        nameof(input.UserType)
                    );
            }

            bool result = await repository.CreateAsync(user);

            return result;
        }
        
        
        /*
         * Polymorphic
         * Pros/Cons of this approach
         * + Easier to read
         * + Intent is front-and-center (clean code)
         * + Easily extended upon (Open-Closed Principle)
         * + Adding functionality does not require code modification
         * + Removes multiway branching
         * + Object-oriented
         * - CPU inefficiencies
         * - Junior developers may have trouble understanding the code's flow
         */
        public async Task<bool> ExecuteAsync(ICreateUserInput input) {
            User user = input.CreateUser();
            bool result = await repository.CreateAsync(user);

            return result;
        }
    }
}