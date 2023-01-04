# NYT Spelling Bee Word Lookup

A small console app that looks up words for the New York Times [Spelling Bee game](https://www.nytimes.com/puzzles/spelling-bee). It accepts two parameters:

1) the letters
2) the center letter

E.g. `dotnet run -- alodinm o`

The word list is a work in progress. I started with the macOS word list (from `/usr/shared/dict/words`) and trimmed out all words less than four letters and all words that contain more than seven unique letters. This was done using `dotnet run -- trim`.

After that, there are still a *lot* of words in it that aren't accepted (Zyzzogeton? csardas?) and a lot that _aren't_ in the list that are accepted (e.g. mondo). So after each game, I add the words that should be removed to the `toBeRemoved` file and the words that should be added to the `toBeAdded` file (one word per line). Then I run `dotnet run -- sync sync` to sync this with the master word list.

Note: Yes, using the program is cheating. I built it as an exercise, to see how hard it was. My initial thought was to look up all combinations of letters up to, say, 15 letters long. Then optimize for things like "no more than two of the same letter in a row". Eventually I realized the number of combinations would be in the millions and there are only a few dozen thousand common words in the English language. So it's faster just to look through that list and see if it meets the criteria of whether it contains the required letters. And lo! macOS contains a decent starting point already.

Use the `answers` option to sync the dictionary with a list of known answers for a given day. To do this:

- Dump the answers for a given day into a file called `answers`
- Run: `dotnet run -- answers`

This will compare all the words in the `answers` file with the dictionary and add words missing from the dictionary to `toBeAdded` and words from the dictionary that aren't in the answers to `toBeRemoved`. You then have the option of synchronizing these files with the master dictionary. This is what I did to refine the dictionary over time by reviewing past games.

## Acknowledgements

Thanks to https://nytbee.com/ for providing archival answers for refining the dictionary. I looked at a few sites that had archives and this one provided answers in a way that was easiest to copy and paste from.