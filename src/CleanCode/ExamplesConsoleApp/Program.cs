using System.Linq;

/*
 * Quick and easy way to write simple, clean code.
 * 
 * Each example shows the verbose/"bad" way, and
 * a corresponding clean modern way.
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
    if (age >= 16) {
        return true;
    } else {
        return false;
    }
    
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

    if (!string.IsNullOrEmpty(title) && title.Contains("senior")) {
        isAllowedEntrance = true;
    }

    // clean
    //           null conditional operator     null-coalescing
    //                       |                     |
    isAllowedEntrance = title?.Contains("senior") ?? false;
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
