using System.ComponentModel.DataAnnotations;

namespace Polymorphism.WebApi.Users.Commands {
    /// <summary>
    /// One class representing multiple different user "types", based on the content of UserType property.
    /// Regular, Premium.
    /// </summary>
    public class UserInput {
        
        /// <summary>
        /// Must be either "regular" or "premium".
        /// </summary>
        [Required] public string UserType { get; set; }
        [Required] public string Username { get; set; }
    }
}