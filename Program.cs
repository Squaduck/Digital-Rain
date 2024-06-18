namespace Digital_Rain;

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

class Program
{
    public static void Main(string[] args)
    {
        (int cols, int lines) LastTermSize = (-1, -1);

        Span<RGB_Char> CharBuffer = new();

        List<Raindrop> Raindrops = new();

        RGB_Col Black = new(0, 0, 0);
        RGB_Char BlankChar = new() { FG_Col = Black, BG_Col = Black, Character = ' ' };

        Console.Write("\x1b[?1049h");
        Console.CursorVisible = false;

        AppDomain.CurrentDomain.ProcessExit += new EventHandler(PrepForGracefulExit);
        Console.CancelKeyPress += PrepForGracefulExit;

        int TrailLength = 30;
        RGB_Col ActiveRaindropCharCol = new(255, 255, 255);
        //RGB_Col RaindropTrailStartCol = new(0, 255, 0);

        bool Quit = false;
        while (!Quit)
        {
            (int cols, int lines) CurrentTermSize = (Console.BufferWidth, Console.BufferHeight);

            if (CurrentTermSize != LastTermSize)
            {
                // Terminal was resized

                CharBuffer = new(new RGB_Char[CurrentTermSize.cols * CurrentTermSize.lines]);

                LastTermSize = CurrentTermSize;
            }

            CharBuffer.Fill(BlankChar);

            while(Random.Shared.Next(2) == 0)
                Raindrops.Add(new(TrailLength, CurrentTermSize.cols, HSL_Util.HSLtoRGB(new(Random.Shared.NextDouble() * 360d, 1d, 0.6d)), ActiveRaindropCharCol));

            for (int i = 0; i < Raindrops.Count; i++)
            {
                if (Raindrops[i].IsInBounds(CurrentTermSize))
                {
                    Raindrops[i].Update();
                    Raindrops[i].Render(CharBuffer, CurrentTermSize);
                }
                else
                    Raindrops.RemoveAt(i);
            }

            Console.Write(RenderFrameForConsole(CharBuffer, CurrentTermSize));

            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Q:
                        Quit = true;
                        continue;
                    default:
                        break;
                }
            }

            Thread.Sleep(100);
        }

        Console.Write("\x1b[?1049l");
        Console.CursorVisible = true;
    }

    static void PrepForGracefulExit(object? sender, EventArgs e)
    {
        Console.Write("\x1b[?1049l");
        Console.CursorVisible = true;
    }

    static string RenderFrameForConsole(Span<RGB_Char> CharBuffer, (int cols, int lines) TerminalSize)
    {
        StringBuilder sb = new(TerminalSize.lines * TerminalSize.cols);

        RGB_Col CurrentFG_Col;
        RGB_Col CurrentBG_Col;

        //last color sent to the console
        RGB_Col LastFG_Col;
        RGB_Col LastBG_Col;

        sb.Append($"\x1b[38;2;{CharBuffer[0].FG_Col.R};{CharBuffer[0].FG_Col.G};{CharBuffer[0].FG_Col.B};" + //FG
                       $"48;2;{CharBuffer[0].BG_Col.R};{CharBuffer[0].BG_Col.G};{CharBuffer[0].BG_Col.B}m"); //BG
        sb.Append(CharBuffer[0].Character);

        LastFG_Col = new(CharBuffer[0].FG_Col.R, CharBuffer[0].FG_Col.G, CharBuffer[0].FG_Col.B);
        LastBG_Col = new(CharBuffer[0].BG_Col.R, CharBuffer[0].BG_Col.G, CharBuffer[0].BG_Col.B);

        for (int i = 1; i < (TerminalSize.lines * TerminalSize.cols); i++)
        {
            int Column = i % TerminalSize.cols; // Position of the character in the final console window
            int Line = (i - Column) / TerminalSize.cols;

            if (Column == 0) // If we are on the first column, we need a newline first.
                sb.AppendLine();

            CurrentFG_Col = new(CharBuffer[i].FG_Col.R, CharBuffer[i].FG_Col.G, CharBuffer[i].FG_Col.B);
            CurrentBG_Col = new(CharBuffer[i].BG_Col.R, CharBuffer[i].BG_Col.G, CharBuffer[i].BG_Col.B);

            if (CurrentFG_Col == CurrentBG_Col) // It's just a solid color
            {
                if (CurrentBG_Col != LastBG_Col)
                {
                    sb.Append($"\x1b[48;2;{CharBuffer[i].BG_Col.R};{CharBuffer[i].BG_Col.G};{CharBuffer[i].BG_Col.B}m"); // BG

                    LastBG_Col = CurrentBG_Col;
                }
                sb.Append(' ');
            }
            else // Not just a solid color
            {
                if (CurrentFG_Col != LastFG_Col && CurrentBG_Col != LastBG_Col) // If both are different, then there isn't any point sending two separate ANSI sequences
                {

                    sb.Append($"\x1b[38;2;{CharBuffer[i].FG_Col.R};{CharBuffer[i].FG_Col.G};{CharBuffer[i].FG_Col.B};" + // FG
                                   $"48;2;{CharBuffer[i].BG_Col.R};{CharBuffer[i].BG_Col.G};{CharBuffer[i].BG_Col.B}m"); // BG

                    LastFG_Col = CurrentFG_Col;
                    LastBG_Col = CurrentBG_Col;
                }
                else
                {
                    if (CurrentFG_Col != LastFG_Col)
                    {
                        sb.Append($"\x1b[38;2;{CharBuffer[i].FG_Col.R};{CharBuffer[i].FG_Col.G};{CharBuffer[i].FG_Col.B}m"); // FG

                        LastFG_Col = CurrentFG_Col;
                    }
                    if (CurrentBG_Col != LastBG_Col)
                    {
                        sb.Append($"\x1b[48;2;{CharBuffer[i].BG_Col.R};{CharBuffer[i].BG_Col.G};{CharBuffer[i].BG_Col.B}m"); // BG

                        LastBG_Col = CurrentBG_Col;
                    }
                }
                sb.Append(CharBuffer[i].Character);

            }
        }
        return sb.ToString();
    }
}