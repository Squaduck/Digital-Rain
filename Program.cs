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
        RGB_Col[] RaindropTrailPallette =
        //[new(0xff1b8d), new(0xffd900), new(0x1bb3ff)];                                                // Pansexual pride flag. Colors from Wikipedia: #ff1b8d, #ffd900, #1bb3ff
        //[new(0x3aa740), new(0xa8d47a), new(0xffffff), new(0xababab)];                                 // Aromantic pride flag. Colors from Wikipedia: #3aa740, #a8d47a, #ffffff, #ababab, #000000. Pure black omitted.
        //[new(0xa5a5a5), new(0xffffff), new(0x810081)];                                                // Asexual pride flag. Colors from Wikipedia: #000000, #a5a5a5, #ffffff, #810081. Pure black omitted.
        //[new(0x3aa740), new(0xa8d47a), new(0xffffff), new(0x810081)];                                 // aroace pride flag. Previous two mashed together.
        //[new(0xd70071), new(0x9c4e97), new(0x0035aa)];                                                // Bisexual pride flag. Colors from Wikipedia: #d70071, #9c4e97, #0035aa
        //[new(0xffd900), new(0xffd900), new(0xffd900), new(0x7a00ac)];                                 // Intersex pride flag. Colors from Wikipedia: #ffd900, #7a00ac. Yellow repeated to make it more common.
        //[new(0xfff42f), new(0xffffff), new(0x9c59d1), new(0x292929)];                                 // Non-Binary pride flag. Colors from Wikipedia: #fff42f, #ffffff, #9c59d1, #292929. Dark grey included.
        //[new(0x5bcffa), new(0xf5abb9), new(0xffffff), new(0xf5abb9), new(0x5bcffa)];                  // Transgender pride flag. Colors from Wikipedia: #5bcffa, #f5abb9, #ffffff, #f5abb9, #5bcffa. Colors duplicated to make white less common.
        //[new(0xd62900), new(0xff9b55), new(0xffffff), new(0xd462a6), new(0xa50062)];                  // Lesbian pride flag (5-Stripe). Colors from Wikipedia: #d62900, #ff9b55, #ffffff, #d462a6, #a50062
        // Intersex-Inclusive Progressive Pride Flag:
        [new(0xfdd910), new(0x7a00ac), new(0xffffff), new(0xf4b0c9), new(0x7ccde6), new(0x7ccde6), new(0x95540e), new(0xe31a0e), new(0xf28a10), new(0xf0e61f), new(0x79b925), new(0x2857a6), new(0x6d1e81)]; // Colors from Wikipedia: #fdd910, #66308c (#7a00ac used instead. Taken from standalone intersex), #ffffff, #f4b0c9, #7ccde6, #95540e, #000000, #e31a0e, #f28a10, #f0e61f, #79b925, #2857a6, #6d1e81 . Pure black omitted.
        //[new(0x623500), new(0xd66300), new(0xfede63), new(0xfee7b9), new(0xffffff), new(0x545454)];   // Bear pride flag. Colors from Wikipedia: #623500, #d66300, #fede63, #fee7b9, #ffffff, #545454, #000000. Pure black omitted.
        //[new(0x018e71), new(0x99e9c2), new(0xffffff), new(0x7cafe3), new(0x3b1379)];                  // Gay pride flag. Colors from Wikipedia: #018e71, #99e9c2, #ffffff, #7cafe3, #3b1379.
        // You get the point. If you want more, take colors from Hyfetch or something.

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
                Raindrops.Add(new(TrailLength, CurrentTermSize.cols, RaindropTrailPallette[Random.Shared.Next(RaindropTrailPallette.Length)], ActiveRaindropCharCol));

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