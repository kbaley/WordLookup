if (args.Length != 2)
{
    Console.WriteLine("Please provide letters to check and the center letter");
    return;
}

var letters = args[0].ToCharArray();
// Contains all words from the macOS /usr/share/dict/words file where:
//   - Length is 4 or more
//   - Number of distinct letters is 7 or less
// It's also regularly augmented with new words that the NYT accepts and has some removed that
// the NYT doesn't accept
var words = File.ReadAllLines("words");

// Trims the word list to ones that contain only 7 or fewer unique letters
// Writes the results to "newwords" just in case
if (args[0] == "trim")
{
    var sevenLetters = words
        .Where(w => w.Length >= 4)
        .Where(w => w.ToLower().Distinct().Count() <= 7);
    File.WriteAllLines("newwords", sevenLetters);
    return;
}

// Adds all words from the "extras" file to the word list
// Removes all words from the "ignored" file from the word list
// Writes the results to "newwords" just in case
if (args[0] == "sync")
{
    Console.WriteLine("syncing");
    var extras = File.ReadAllLines("toBeAdded").Where(w => !words.Contains(w));
    var newList = words.ToList();
    newList.AddRange(extras);
    newList = newList.OrderBy(w => w).ToList();
    var ignored = File.ReadAllLines("toBeRemoved");
    File.WriteAllLines("words", newList.Where(w => !ignored.Contains(w)));
    File.WriteAllLines("extras", Array.Empty<string>());
    File.WriteAllLines("ignored", Array.Empty<string>());
    return;
}

var center = args[1];

words = words.Where(
    w => w.Contains(center) && w.All(c => letters.Contains(c))).ToArray();
foreach (var word in words)
{
    Console.WriteLine(word);
}

Console.WriteLine($"Words: {words.Length}");