namespace Digital_Rain;

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

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

        int DisplayDiagnostics = 0;
        Stopwatch StopwatchFull = new();
        Stopwatch StopwatchPart = new();
        TimeSpan LastLoopTime = new();
        TimeSpan UpdateAndRenderTime = new();
        TimeSpan LastCompositeTime = new();
        TimeSpan LastWriteOutTime = new();
        TimeSpan SpawnTime = new();
        int LastWriteOutLength = 0;
        string DiagnosticDisplayString;

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

            StopwatchPart.Restart();

            for (int i = 0; i < CurrentTermSize.cols; i++)
            {
                if (Random.Shared.NextSingle() < 0.015f) // Flat 1.5% chance for every column every update. Feels a little slow at small window sizes.
                {
                    // 90% chance that it starts from the top. 10% chance it starts somewhere random in the top half of the window. (Potentially still the top.)
                    if (Random.Shared.NextSingle() < 0.9f)
                    {
                        Raindrops.Add(new(TrailLength, i, HSL_Util.HSLtoRGB(new(Random.Shared.NextDouble() * 360d, 1d, 0.6d)), ActiveRaindropCharCol));
                    }
                    else
                    {
                        int StartingLine = Random.Shared.Next(CurrentTermSize.lines / 2);
                        // The rendering order relies on the fact that items in the list are ordered by age, naturally putting newer, higher raindrops later in the list.
                        // Putting raindrops in with any other order would break the overlapping.
                        // The following code *should* insert the new raindrop at the right-ish point in the list. 
                        // I haven't seen any issues with it, but this is just my first attempt at writing it and it may be wrong.
                        for (int j = 0; j <= Raindrops.Count; j++)
                        {
                            if (j + 1 >= Raindrops.Count)
                            {
                                Raindrops.Insert(0, new(TrailLength, i, HSL_Util.HSLtoRGB(new(Random.Shared.NextDouble() * 360d, 1d, 0.6d)), ActiveRaindropCharCol, StartingLine));
                                break;
                            }
                            if (Raindrops[^(j + 1)].Line > StartingLine)
                            {
                                Raindrops.Insert(Raindrops.Count - (j + 1), new(TrailLength, i, HSL_Util.HSLtoRGB(new(Random.Shared.NextDouble() * 360d, 1d, 0.6d)), ActiveRaindropCharCol, StartingLine));
                                break;
                            }
                        }
                    }
                }
            }

            StopwatchPart.Stop();
            SpawnTime = StopwatchPart.Elapsed;

            StopwatchPart.Restart();

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

            StopwatchPart.Stop();
            UpdateAndRenderTime = StopwatchPart.Elapsed;

            if (DisplayDiagnostics != 0)
            {
                DiagnosticDisplayString = (DisplayDiagnostics == 2 ? $"Raindrop Spawning Time: {SpawnTime.TotalMilliseconds:F3} ms. Update & Render Time: {UpdateAndRenderTime.TotalMilliseconds:F3} ms. Last Composite Time: {LastCompositeTime.TotalMilliseconds:F3} ms. Last Write Time: {LastWriteOutTime.TotalMilliseconds:F3} ms. Last Write Length: {LastWriteOutLength} " : "") + $"Last Loop Time: {LastLoopTime.TotalMilliseconds:F3} ms. Num Raindrops: {Raindrops.Count}";
                bool SuccessfullyWroteToBottomOfScreen = DiagnosticDisplayString.ToRGB_Chars().TryCopyTo(CharBuffer.Slice((CurrentTermSize.lines - 1) * CurrentTermSize.cols));
                if (!SuccessfullyWroteToBottomOfScreen)
                    DiagnosticDisplayString.ToRGB_Chars().TryCopyTo(CharBuffer);
            }

            StopwatchPart.Restart();

            string RenderedFrame = RenderFrameForConsole(CharBuffer, CurrentTermSize);
            LastWriteOutLength = RenderedFrame.Length;

            StopwatchPart.Stop();
            LastCompositeTime = StopwatchPart.Elapsed;

            StopwatchPart.Restart();

            Console.SetCursorPosition(0, 0);
            Console.Write(RenderedFrame);

            StopwatchPart.Stop();
            LastWriteOutTime = StopwatchPart.Elapsed;

            if (Console.KeyAvailable)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Q:
                        Quit = true;
                        continue;
                    case ConsoleKey.D:
                        DisplayDiagnostics = (DisplayDiagnostics + 1) % 3;
                        break;
                    default:
                        break;
                }
            }

            StopwatchFull.Stop();
            LastLoopTime = StopwatchFull.Elapsed;

            Thread.Sleep(100);

            StopwatchFull.Restart();
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