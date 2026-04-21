using System;
using System.Collections.Generic;
using UnityEngine;

// ルート音とスケール種別から構成音を決定する設定データ
[Serializable]
public class ScaleDefinition
{
    private static readonly Dictionary<ScalePreset, ScaleMetadata> ScaleMetadataMap = new Dictionary<ScalePreset, ScaleMetadata>
    {
        { ScalePreset.Major, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 9, 11 }, "メジャー・スケール", "Major Scale", new[] { "1", "2", "3", "4", "5", "6", "7" }) },
        { ScalePreset.NaturalMinor, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 8, 10 }, "ナチュラルマイナー・スケール", "Natural Minor Scale", new[] { "1", "2", "♭3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.HarmonicMinor, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 8, 11 }, "ハーモニックマイナー・スケール", "Harmonic Minor Scale", new[] { "1", "2", "♭3", "4", "5", "♭6", "7" }) },
        { ScalePreset.MelodicMinor, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 9, 11 }, "メロディックマイナー・スケール", "Melodic Minor Scale", new[] { "1", "2", "♭3", "4", "5", "6", "7" }) },
        { ScalePreset.MajorPentatonic, new ScaleMetadata(new[] { 0, 2, 4, 7, 9 }, "メジャーペンタトニック・スケール", "Major Pentatonic Scale", new[] { "1", "2", "3", "5", "6" }) },
        { ScalePreset.MinorPentatonic, new ScaleMetadata(new[] { 0, 3, 5, 7, 10 }, "マイナーペンタトニック・スケール", "Minor Pentatonic Scale", new[] { "1", "♭3", "4", "5", "♭7" }) },
        { ScalePreset.Blues, new ScaleMetadata(new[] { 0, 2, 3, 4, 7, 9 }, "ブルース・スケール", "Blues Scale", new[] { "1", "2", "♭3", "3", "5", "6" }) },
        { ScalePreset.MinorBlues, new ScaleMetadata(new[] { 0, 3, 5, 6, 7, 10 }, "マイナーブルース・スケール", "Minor Blues Scale", new[] { "1", "♭3", "4", "♭5", "5", "♭7" }) },
        { ScalePreset.Ionian, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 9, 11 }, "イオニアン・スケール", "Ionian Scale", new[] { "1", "2", "3", "4", "5", "6", "7" }) },
        { ScalePreset.Dorian, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 9, 10 }, "ドリアン・スケール", "Dorian Scale", new[] { "1", "2", "♭3", "4", "5", "6", "♭7" }) },
        { ScalePreset.Phrygian, new ScaleMetadata(new[] { 0, 1, 3, 5, 7, 8, 10 }, "フリジアン・スケール", "Phrygian Scale", new[] { "1", "♭2", "♭3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.Lydian, new ScaleMetadata(new[] { 0, 2, 4, 6, 7, 9, 11 }, "リディアン・スケール", "Lydian Scale", new[] { "1", "2", "3", "#4", "5", "6", "7" }) },
        { ScalePreset.Mixolydian, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 9, 10 }, "ミクソリディアン・スケール", "Mixolydian Scale", new[] { "1", "2", "3", "4", "5", "6", "♭7" }) },
        { ScalePreset.Aeolian, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 8, 10 }, "エオリアン・スケール", "Aeolian Scale", new[] { "1", "2", "♭3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.Locrian, new ScaleMetadata(new[] { 0, 1, 3, 5, 6, 8, 10 }, "ロクリアン・スケール", "Locrian Scale", new[] { "1", "♭2", "♭3", "4", "♭5", "♭6", "♭7" }) },
        { ScalePreset.Altered, new ScaleMetadata(new[] { 0, 1, 3, 4, 6, 8, 10 }, "オルタード・スケール", "Altered Scale", new[] { "1", "♭2", "#2", "3", "♭5", "♭6", "♭7" }) },
        { ScalePreset.WholeTone, new ScaleMetadata(new[] { 0, 2, 4, 6, 8, 10 }, "ホールトーン・スケール", "Whole Tone Scale", new[] { "1", "2", "3", "#4", "#5", "♭7" }) },
        { ScalePreset.HalfWholeTone, new ScaleMetadata(new[] { 0, 1, 3, 4, 6, 7, 9, 10 }, "ハーフホールトーン・スケール", "Half Whole Tone Scale", new[] { "1", "♭2", "#2", "3", "#4", "5", "6", "♭7" }) },
        { ScalePreset.Diminished, new ScaleMetadata(new[] { 0, 2, 3, 5, 6, 8, 9, 11 }, "ディミニッシュ・スケール", "Diminished Scale", new[] { "1", "2", "♭3", "4", "♭5", "♭6", "6", "7" }) },
        { ScalePreset.LydianDominant, new ScaleMetadata(new[] { 0, 2, 4, 6, 7, 9, 10 }, "リディアンドミナント・スケール", "Lydian Dominant Scale", new[] { "1", "2", "3", "#4", "5", "6", "♭7" }) },
        { ScalePreset.Spanish8Note, new ScaleMetadata(new[] { 0, 1, 3, 4, 5, 6, 8, 10 }, "スパニッシュ・8ノート・スケール", "Spanish 8 Note Scale", new[] { "1", "♭2", "#2", "3", "4", "#4", "♭6", "♭7" }) },
        { ScalePreset.Arabic, new ScaleMetadata(new[] { 0, 2, 4, 5, 6, 8, 10 }, "アラビック・スケール", "Arabic Scale", new[] { "1", "2", "3", "4", "♭5", "♭6", "♭7" }) },
        { ScalePreset.Hungarian, new ScaleMetadata(new[] { 0, 2, 3, 6, 7, 8, 11 }, "ハンガリアン・スケール", "Hungarian Scale", new[] { "1", "2", "♭3", "#4", "5", "♭6", "7" }) },
        { ScalePreset.Gypsy, new ScaleMetadata(new[] { 0, 1, 4, 5, 7, 8, 10 }, "ジプシー・スケール", "Gypsy Scale", new[] { "1", "♭2", "3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.Hindu, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 8, 10 }, "ヒンズー・スケール", "Hindu Scale", new[] { "1", "2", "3", "4", "5", "♭6", "♭7" }) },
        { ScalePreset.Scottish, new ScaleMetadata(new[] { 0, 2, 4, 7, 9 }, "スコティッシュ・スケール", "Scottish Scale", new[] { "1", "2", "3", "5", "6" }) },
        { ScalePreset.YonanukiMajor, new ScaleMetadata(new[] { 0, 2, 4, 7, 9 }, "ヨナ抜き長音階", "Yonanuki Major Scale", new[] { "1", "2", "3", "5", "6" }) },
        { ScalePreset.YonanukiMinor, new ScaleMetadata(new[] { 0, 3, 5, 7, 10 }, "ヨナ抜き短音階", "Yonanuki Minor Scale", new[] { "1", "♭3", "4", "5", "♭7" }) },
        { ScalePreset.Ryukyu, new ScaleMetadata(new[] { 0, 4, 5, 7, 11 }, "琉球音階", "Ryukyu Scale", new[] { "1", "3", "4", "5", "7" }) },
        // Hindustani ragas ordered and assigned contiguous time ranges (covers 0:00–24:00 without overlap)
        { ScalePreset.RagaKhamaj, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 9, 10 }, "ラーガ・カマージ", "Raga Khamaj Scale", new[] { "1", "2", "3", "4", "5", "6", "♭7" }, "0:00〜3:00", "0:00-3:00") },
        { ScalePreset.RagaKafi, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 9, 10 }, "ラーガ・カフィ", "Raga Kafi Scale", new[] { "1", "2", "♭3", "4", "5", "6", "♭7" }, "3:00〜6:00", "3:00-6:00") },
        { ScalePreset.RagaBhairav, new ScaleMetadata(new[] { 0, 1, 4, 5, 7, 8, 11 }, "ラーガ・バイラブ", "Raga Bhairav Scale", new[] { "1", "♭2", "3", "4", "5", "♭6", "7" }, "6:00〜8:00", "6:00-8:00") },
        { ScalePreset.RagaTodi, new ScaleMetadata(new[] { 0, 1, 3, 6, 7, 8, 10 }, "ラーガ・トディ", "Raga Todi Scale", new[] { "1", "♭2", "♭3", "#4", "5", "♭6", "♭7" }, "8:00〜11:00", "8:00-11:00") },
        { ScalePreset.RagaBilawal, new ScaleMetadata(new[] { 0, 2, 4, 5, 7, 9, 11 }, "ラーガ・ビラーワル", "Raga Bilawal Scale", new[] { "1", "2", "3", "4", "5", "6", "7" }, "11:00〜14:00", "11:00-14:00") },
        { ScalePreset.RagaBhairavi, new ScaleMetadata(new[] { 0, 1, 3, 5, 7, 8, 10 }, "ラーガ・バイラヴィ", "Raga Bhairavi Scale", new[] { "1", "♭2", "♭3", "4", "5", "♭6", "♭7" }, "14:00〜16:30", "14:00-16:30") },
        { ScalePreset.RagaAsavari, new ScaleMetadata(new[] { 0, 2, 3, 5, 7, 8, 10 }, "ラーガ・アサヴァリ", "Raga Asavari Scale", new[] { "1", "2", "♭3", "4", "5", "♭6", "♭7" }, "16:30〜18:30", "16:30-18:30") },
        { ScalePreset.RagaPurvi, new ScaleMetadata(new[] { 0, 1, 4, 6, 7, 8, 11 }, "ラーガ・プルヴィ", "Raga Purvi Scale", new[] { "1", "♭2", "3", "#4", "5", "♭6", "7" }, "18:30〜20:30", "18:30-20:30") },
        { ScalePreset.RagaKalyan, new ScaleMetadata(new[] { 0, 2, 4, 6, 7, 9, 11 }, "ラーガ・カリヤン", "Raga Kalyan (Yaman) Scale", new[] { "1", "2", "3", "#4", "5", "6", "7" }, "20:30〜24:00", "20:30-24:00") }
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
        get
        {
            var md = CurrentMetadata;
            if (!string.IsNullOrEmpty(md.TimeRangeJapanese))
            {
                return string.Format("{0}（{1}）", md.DisplayNameJapanese, md.TimeRangeJapanese);
            }
            return md.DisplayNameJapanese;
        }
    }

    public string PresetDisplayNameEnglish
    {
        get
        {
            var md = CurrentMetadata;
            if (!string.IsNullOrEmpty(md.TimeRangeEnglish))
            {
                return string.Format("{0} ({1})", md.DisplayNameEnglish, md.TimeRangeEnglish);
            }
            return md.DisplayNameEnglish;
        }
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
        var md = GetMetadata(preset);
        if (!string.IsNullOrEmpty(md.TimeRangeJapanese))
        {
            return string.Format("{0}（{1}）", md.DisplayNameJapanese, md.TimeRangeJapanese);
        }
        return md.DisplayNameJapanese;
    }

    // スケール種別の英語名を返す
    public static string GetPresetDisplayNameEnglish(ScalePreset preset)
    {
        var md = GetMetadata(preset);
        if (!string.IsNullOrEmpty(md.TimeRangeEnglish))
        {
            return string.Format("{0} ({1})", md.DisplayNameEnglish, md.TimeRangeEnglish);
        }
        return md.DisplayNameEnglish;
    }
}

