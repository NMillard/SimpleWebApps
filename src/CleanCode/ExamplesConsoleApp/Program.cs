using System;
using System.Linq;

/*
 * Code walk-thru here: https://www.youtube.com/watch?v=goVjlWLY9NQ
 * 
 * Quick and easy way to write simple, clean code.
 * 
 * Each example shows the verbose/"bad" way, and
 * a corresponding clean modern way.
 * 
 * Writing clean code is largely about reducing the
 * noise-to-signal ratio. You want to convey intent
 * and concepts as clearly as possible.
 */

void UnnecessaryBooleanChecks(bool hasPermissions) {
    
    // verbose
    if (hasPermissions == true) {
        // Do whatever
    }
    
    // clean
    if (hasPermissions) {
        // Do whatever
    }
}

void VerboseBooleanAssignment(string[] currentPermissions) {
    
    bool hasPermissionsToCreateFile;
    
    // verbose
    if (currentPermissions.Contains("files.create")) {
        hasPermissionsToCreateFile = true;
    } else {
        hasPermissionsToCreateFile = false;
    }
    
    // clean
    hasPermissionsToCreateFile = currentPermissions.Contains("files.create");
}

void ConditionalAssignment(bool operationSucceeded) {
    var message = string.Empty;
    
    // verbose
    if (operationSucceeded) {
        message = "the operation succeeded";
    } else {
        message = "the operation failed";
    }
    
    // clean       condition                if true                  if false
    message = operationSucceeded ? "the operation succeeded" : "the operation failed";
}

bool ExplainConditionalLogic(int age) {
    // verbose
    if (age >= 16) {
        return true;
    } else {
        return false;
    }
    
    // clean
    bool isOldEnoughToBuyAlcohol = age >= 16;
    return isOldEnoughToBuyAlcohol;
}

void DefaultValueAssignment(string? message) {
    // verbose
    if (message is null) {
        message = "default message";
    }
    
    // clean
    //    null-coalescing assignment
    //        |
    message ??= "default message";
}

void ConditionalMemberAccess(string? title) {
    bool isAllowedEntrance = false;

    // verbose
    if (!string.IsNullOrEmpty(title) && title.Contains("senior")) {
        isAllowedEntrance = true;
    }

    // clean
    //           null conditional operator     null-coalescing
    //                       |                     |
    isAllowedEntrance = title?.Contains("senior") ?? false;
    
    // or
    isAllowedEntrance = !string.IsNullOrEmpty(title) && title.Contains("senior");
}

void CascadingNullCoalescing(bool? value1, bool? value2, bool? value3) {
    // verbose
    bool anyValue = value1.HasValue && (bool) value1 ||
                    value2.HasValue && (bool) value2 ||
                    value3.HasValue && (bool) value3;

    // clean
    anyValue = value1 ??
               value2 ??
               value3 ??
               false;
}

bool FailFastWithGuardClauses(string? password) {
    // verbose
    bool hasValue = !string.IsNullOrEmpty(password);
    if (hasValue) {
        bool hasSpecialCharacter = password.Any(character => !char.IsLetterOrDigit(character));
        if (hasSpecialCharacter) {
            bool longEnough = password.Length >= 10;
            if (longEnough) {
                return true;
            } else {
                throw new ArgumentException("Must be at least 10 characters");
            }
        } else {
            throw new ArgumentException("Must contain at least one special character");
        }
    } else {
        throw new ArgumentException("Password must have a value", nameof(password));
    }
    
    // clean
    if (string.IsNullOrEmpty(password)) throw new ArgumentException("Cannot be null or empty");
    if (password.Any(character => !char.IsLetterOrDigit(character))) throw new ArgumentException("Must contain at least one special character");
    if (password.Length >= 10) throw new ArgumentException("Must be at least 10 characters");

    return true;
}
