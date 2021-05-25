using System.Collections.Generic;
using Polymorphism.WebApi.Users.Models;

namespace Polymorphism.WebApi.Users.Commands {
    public interface ICreateUserInput {
        User CreateUser();
    }

    public abstract record CreateUserInputBase : ICreateUserInput {
        public string Username { get; init; }
        public abstract User CreateUser();
    }

    public record CreateRegularUserInput : CreateUserInputBase {
        public override User CreateUser() => new(Username);
    }

    public record CreatePremiumUserInput : CreateUserInputBase {
        public override User CreateUser() => new(Username, new List<Permission> {
                "PremiumFeature.Read",
                "PremiumFeature.Create",
            }
        ) {
            IsPremium = true,
        };
    }
}