[Serializable]
public class ScaleMetadata
{
    // Backward-compatible constructor (no time range)
    public ScaleMetadata(int[] intervals, string displayNameJapanese, string displayNameEnglish, string[] degreeNames)
        : this(intervals, displayNameJapanese, displayNameEnglish, degreeNames, null, null)
    {
    }

    // New constructor with optional time range strings (Japanese / English)
    public ScaleMetadata(int[] intervals, string displayNameJapanese, string displayNameEnglish, string[] degreeNames, string timeRangeJapanese, string timeRangeEnglish)
    {
        Intervals = intervals;
        DisplayNameJapanese = displayNameJapanese;
        DisplayNameEnglish = displayNameEnglish;
        DegreeNames = degreeNames;
        TimeRangeJapanese = timeRangeJapanese;
        TimeRangeEnglish = timeRangeEnglish;
    }

    public int[] Intervals { get; private set; }

    public string DisplayNameJapanese { get; private set; }

    public string DisplayNameEnglish { get; private set; }

    public string[] DegreeNames { get; private set; }

    // Optional time range to display (e.g. "6:00〜8:00")
    public string TimeRangeJapanese { get; private set; }

    public string TimeRangeEnglish { get; private set; }
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
    RagaBhairav = 29,
    // Added Hindustani raga presets (primary/top-level variants only)
    RagaAsavari = 30,
    RagaBilawal = 31,
    RagaBhairavi = 32,
    RagaKafi = 33,
    RagaKalyan = 34,
    RagaKhamaj = 35,
    RagaPurvi = 36,
    RagaTodi = 37
}
