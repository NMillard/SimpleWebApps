using System;
using System.Linq;

namespace ReadableCode {
    public class Conditionals {
        public void Negated() {
            var result = Result.Success;

            if (!result.IsSuccessful) {
                // do things
            }

        }

        public void PoorlyNamedConditional(Person person) {
            // Guard clauses
            if (!person.HasIdentification) throw new InvalidOperationException();
            if (person.Age != 16) throw new InvalidOperationException();
            
            // proceed
        }

        public void ProperlyNamedConditionals(Person person) {
            const int legalDrinkingAge = 16;
            bool notOldEnough = person.Age < legalDrinkingAge;
            bool missingIdentification = !person.HasIdentification;
            bool cannotBuyAlcohol = missingIdentification && notOldEnough; 
            if (cannotBuyAlcohol) throw new InvalidOperationException();
            
            // proceed
        }

        public void CanWithdrawMoney(int amount, string currencyCode) {
            if (amount < 0) throw new InvalidOperationException($"{nameof(amount)} must be larger than 0");
            if (string.IsNullOrEmpty(currencyCode)) throw new ArgumentNullException(nameof(currencyCode));
            if (currencyCode.Length != 3) throw new ArgumentException("Must be exactly 3 characters");
            if (currencyCode.Any(c => !char.IsLetter(c))) throw new ArgumentException("Can only contain letters");
            
            // process
        }
    }

    public sealed class CurrencyCode {
        private readonly string code;

        public CurrencyCode(string code) {
            if (string.IsNullOrEmpty(code)) throw new ArgumentNullException(nameof(code));
            if (code.Length != 3) throw new ArgumentException("Must be exactly 3 characters");
            if (code.Any(c => !char.IsLetter(c))) throw new ArgumentException("Can only contain letters");
            this.code = code;
        }
    }

    public class Person {
        public int Age { get; set; }
        public bool HasIdentification { get; set; }
    }

    public class Result {
        public bool IsSuccessful { get; private set; }

        public static Result Success => new() { IsSuccessful = true };
        public static Result Failed => new();
    }
}