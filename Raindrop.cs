namespace Digital_Rain;

using System;
using System.Collections.Generic;
using System.Numerics;

public class Raindrop
{
    // A bunch of random characters that exist in the font 'Hack' 
    // I've removed some that were quite small, but I can't be bothered to prune these more. 
    const string RaindropChars = "!#$%&()*+-/0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^abcdefghijklmnopqrstuvwxyz{|}~¡¢£¤¥§©ª«®±µ¶·º»¼½¿ÀÃÄÅÆÇÈÊËÌÎÏÐÑÒÔÕÖ×ØÙÛÜÝÞßàâãäåæçèêëìîïðñòôö÷øùûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƒƠơƤƯưΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϴ϶ЀЁЂЃЄЅІЇЈЉЊЋЌЍЎЏАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюяѐёђѓєѕіїјљњћќѝўџѢѣѲѳҐґҒғҔҕҖҗҘҙҚқҢңҤҥҪҫҬҭҮүҰұҲҳҺһӀӁӂӃӄӇӈӋӌӏӐӑӒӓӔӕӖӗӘәӚӛӜӝӞӟӠӡӢӣӤӥӦӧӨөӪӫӬӭӮӯӰӱӲӳӴӵӶӷӸӹԐԑԚԛԜԝԱԲԳԴԵԶԷԸԹԺԻԼԽԾԿՀՁՂՃՄՅՆՇՈՉՊՋՌՍՎՏՐՑՒՓՔՕՖաբգդեզէըթժիլխծկհձղճմյնշոչպջռսվտրցւփքօֆև։฿აბგდევზთიკლმნოპჟრსტუფქღყშჩცძწჭხჯჰჱჲჳჴჵჶჷჸჹჺẀẁẂẃẄẅẼẽỲỳỸỹ†‡•‣‰‹›‼‽⁅⁆⁇⁈⁉⁋₠₡₢₣₤₥₦₧₨₩₪₫€₭₮₯₰₱₲₳₴₵₷₸₹№™Ω⅐⅑⅓⅔⅕⅖⅗⅘⅙⅚⅛⅜⅝⅞⅟←↑→↓↔↕↖↗↘↙↚↛↜↝↞↟↠↡↢↣↤↥↦↧↨↩↪↫↬↭↮↯↰↱↲↳↴↵↶↷↸↹↺↻↼↽↾↿⇀⇁⇂⇃⇄⇅⇆⇇⇈⇉⇊⇋⇌⇍⇎⇏⇐⇑⇒⇓⇔⇕⇖⇗⇘⇙⇚⇛⇜⇝⇠⇡⇢⇣⇤⇥⇦⇧⇨⇩⇫⇬⇭⇮⇯⇰⇱⇲⇳⇴⇵⇶⇷⇸⇹⇺⇻⇼⇽⇾⇿∀∁∂∃∄∅∆∇∈∉∊∋∌∍∎∏∐∑−∓∕∗∘∙√∛∜∝∞∟∠∣∧∨∩∪∫∬∭∴∵∶∷∸∹∺∻∼∽≁≂≃≄≅≆≇≈≉≊≋≌≍≎≏≐≑≒≓≔≕≖≗≘≙≚≛≜≝≞≟≠≡≢≣≤≥≦≧≨≩≭≮≯≰≱≲≳≴≵≶≷≸≹≺≻≼≽≾≿⊀⊁⊂⊃⊄⊅⊆⊇⊈⊉⊊⊋⊍⊎⊏⊐⊑⊒⊓⊔⊕⊖⊗⊘⊙⊚⊛⊜⊝⊞⊟⊠⊡⊢⊣⊤⊲⊳⊴⊵⊸⋂⋃⋄⋅⋆⋍⋎⋏⋐⋑⋚⋛⋜⋝⋞⋟⋠⋡⋢⋣⋤⋥⋦⋧⋨⋩⋯⌄⌈⌉⌊⌋⌐⌠⌡⎛⎜⎝⎞⎟⎠⎡⎢⎣⎤⎥⎦⎧⎨⎩⎪⎫⎬⎭⎮─━│┃┄┅┆┇┈┉┊┋┏┓┗┛┣┫┳┻╋╍╏═║╒╓╔╕╖╗╘╙╚╛╜╝╞╟╠╡╢╣╤╥╦╧╨╩╪╫╬╭╮╯╰╱╲╳╸╹╺╻▀▁▄▇▊▐░▓■▢▪▮▯▰▱▲△▴▵▶▷▸▹►▻▼▽▾▿◀◁◂◃◄◅◆◇◈◉◊○◌◍◎●◐◑◒◓◔◕◖◗◘◙◚◛◜◝◞◟◠◡◢◣◤◥◦◧◨◩◪◫◬◭◮◯◰◱◲◳◴◵◶◷◸◹◺◻◼◿♪❖❨❩❪❫❬❭❮❯❰❱❲❳❴❵➘➚➜➠➢➤➩➪➫➬➭➮➯➱➲➳➴➵➶➷➸➹➺➻➼➽➾⟂⟅⟆⟜⟠⟦⟧⟨⟩⟪⟫⟵⟶⟷⦇⦈⦗⦘⧫⧺⧻⨀⨯⩪⩫⬅⬆⬇⬈⬉⬊⬋⬌⬍⬖⬗⬘⬙⬚⸘⸟⸢⸣⸤⸥⸮";
    List<RGB_Char> Trail;

