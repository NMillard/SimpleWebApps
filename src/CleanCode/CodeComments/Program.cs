/*
 * Comments often increase the noise to signal ratio.
 * It's annoying listening to music with a lot of static noise,
 * and the same goes for reading code with lots of unnecessary comments.
 */
using System;
using System.Collections.Generic;


static void CommentingOnCodeBehavior(ref int number) {
    number++; // increment number by one
}

static void CommentingLanguageSyntax(bool hasAccess) {
    // comments like these assumes the reader don't know the language
    string? message = hasAccess ? "Has access" : "No access"; // Ternary statement
    int messageLength = message?.Length ?? 0; // conditional access and null-coalescing operator
}

static string CommentingOnObviousIntent(int articleStatus) {
    // Check the article status
    // 0 = draft 1 = published
    if (articleStatus == 0) {
        return "Article is in draft mode";
    } else {
        return "Article is published";
    }
    
    // An enum is better suited
    ArticleStatus status = ArticleStatus.Draft;
    if (status == ArticleStatus.Draft) {
        return "Article is in draft mode";
    } else {
        return "Article is published"; 
    }
}

static void CommentingOnConditionals() {
    var age = 18;
    var message = string.Empty;
    
    // must be 18 or older to get driver's license
    message = age >= 18 
        ? "Can apply for driver's license"
        : "Not old enough for driver's license";

    bool canGetDriversLicense = age >= 18;
    message = canGetDriversLicense
        ? "Can apply for driver's license"
        : "Not old enough for driver's license";
}

static void ZombieComments() {
    /*
     * Code gets "zombied" when left in out-commented state.
     * Often because you want to try a different approach but perhaps
     * want to fallback on old approach again... use git instead.
     */
    
    // Check for duplicate numbers
    int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 8, 9, 10 };
    int? duplicateNumber = null;
    
    // commented out because of a bug
    // int currentNumber = numbers[0];
    // for (var i = 0; i < numbers.Length; i++) {
    //     
    //     bool isLastNumber = i + 1 == numbers.Length;
    //     if (isLastNumber) {
    //         duplicateNumber = currentNumber == numbers[i] ? currentNumber : null;
    //         break;
    //     } else if (currentNumber == numbers[i + 1]) {
    //         duplicateNumber = currentNumber;
    //         break;
    //     }
    //
    //     currentNumber = numbers[i + 1];
    // }

    var set = new HashSet<int>();
    foreach (int number in numbers) {
        if (set.Contains(number)) duplicateNumber = number;
        set.Add(number);
    }

    Console.WriteLine(duplicateNumber);
}

static void CommentingOnMatchingBraces() {
    bool condition = true;
    bool secondCondition = true;
    bool lastCondition = true;
    
    if (condition) {
        if (secondCondition) {
            if (lastCondition) {
            } // end inner
        } // end second
    } // end first
}


public enum ArticleStatus {
    Draft,
    Published
}

/*
 * Redundant comments
 * High noise-to-signal ratio.
 */
/// <summary>
/// The post class
/// </summary>
public class RedundantComments {

    /// <summary>
    /// Post's content
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Update content
    /// </summary>
    public void UpdateContent(string newContent) => Content = newContent;
}

public class InsaneDividerComments {
    // ---
    // FIELDS
    // ---
    private readonly string firstName;
    private readonly string lastName;
    private readonly List<string> petNames;

    // ---
    // CONSTRUCTORS
    // ---
    public InsaneDividerComments() {
        petNames = new List<string>();
    }

    public InsaneDividerComments(string firstName, string lastName) {
        this.firstName = firstName;
        this.lastName = lastName;
    }
    
    // ---
    // PROPERTIES
    // ---
    public string Username { get; set; }
    public DateTime BirthDate { get; set; }
    public IEnumerable<string> PetNames => petNames;

    // ---
    // METHODS
    // ---
    public string GetFullName() => $"{firstName} {lastName}";
}


/*
 * Rules of thumb
 * - clean code reduces the need for comments
 * - only document why something exists
 * - don't document how something is done
 * - document not-so-obvious solutions
 */