# Digital Rain
My take on the famous digital rain effect from the matrix.

[![Digital-Rain-Big-Blue-Terminal-Plus.png](https://i.postimg.cc/GtRNHKY3/Digital-Rain-Big-Blue-Terminal-Plus.png)](https://postimg.cc/FdPG6jDw)

[(Old-ish) Youtube video of it running for longer](https://youtu.be/9UXcYOdrRUM)

### Running
If you have dotnet installed, you can clone this repo, cd into it, and run `dotnet run` in your favorite shell.

If that's too clunky, then you can [install it from Nuget](https://www.nuget.org/packages/Squaduck.CSharpMatrix) (Command below) as a global tool and invoke it from anywhere in your favorite shell with the command `csharpmatrix`.
```sh
dotnet tool install -g Squaduck.CSharpMatrix
```

If you don't have dotnet installed, you should download a [release](https://github.com/Squaduck/Digital-Rain/releases/latest) version.

### Keybinds
While the program is running, there are some buttons you can press to do things.
- <kbd>q</kbd> - Quit after next frame.
- <kbd>ctrl</kbd> + <kbd>c</kbd> - Quit ASAP.
- <kbd>d</kbd> - Show time how much time is spent in each part of the main loop. Rotates through 2 modes - simple and detailed. (Also off, if you count that.)

### Font
For the best experience, use the font [Hack](https://github.com/source-foundry/Hack). It's the default in KDE's Konsole.
I chose the characters from the characters that exist in Hack. Other fonts may work, but Hack is guaranteed to work.

> But that GIF up there isn't Hack, whats the deal? 
- Correct, the font in the looping APNG at the top of this README is using the font BigBlueTerminalPlus. If you want it to look like that, see [this](https://gist.github.com/Squaduck/2ead723de969faa928a8f1246b2b2749) gist. 

### Final Notes
This repo is just for funsies, and there is no promise of support. If you have a problem, feel free to reach out, but there is no guarantee of a response.