    // Both starting from 0
    public int Column { get; private init; }
    public int Line { get; private set; }

    readonly int TrailLength;

    RGB_Col ColDecrement;
    RGB_Col ActiveCharColor;
    RGB_Col TrailStartingColor;

    readonly int MaxTrailCharChanges;

    /* IDEA:(s) for other variables */
    // Background Color (Active and Trail)
    // Trail Char 'Corruption' Chance
    // More advanced trail color 'decrement' (Func<>?)
    // Func<>(s) for color stuff in general. Could allow active color randomization, as an example.

    public Raindrop(int TrailLength, int Column, RGB_Col TrailStartingColor, RGB_Col ActiveCharColor, int StartingLine = 0, int MaxTrailCharChanges = 3)
    {
        this.TrailLength = TrailLength;
        Trail = new(TrailLength);
        this.Column = Column;
        this.ActiveCharColor = ActiveCharColor;
        this.TrailStartingColor = TrailStartingColor;
        this.MaxTrailCharChanges = MaxTrailCharChanges;
        this.Line = StartingLine;

        // Calculate how much to decrement the trail each step.
        // This prevents us from easily changing a character to a different color in the middle of the trail.
        // But I can't think of another way.
        Vector3 StartingCol = new(TrailStartingColor.R, TrailStartingColor.G, TrailStartingColor.B);
        StartingCol /= TrailLength + 1; // +1 extends how long it's going to still have color.
        ColDecrement = new((byte)Math.Ceiling(StartingCol.X), (byte)Math.Ceiling(StartingCol.Y), (byte)Math.Ceiling(StartingCol.Z));

        // Ensure that ColorDecrement is at least 1 when relevant.
        // Shouldn't be necesarry, but I'd rather be safe rather than sorry.
        if (TrailStartingColor.R != 0 && ColDecrement.R == 0)
            ColDecrement.R = 1;
        if (TrailStartingColor.G != 0 && ColDecrement.G == 0)
            ColDecrement.G = 1;
        if (TrailStartingColor.B != 0 && ColDecrement.B == 0)
            ColDecrement.B = 1;
    }

    public bool IsInBounds((int Cols, int Lines) TerminalSize) => !(Column > TerminalSize.Cols || (Line - TrailLength) > TerminalSize.Lines);

    public void Update()
    {
        if (Trail.Count != 0) // Don't operate on a trail that doesn't exist.
        {
            // Potentially randomize some characters
            for (int i = 0; i < MaxTrailCharChanges; i++)
            {
                if (Random.Shared.NextSingle() < 0.15f) // IDEA: Could make this a member variable and set in constructor. That could allow it to either be set as a paramater or by the trail length or something.
                {
                    int IndexToChange = Random.Shared.Next(Trail.Count);
                    RGB_Char CopiedChar = Trail[IndexToChange];
                    CopiedChar.Character = RaindropChars[Random.Shared.Next(RaindropChars.Length)];
                    Trail[IndexToChange] = CopiedChar;
                }
            }

            // Step each trail char down a bit
            for (int i = 0; i < Trail.Count; i++)
            {
                RGB_Char CopiedChar = Trail[i];
                CopiedChar.FG_Col -= ColDecrement;
                Trail[i] = CopiedChar;
            }

            // Make the last char part of the trail
            {
                RGB_Char CopiedChar = Trail[^1];
                CopiedChar.FG_Col = TrailStartingColor;
                Trail[^1] = CopiedChar;
            }

            Line++;
        }

        // Append new 'active' char
        Trail.Add(new() { Character = RaindropChars[Random.Shared.Next(RaindropChars.Length)], BG_Col = new(0, 0, 0), FG_Col = ActiveCharColor });

        // Remove oldest trail char if length is too long.
        if (Trail.Count > TrailLength)
            Trail.RemoveAt(0);
    }

    public void Render(Span<RGB_Char> CharBuffer, (int cols, int lines) TerminalSize)
    {
        // Trail[0] is the oldest part of the trail. Trail[^1] is the newest, and should be at the position indicated by the member variable 'Line'.

        for (int i = 0; i < Trail.Count; i++)
        {
            int PosIndex = (Line - i) * TerminalSize.cols + Column;

            if (PosIndex >= 0 && PosIndex < CharBuffer.Length)
                CharBuffer[PosIndex] = Trail[^(i + 1)];
        }
    }
}