var syncWordLists = args.Length == 1 && args[0].ToLower() == "sync";
var trimWordList = args.Length == 1 && args[0].ToLower() == "trim";
var doAnswerSync = args.Length == 1 && args[0].ToLower() == "answers";

if (!syncWordLists && !trimWordList && !doAnswerSync)
{
    Console.WriteLine("Usage: WordLookup [command]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  --sync                             Add/remove words from the word list");
    Console.WriteLine("  --trim                             Trim words from the word list that don't follow Spelling Bee rules");
    Console.WriteLine("  --answers                          Read words from an answers file and compare them to the dictionary");
    Console.WriteLine("  <letter list> <center letter>      Find all words that contain only letters in <letter list> AND the center letter");
    return;
}

// Contains all words from the macOS /usr/share/dict/words file where:
//   - Length is 4 or more
//   - Number of distinct letters is 7 or less
// It's also regularly augmented with new words that the NYT accepts and has some removed that
// the NYT doesn't accept
var words = File.ReadLines("words");

// Trims the word list to ones that contain only 7 or fewer unique letters
if (trimWordList)
{
    TrimWordList(words);
    return;
}

// Adds all words from the "toBeAdded" file to the word list
// Removes all words from the "toBeRemoved" file from the word list
if (syncWordLists)
{
    SyncWordList(words);
    return;
}

if (doAnswerSync)
{
    DoAnswerSync(words);
    return;
}

var wordList = Solve(words, args[0].ToCharArray(), args[1]);
foreach (var word in words)
{
    Console.WriteLine(word);
}
Console.WriteLine($"Words: {words.Count()}");


static void SyncWordList(IEnumerable<string> words) {
    Console.WriteLine("Adding/removing words to dictionary...");
    var extras = File.ReadAllLines("toBeAdded").Where(w => !words.Contains(w));
    var newList = words.ToList();
    newList.AddRange(extras);
    newList = newList.OrderBy(w => w).ToList();
    var ignored = File.ReadAllLines("toBeRemoved");
    File.WriteAllLines("words", newList.Where(w => !ignored.Contains(w)));
    File.WriteAllLines("toBeAdded", Array.Empty<string>());
    File.WriteAllLines("toBeRemoved", Array.Empty<string>());
}

static void TrimWordList(IEnumerable<string> words) {
    var sevenLetters = words
        .Where(w => w.Length >= 4)
        .Where(w => w.ToLower().Distinct().Count() <= 7);
    File.WriteAllLines("words", sevenLetters);
}

static IEnumerable<string> Solve(IEnumerable<string> words, char[] letters, string center) {
    return words.Where(
        w => w.Contains(center) && w.All(c => letters.Contains(c))).ToArray();
}

static void DoAnswerSync(IEnumerable<string> words) {
    // Read the answers
    var answers = File.ReadAllLines("answers")
        .Where(l => !string.IsNullOrEmpty(l))
        .Select(l => l.Trim());

    // Determine the letters by selecting all the distinct characters from the answers
    var letters = string.Join("", answers).ToCharArray().Distinct()
        .Where(l => "qwertyuiopasdfghjklzxcvbnm".Contains(l)).ToArray();

    // Get the center letter by selecting the only character that appears in all answers
    // Note: Is it possible there may be more than one candidate that meets the criteria?
    var first = answers.First().Distinct();
    var proposed = first.Where(c => answers.All(a => a.Contains(c)));
    var center = proposed.First().ToString();
    var wordList = Solve(words, letters, center);

    // Determine which words need to be removed and added to the dictionary
    // based on the answer
    var toBeRemoved = wordList.Where(w => !answers.Contains(w));
    var toBeAdded = answers.Where(w => !wordList.Contains(w));
    File.WriteAllLines("toBeRemoved", toBeRemoved);
    File.WriteAllLines("toBeAdded", toBeAdded);

    // Write out some stats
    Console.Write("New words:\n\t");
    Console.WriteLine(string.Join("\n\t", toBeAdded));
    Console.WriteLine();
    Console.WriteLine($"Center letter: {center}");
    if (proposed.Count() > 1)
    {
        Console.WriteLine($"***** Multiple possible first letters: {string.Join(string.Empty, proposed.ToArray())}");
    }
    Console.WriteLine($"Words to be added: {toBeAdded.Count()}");
    Console.WriteLine($"Words to be removed: {toBeRemoved.Count()}");
    Console.WriteLine($"Answers: {answers.Count()}");
    Console.WriteLine($"Found words: {wordList.Count()}");
    Console.WriteLine();
    Console.Write("Do you want to sync (Y/n)? ");
    var doSync = Console.ReadKey();
    Console.WriteLine();
    if (doSync.KeyChar == 'y' || doSync.Key == ConsoleKey.Enter)
    {
        SyncWordList(words);
    }
    Console.WriteLine("Done");
}