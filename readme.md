# NYT Spelling Bee Word Lookup

A small console app that looks up words for the New York Times [Spelling Bee game](https://www.nytimes.com/puzzles/spelling-bee). It accepts two parameters:

1) the letters
2) the center letter

E.g. `dotnet run -- alodinm o`

The word list is a work in progress. I started with the macOS word list (from `/usr/shared/dict/words`) and trimmed out all words less than four letters and all words that contain more than seven unique letters. This was done using `dotnet run -- trim trim`.

After that, there are still a *lot* of words in it that aren't accepted (Zyzzogeton? csardas?) and a lot that _aren't_ in the list that are accepted (e.g. mondo). So after each game, I add the words that should be removed to the `toBeRemoved` file and the words that should be added to the `toBeAdded` file (one word per line). Then I run `dotnet run -- sync sync` to sync this with the master word list.