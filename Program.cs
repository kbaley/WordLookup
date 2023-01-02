var syncWordLists = args.Length == 1 && args[0].ToLower() == "sync";
var trimWordList = args.Length == 1 && args[0].ToLower() == "trim";
var doAnswerSync = args.Length == 1 && args[0].ToLower() == "answers";
var syncAnswers = (args.Length == 3 && args[2].ToLower() == "sync") || doAnswerSync;
var generateList = args.Length == 2 || syncAnswers;

if (!syncWordLists && !trimWordList && !generateList && !doAnswerSync)
{
    Console.WriteLine("Usage: WordLookup [command]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  --sync                             Add/remove words from the word list");
    Console.WriteLine("  --trim                             Trim words from the word list that don't follow Spelling Bee rules");
    Console.WriteLine("  <letter list> <center letter>      Find all words that contain only letters in <letter list> AND the center letter");
    Console.WriteLine("     append 'sync' to the previous command to compare the results with known answers");
    return;
}

// Contains all words from the macOS /usr/share/dict/words file where:
//   - Length is 4 or more
//   - Number of distinct letters is 7 or less
// It's also regularly augmented with new words that the NYT accepts and has some removed that
// the NYT doesn't accept
var words = File.ReadAllLines("words");

// Trims the word list to ones that contain only 7 or fewer unique letters
if (trimWordList)
{
    var sevenLetters = words
        .Where(w => w.Length >= 4)
        .Where(w => w.ToLower().Distinct().Count() <= 7);
    File.WriteAllLines("words", sevenLetters);
    return;
}

// Adds all words from the "toBeAdded" file to the word list
// Removes all words from the "toBeRemoved" file from the word list
if (syncWordLists)
{
    Console.WriteLine("syncing");
    var extras = File.ReadAllLines("toBeAdded").Where(w => !words.Contains(w));
    var newList = words.ToList();
    newList.AddRange(extras);
    newList = newList.OrderBy(w => w).ToList();
    var ignored = File.ReadAllLines("toBeRemoved");
    File.WriteAllLines("words", newList.Where(w => !ignored.Contains(w)));
    File.WriteAllLines("toBeAdded", Array.Empty<string>());
    File.WriteAllLines("toBeRemoved", Array.Empty<string>());
    return;
}

char[] letters;
string center;
if (doAnswerSync)
{
    center = File.ReadLines("answers").First();
    letters = File.ReadAllText("answers").ToCharArray().Distinct()
        .Where(l => "qwertyuiopasdfghjklzxcvbnm".Contains(l)).ToArray();

} else {
    letters = args[0].ToCharArray();
    center = args[1];
}


words = words.Where(
    w => w.Contains(center) && w.All(c => letters.Contains(c))).ToArray();
if (!syncAnswers)
{
    foreach (var word in words)
    {
        Console.WriteLine(word);
    }
} else {
    var answers = File.ReadAllLines("answers")
        .Skip(1)
        .Where(l => !string.IsNullOrEmpty(l))
        .Select(l => l.Trim());
    var toBeRemoved = words.Where(w => !answers.Contains(w));
    var toBeAdded = answers.Where(w => !words.Contains(w));
    File.WriteAllLines("toBeRemoved", toBeRemoved);
    File.WriteAllLines("toBeAdded", toBeAdded);
}

Console.WriteLine($"Words: {words.Length}");