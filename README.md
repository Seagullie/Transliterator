**English** | [Українською](readme-ukr.md)

<div>
<img src="docs/images/translit-icon-sharp-corners.ico" align="left" width="40px"/>
<h1> Transliterator </h1>
</div>

> 🟢 **Project status**: active<sup>[[?]](https://github.com/Tyrrrz/.github/blob/master/docs/project-status.md)</sup>


Transliteration refers to the method of mapping from one system of writing to another based on phonetic similarity.</br>
With this tool, you type in letters (e.g. a, b, c etc.), which are converted to characters that have similar pronunciation in the target language.
For example, in Ukrainian transliteration, you can type in `vitannja` to get `вітання`.

</br>

This transliterator is an utility that converts user input by providing **system-wide** transliteration using a mapping between, for example, Latin and Cyrillic letters. The program supports different mappings for various languages and has multiple use cases beyond the initial purpose of facilitating typing in foreign languages.

## Use Cases

* Transliterating a snippet of text.
* Typing in foreign language via transliteration.
* [Some languages have two alphabets](https://www.google.com/url?q=https%3A%2F%2Fen.wikipedia.org%2Fwiki%2FDigraphia&sa=D). This tool should help, I think?
* Typing diacritics and accents. For example, to get `õ`, you can type `~o` and it will be converted to `õ`.

## Screenshots (Windows 11)
<img src="docs/images/ui-screenshots/snippet_panel.png">
<img src="docs/images/ui-screenshots/snippet_panel_lorem_ipsum.png">
<img src="docs/images/ui-screenshots/translit_table_view_panel.png">
<img src="docs/images/ui-screenshots/settings_folded.png">
<img src="docs/images/ui-screenshots/settings_unfolded.png">
<img src="docs/images/ui-screenshots/settings_light_theme.png">
<img src="docs/images/ui-screenshots/translit_table_view_panel_light.png">
<img src="docs/images/ui-screenshots/snippet_panel_lorem_ipsum_light.png">


## Getting Started
1. Clone the repo or download installer from [release page](releases).
2. If you cloned the repo, compile and run the project.
3. Once the program boots up, select your language pair by clicking the dropdown in the top right corner and then selecting the table you want (for example, `tableAccentsAndDiacritics` or `tableENG-LAT_UKR-CYR`).

## Transliteration Tables
Transliteration tables are stored in `Transliterator.Core\Resources\TranslitTables` as `.json` files.</br>
Each table is a mapping between input and output characters.
Some mappings do not follow existing transliteration standarts completely. That's due to technical limitations and for ease of use.

## Planned Features

- [ ] Async handling of key presses
- [ ] Table Editor
- [ ] Different transliteration rules depending on position of character in a word (e. g., `ya --> "я"` in beginning of a word and `ia -> "я"` elsewhere)


