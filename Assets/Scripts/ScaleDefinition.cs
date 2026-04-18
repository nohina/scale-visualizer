using System;
using System.Collections.Generic;
using UnityEngine;

// ルート音とスケール種別から構成音を決定する設定データ
[Serializable]
public class ScaleDefinition
{
    private static readonly Dictionary<ScalePreset, ScaleMetadata> ScaleMetadataMap = new Dictionary<ScalePreset, ScaleMetadata>
    {
        { ScalePreset.Major, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 9, 11 }, "メジャースケール", "Major Scale", new[] { "1", "2", "3", "4", "5", "6", "7" }) },
        { ScalePreset.NaturalMinor, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 8, 10 }, "ナチュラルマイナースケール", "Natural Minor Scale", new[] { "1", "2", "♭3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.HarmonicMinor, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 8, 11 }, "ハーモニックマイナースケール", "Harmonic Minor Scale", new[] { "1", "2", "♭3", "4", "5", "♭6", "7" }) },
        { ScalePreset.MelodicMinor, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 9, 11 }, "メロディックマイナースケール", "Melodic Minor Scale", new[] { "1", "2", "♭3", "4", "5", "6", "7" }) },
        { ScalePreset.MajorPentatonic, new ScaleMetadata(new[] { 0, 2, 4, 7, 9 }, "メジャーペンタトニックスケール", "Major Pentatonic Scale", new[] { "1", "2", "3", "5", "6" }) },
        { ScalePreset.MinorPentatonic, new ScaleMetadata(new[] { 0, 3, 5, 7, 10 }, "マイナーペンタトニックスケール", "Minor Pentatonic Scale", new[] { "1", "♭3", "4", "5", "♭7" }) },
        { ScalePreset.Blues, new ScaleMetadata(new[] { 0, 2, 3, 4, 7, 9 }, "ブルーススケール", "Blues Scale", new[] { "1", "2", "♭3", "3", "5", "6" }) },
        { ScalePreset.MinorBlues, new ScaleMetadata(new[] { 0, 3, 5, 6, 7, 10 }, "マイナーブルーススケール", "Minor Blues Scale", new[] { "1", "♭3", "4", "♭5", "5", "♭7" }) },
        { ScalePreset.Ionian, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 9, 11 }, "イオニアンスケール", "Ionian Scale", new[] { "1", "2", "3", "4", "5", "6", "7" }) },
        { ScalePreset.Dorian, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 9, 10 }, "ドリアンスケール", "Dorian Scale", new[] { "1", "2", "♭3", "4", "5", "6", "♭7" }) },
        { ScalePreset.Phrygian, new ScaleMetadata(new[] { 0, 1, 3, 5, 7, 8, 10 }, "フリジアンスケール", "Phrygian Scale", new[] { "1", "♭2", "♭3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.Lydian, new ScaleMetadata(new[] { 0, 2, 4, 6, 7, 9, 11 }, "リディアンスケール", "Lydian Scale", new[] { "1", "2", "3", "#4", "5", "6", "7" }) },
        { ScalePreset.Mixolydian, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 9, 10 }, "ミクソリディアンスケール", "Mixolydian Scale", new[] { "1", "2", "3", "4", "5", "6", "♭7" }) },
        { ScalePreset.Aeolian, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 8, 10 }, "エオリアンスケール", "Aeolian Scale", new[] { "1", "2", "♭3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.Locrian, new ScaleMetadata(new[] { 0, 1, 3, 5, 6, 8, 10 }, "ロクリアンスケール", "Locrian Scale", new[] { "1", "♭2", "♭3", "4", "♭5", "♭6", "♭7" }) },
        { ScalePreset.Altered, new ScaleMetadata(new[] { 0, 1, 3, 4, 6, 8, 10 }, "オルタードスケール", "Altered Scale", new[] { "1", "♭2", "#2", "3", "♭5", "♭6", "♭7" }) },
        { ScalePreset.WholeTone, new ScaleMetadata(new[] { 0, 2, 4, 6, 8, 10 }, "ホールトーンスケール", "Whole Tone Scale", new[] { "1", "2", "3", "#4", "#5", "♭7" }) },
        { ScalePreset.HalfWholeTone, new ScaleMetadata(new[] { 0, 1, 3, 4, 6, 7, 9, 10 }, "ハーフホールトーンスケール", "Half Whole Tone Scale", new[] { "1", "♭2", "#2", "3", "#4", "5", "6", "♭7" }) },
        { ScalePreset.Diminished, new ScaleMetadata(new[] { 0, 2, 3, 5, 6, 8, 9, 11 }, "ディミニッシュスケール", "Diminished Scale", new[] { "1", "2", "♭3", "4", "♭5", "♭6", "6", "7" }) },
        { ScalePreset.LydianDominant, new ScaleMetadata(new[] { 0, 2, 4, 6, 7, 9, 10 }, "リディアンドミナントスケール", "Lydian Dominant Scale", new[] { "1", "2", "3", "#4", "5", "6", "♭7" }) },
        { ScalePreset.Spanish8Note, new ScaleMetadata(new[] { 0, 1, 3, 4, 5, 6, 8, 10 }, "スパニッシュ8ノートスケール", "Spanish 8 Note Scale", new[] { "1", "♭2", "#2", "3", "4", "#4", "♭6", "♭7" }) },
        { ScalePreset.Arabic, new ScaleMetadata(new[] { 0, 2, 4, 5, 6, 8, 10 }, "アラビックスケール", "Arabic Scale", new[] { "1", "2", "3", "4", "♭5", "♭6", "♭7" }) },
        { ScalePreset.Hungarian, new ScaleMetadata(new[] { 0, 2, 3, 6, 7, 8, 11 }, "ハンガリアンスケール", "Hungarian Scale", new[] { "1", "2", "♭3", "#4", "5", "♭6", "7" }) },
        { ScalePreset.Gypsy, new ScaleMetadata(new[] { 0, 1, 4, 5, 7, 8, 10 }, "ジプシースケール", "Gypsy Scale", new[] { "1", "♭2", "3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.Hindu, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 8, 10 }, "ヒンズースケール", "Hindu Scale", new[] { "1", "2", "3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.Scottish, new ScaleMetadata(new[] { 0, 2, 4, 7, 9 }, "スコティッシュスケール", "Scottish Scale", new[] { "1", "2", "3", "5", "6" }) },
        { ScalePreset.YonanukiMajor, new ScaleMetadata(new[] { 0, 2, 4, 7, 9 }, "ヨナ抜き長音階", "Yonanuki Major Scale", new[] { "1", "2", "3", "5", "6" }) },
        { ScalePreset.YonanukiMinor, new ScaleMetadata(new[] { 0, 3, 5, 7, 10 }, "ヨナ抜き短音階", "Yonanuki Minor Scale", new[] { "1", "♭3", "4", "5", "♭7" }) },
        { ScalePreset.Ryukyu, new ScaleMetadata(new[] { 0, 4, 5, 7, 11 }, "琉球音階", "Ryukyu Scale", new[] { "1", "3", "4", "5", "7" }) },
        { ScalePreset.RagaBhairav, new ScaleMetadata(new[] { 0, 1, 4, 5, 7, 8, 11 }, "ラーガバイラブスケール", "Raga Bhairav Scale", new[] { "1", "♭2", "3", "4", "5", "♭6", "7" }) }
    };

    // ルート音
    [SerializeField] private NoteName rootNote = NoteName.C;
    // スケール種別
    [SerializeField] private ScalePreset scalePreset = ScalePreset.Major;

    public int RootNote
    {
        get { return (int)rootNote; }
    }

    public NoteName RootNoteName
    {
        get { return rootNote; }
    }

    public ScalePreset Preset
    {
        get { return scalePreset; }
    }

    public string PresetDisplayNameJapanese
    {
        get { return CurrentMetadata.DisplayNameJapanese; }
    }

    public string PresetDisplayNameEnglish
    {
        get { return CurrentMetadata.DisplayNameEnglish; }
    }

    public IReadOnlyList<string> DegreeNames
    {
        get { return CurrentMetadata.DegreeNames; }
    }

    // 現在選択されているスケールのインターバル一覧を返す
    public IReadOnlyList<int> Intervals
    {
        get { return CurrentMetadata.Intervals; }
    }

    private ScaleMetadata CurrentMetadata
    {
        get { return GetMetadata(scalePreset); }
    }

    private static ScaleMetadata GetMetadata(ScalePreset preset)
    {
        ScaleMetadata metadata;
        if (ScaleMetadataMap.TryGetValue(preset, out metadata))
        {
            return metadata;
        }

        return ScaleMetadataMap[ScalePreset.Major];
    }

    // スケール構成音を半音番号の集合として返す
    public HashSet<int> GetScaleNotes()
    {
        HashSet<int> notes = new HashSet<int>();
        IReadOnlyList<int> intervals = Intervals;

        for (int i = 0; i < intervals.Count; i++)
        {
            int note = NoteNameUtility.Normalize(RootNote + intervals[i]);
            notes.Add(note);
        }

        return notes;
    }

    // UI からルート音を変更する
    public void SetRootNote(NoteName note)
    {
        rootNote = note;
    }

    // UI からスケール種別を変更する
    public void SetScalePreset(ScalePreset preset)
    {
        scalePreset = preset;
    }

    // スケール種別の日本語名を返す
    public static string GetPresetDisplayNameJapanese(ScalePreset preset)
    {
        return GetMetadata(preset).DisplayNameJapanese;
    }

    // スケール種別の英語名を返す
    public static string GetPresetDisplayNameEnglish(ScalePreset preset)
    {
        return GetMetadata(preset).DisplayNameEnglish;
    }
}

[Serializable]
public class ScaleMetadata
{
    public ScaleMetadata(int[] intervals, string displayNameJapanese, string displayNameEnglish, string[] degreeNames)
    {
        Intervals = intervals;
        DisplayNameJapanese = displayNameJapanese;
        DisplayNameEnglish = displayNameEnglish;
        DegreeNames = degreeNames;
    }

    public int[] Intervals { get; private set; }

    public string DisplayNameJapanese { get; private set; }

    public string DisplayNameEnglish { get; private set; }

    public string[] DegreeNames { get; private set; }
}

// 利用可能なスケール種別一覧
public enum ScalePreset
{
    Major = 0,
    NaturalMinor = 1,
    HarmonicMinor = 2,
    MelodicMinor = 3,
    MajorPentatonic = 4,
    MinorPentatonic = 5,
    Blues = 6,
    MinorBlues = 7,
    Ionian = 8,
    Dorian = 9,
    Phrygian = 10,
    Lydian = 11,
    Mixolydian = 12,
    Aeolian = 13,
    Locrian = 14,
    Altered = 15,
    WholeTone = 16,
    HalfWholeTone = 17,
    Diminished = 18,
    LydianDominant = 19,
    Spanish8Note = 20,
    Arabic = 21,
    Hungarian = 22,
    Gypsy = 23,
    Hindu = 24,
    Scottish = 25,
    YonanukiMajor = 26,
    YonanukiMinor = 27,
    Ryukyu = 28,
    RagaBhairav = 29
}
