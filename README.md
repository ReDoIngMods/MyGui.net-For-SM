# MyGui.NET (0.0.1 - BETA)
MyGui.NET is a rewrite of the original [MyGUI Layout Editor](http://mygui.info/) built using .NET 9, WinForms and [SkiaSharp](https://github.com/mono/SkiaSharp). This version was specifically created for [Scrap Mechanic](https://store.steampowered.com/app/387990/Scrap_Mechanic/) Layout making.

**This project is not affiliated with [MyGUI](http://mygui.info/) in any way, shape or form. It is simply an alternative to it to make Scrap Mechanic modding easier.**

## Why use this Editor instead of the default MyGUI Layout Editor?
- **TONS** more Quality of Life features!
- Way better **User Experience**!
- *(Subjectively)* More readable source code, less of an *OOP hell*.
- Many built in Editors!
- **Less taxing on hardware** (uses event driven rendering, useful when having the game open at the same time)!
- More accurate rendering than the stock editor. (When talking about [Scrap Mechanic](https://store.steampowered.com/app/387990/Scrap_Mechanic/), useless for any other MyGUI app.)
- Pulls *(most)* of the files straight from the game. (Borrowing a few exceptions where the files in the game actually contain incorrect information.)

## Special thanks to
- [Questionable Mark](https://github.com/QuestionableM)
- [crackx02](https://github.com/crackx02)
- [Ben Bingo](https://github.com/Ben-Bingo)
- [The Guild of Scrap Mechanic Modders Discord Server](https://discord.gg/SVEFyus)
- [ReDoIng Mods Discord Server](https://discord.gg/DyUxeyAJRz)
- And the rest of you amazing contributors!

## Used Packages
- [SkiaSharp](https://github.com/mono/SkiaSharp)
- [Cyotek WinForms Color Picker](https://github.com/cyotek/Cyotek.Windows.Forms.ColorPicker)

## Notes
- This is my **very first real .NET project**, the code is **very much reflectant of that**.
- There will be a major rewrite and cleanup with version 1.0.0, anything before that is considered a **BETA** version.

# Known Issues
- Skins with Client area (like Window skins) don't actually put the children into the Client area, but the whole widget itself.
- Text rendering issues:
    - Text clipping being weird when text align is set to centered on any axis.
    - Incorrect sizing when using static font sizes (which shouldn't be used anyway)
    - Small differences in font rendering (should be negligible)
