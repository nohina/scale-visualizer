public enum NoteName
{
    C = 0,
    CSharp = 1,
    D = 2,
    DSharp = 3,
    E = 4,
    F = 5,
    FSharp = 6,
    G = 7,
    GSharp = 8,
    A = 9,
    ASharp = 10,
    B = 11
}

// 音名の表示ルール
public enum AccidentalDisplay
{
    Auto = 0,
    Sharp = 1,
    Flat = 2,
    TheoreticalSharp = 3,
    TheoreticalFlat = 4
}

// 音名表示の共通ユーティリティ
public static class NoteNameUtility
{
    private static readonly string[] SharpNoteNames =
    {
        "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"
    };

    private static readonly string[] FlatNoteNames =
    {
        "C", "D♭", "D", "E♭", "E", "F", "G♭", "G", "A♭", "A", "B♭", "B"
    };

    private static readonly string[] TheoreticalSharpNoteNames =
    {
        "B#", "C#", "D", "D#", "E", "E#", "F#", "G", "G#", "A", "A#", "B"
    };

    private static readonly string[] TheoreticalFlatNoteNames =
    {
        "C", "D♭", "D", "E♭", "F♭", "F", "G♭", "G", "A♭", "A", "B♭", "C♭"
    };

    public static int Normalize(int note)
    {
        int normalized = note % 12;
        return normalized < 0 ? normalized + 12 : normalized;
    }

    public static string GetDisplayName(int note, AccidentalDisplay display)
    {
        string[] names = GetDisplayNames(display);
        return names[Normalize(note)];
    }

    public static string GetDisplayName(NoteName note, AccidentalDisplay display)
    {
        return GetDisplayName((int)note, display);
    }

    public static string[] GetDisplayNames(AccidentalDisplay display)
    {
        switch (display)
        {
            case AccidentalDisplay.Flat:
                return FlatNoteNames;
            case AccidentalDisplay.TheoreticalSharp:
                return TheoreticalSharpNoteNames;
            case AccidentalDisplay.TheoreticalFlat:
                return TheoreticalFlatNoteNames;
            case AccidentalDisplay.Sharp:
            case AccidentalDisplay.Auto:
            default:
                return SharpNoteNames;
        }
    }

    public static string GetSharpEnumDisplayName(NoteName note)
    {
        return SharpNoteNames[(int)note];
    }
}
