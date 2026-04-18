using System;
using System.Collections.Generic;
using UnityEngine;

// チューニングプリセットと開放弦情報を保持するデータ
[Serializable]
public class GuitarTuningData
{
    private static readonly Dictionary<GuitarTuningPreset, TuningDefinition> TuningDefinitions =
        new Dictionary<GuitarTuningPreset, TuningDefinition>
        {
            { GuitarTuningPreset.EADGBE, new TuningDefinition(new[] { NoteName.E, NoteName.A, NoteName.D, NoteName.G, NoteName.B, NoteName.E }, new[] { 2, 2, 3, 3, 3, 4 }, "EADGBE", "EADGBE (Standard Tuning)", "EADGBE（スタンダードチューニング）") },
            { GuitarTuningPreset.DADGBE, new TuningDefinition(new[] { NoteName.D, NoteName.A, NoteName.D, NoteName.G, NoteName.B, NoteName.E }, new[] { 2, 2, 3, 3, 3, 4 }, "DADGBE", "DADGBE (Drop D Tuning)", "DADGBE（ドロップDチューニング）") },
            { GuitarTuningPreset.CSharpGSharpCSharpFSharpASharpDSharp, new TuningDefinition(new[] { NoteName.CSharp, NoteName.GSharp, NoteName.CSharp, NoteName.FSharp, NoteName.ASharp, NoteName.DSharp }, new[] { 2, 2, 3, 3, 3, 4 }, "C#G#C#F#A#D#", "C#G#C#F#A#D# (Drop C# Tuning)", "C#G#C#F#A#D#（ドロップC#チューニング）") },
            { GuitarTuningPreset.CGCFAD, new TuningDefinition(new[] { NoteName.C, NoteName.G, NoteName.C, NoteName.F, NoteName.A, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "CGCFAD", "CGCFAD (Drop C Tuning)", "CGCFAD（ドロップCチューニング）") },
            { GuitarTuningPreset.BFSharpBEGSharpCSharp, new TuningDefinition(new[] { NoteName.B, NoteName.FSharp, NoteName.B, NoteName.E, NoteName.GSharp, NoteName.CSharp }, new[] { 1, 2, 2, 3, 3, 4 }, "BF#BEG#C#", "BF#BEG#C# (Drop B Tuning)", "BF#BEG#C#（ドロップBチューニング）") },
            { GuitarTuningPreset.ASharpFASharpDSharpGC, new TuningDefinition(new[] { NoteName.ASharp, NoteName.F, NoteName.ASharp, NoteName.DSharp, NoteName.G, NoteName.C }, new[] { 1, 2, 2, 3, 3, 4 }, "B♭FB♭E♭GC", "B♭FB♭E♭GC (Drop B♭ Tuning)", "B♭FB♭E♭GC（ドロップB♭チューニング）") },
            { GuitarTuningPreset.AEADFSharpB, new TuningDefinition(new[] { NoteName.A, NoteName.E, NoteName.A, NoteName.D, NoteName.FSharp, NoteName.B }, new[] { 1, 2, 2, 3, 3, 3 }, "AEADF#B", "AEADF#B (Drop A Tuning)", "AEADF#B（ドロップAチューニング）") },
            { GuitarTuningPreset.GSharpDSharpGSharpCSharpFASharp, new TuningDefinition(new[] { NoteName.GSharp, NoteName.DSharp, NoteName.GSharp, NoteName.CSharp, NoteName.F, NoteName.ASharp }, new[] { 1, 2, 2, 3, 3, 3 }, "A♭E♭A♭D♭FB♭", "A♭E♭A♭D♭FB♭ (Drop A♭ Tuning)", "A♭E♭A♭D♭FB♭（ドロップA♭チューニング）") },
            { GuitarTuningPreset.DGDGBD, new TuningDefinition(new[] { NoteName.D, NoteName.G, NoteName.D, NoteName.G, NoteName.B, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "DGDGBD", "DGDGBD (Open G Tuning)", "DGDGBD（オープンGチューニング）") },
            { GuitarTuningPreset.EAEACSharpE, new TuningDefinition(new[] { NoteName.E, NoteName.A, NoteName.E, NoteName.A, NoteName.CSharp, NoteName.E }, new[] { 2, 2, 3, 3, 4, 4 }, "EAEAC#E", "EAEAC#E (Open A Tuning)", "EAEAC#E（オープンAチューニング）") },
            { GuitarTuningPreset.DADFSharpAD, new TuningDefinition(new[] { NoteName.D, NoteName.A, NoteName.D, NoteName.FSharp, NoteName.A, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "DADF#AD", "DADF#AD (Open D Tuning)", "DADF#AD（オープンDチューニング）") },
            { GuitarTuningPreset.EBEGSharpBE, new TuningDefinition(new[] { NoteName.E, NoteName.B, NoteName.E, NoteName.GSharp, NoteName.B, NoteName.E }, new[] { 2, 2, 3, 3, 3, 4 }, "EBEG#BE", "EBEG#BE (Open E Tuning)", "EBEG#BE（オープンEチューニング）") },
            { GuitarTuningPreset.DGDGASharpD, new TuningDefinition(new[] { NoteName.D, NoteName.G, NoteName.D, NoteName.G, NoteName.ASharp, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "DGDGB♭D", "DGDGB♭D (Open G Minor Tuning)", "DGDGB♭D（オープンGmチューニング）") },
            { GuitarTuningPreset.EAEACE, new TuningDefinition(new[] { NoteName.E, NoteName.A, NoteName.E, NoteName.A, NoteName.C, NoteName.E }, new[] { 2, 2, 3, 3, 4, 4 }, "EAEACE", "EAEACE (Open A Minor Tuning)", "EAEACE（オープンAmチューニング）") },
            { GuitarTuningPreset.EBEGBE, new TuningDefinition(new[] { NoteName.E, NoteName.B, NoteName.E, NoteName.G, NoteName.B, NoteName.E }, new[] { 2, 2, 3, 3, 3, 4 }, "EBEGBE", "EBEGBE (Open E Minor Tuning)", "EBEGBE（オープンEmチューニング）") },
            { GuitarTuningPreset.DADFAD, new TuningDefinition(new[] { NoteName.D, NoteName.A, NoteName.D, NoteName.F, NoteName.A, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "DADFAD", "DADFAD (Open D Minor Tuning)", "DADFAD（オープンDmチューニング）") },
            { GuitarTuningPreset.DADGAD, new TuningDefinition(new[] { NoteName.D, NoteName.A, NoteName.D, NoteName.G, NoteName.A, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "DADGAD", "DADGAD (DADGAD Tuning)", "DADGAD（ダドガドチューニング）") },
            { GuitarTuningPreset.DADEAD, new TuningDefinition(new[] { NoteName.D, NoteName.A, NoteName.D, NoteName.E, NoteName.A, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "DADEAD", "DADEAD (DADEAD Tuning)", "DADEAD（ダデッドチューニング）") },
            { GuitarTuningPreset.DGDGCD, new TuningDefinition(new[] { NoteName.D, NoteName.G, NoteName.D, NoteName.G, NoteName.C, NoteName.D }, new[] { 2, 2, 3, 3, 4, 4 }, "DGDGCD", "DGDGCD (Open Gsus4 Tuning)", "DGDGCD（オープンGsus4チューニング）") },
            { GuitarTuningPreset.DGDGAD, new TuningDefinition(new[] { NoteName.D, NoteName.G, NoteName.D, NoteName.G, NoteName.A, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "DGDGAD", "DGDGAD (DGDGAD Tuning)", "DGDGAD（DGDGADチューニング）") },
            { GuitarTuningPreset.GBDGBD, new TuningDefinition(new[] { NoteName.G, NoteName.B, NoteName.D, NoteName.G, NoteName.B, NoteName.D }, new[] { 2, 2, 3, 3, 3, 4 }, "GBDGBD", "GBDGBD (G Tuning)", "GBDGBD（Gチューニング）") },
            { GuitarTuningPreset.DSharpGASharpCDSharpG, new TuningDefinition(new[] { NoteName.DSharp, NoteName.G, NoteName.ASharp, NoteName.C, NoteName.DSharp, NoteName.G }, new[] { 3, 3, 3, 4, 4, 4 }, "E♭GB♭CE♭G", "E♭GB♭CE♭G (Koto Hirajoshi Style)", "E♭GB♭CE♭G（箏・平調子風チューニング）") },
            { GuitarTuningPreset.DSharpGASharpCFA, new TuningDefinition(new[] { NoteName.DSharp, NoteName.G, NoteName.ASharp, NoteName.C, NoteName.F, NoteName.A }, new[] { 3, 3, 3, 4, 4, 4 }, "E♭GB♭CFA", "E♭GB♭CFA (Koto Nogijoshi Style)", "E♭GB♭CFA（箏・乃木調子風チューニング）") },
            { GuitarTuningPreset.DSharpFGASharpCD, new TuningDefinition(new[] { NoteName.DSharp, NoteName.F, NoteName.G, NoteName.ASharp, NoteName.C, NoteName.D }, new[] { 3, 3, 3, 3, 4, 4 }, "E♭FGB♭CD", "E♭FGB♭CD (Koto Kumoi Style)", "E♭FGB♭CD（箏・雲井調子風チューニング）") },
            { GuitarTuningPreset.CFCFCF, new TuningDefinition(new[] { NoteName.C, NoteName.F, NoteName.C, NoteName.F, NoteName.C, NoteName.F }, new[] { 3, 3, 4, 4, 4, 5 }, "CFCFCF", "CFCFCF (Shamisen Honchoshi Style)", "CFCFCF（三味線・本調子風チューニング）") },
            { GuitarTuningPreset.CGCGCG, new TuningDefinition(new[] { NoteName.C, NoteName.G, NoteName.C, NoteName.G, NoteName.C, NoteName.G }, new[] { 3, 3, 4, 4, 4, 5 }, "CGCGCG", "CGCGCG (Shamisen Niagari Style)", "CGCGCG（三味線・二上り風チューニング）") },
            { GuitarTuningPreset.CFCGCF, new TuningDefinition(new[] { NoteName.C, NoteName.F, NoteName.C, NoteName.G, NoteName.C, NoteName.F }, new[] { 3, 3, 4, 4, 4, 5 }, "CFCGCF", "CFCGCF (Shamisen Sansagari Style)", "CFCGCF（三味線・三下り風チューニング）") },
            { GuitarTuningPreset.CFCGCC, new TuningDefinition(new[] { NoteName.C, NoteName.F, NoteName.C, NoteName.G, NoteName.C, NoteName.C }, new[] { 3, 3, 4, 4, 4, 5 }, "CFCGCC", "CFCGCC (Sanshin Honchoshi Style)", "CFCGCC（沖縄三線・本調子風チューニング）") },
            { GuitarTuningPreset.CFCFCSharpF, new TuningDefinition(new[] { NoteName.C, NoteName.F, NoteName.C, NoteName.F, NoteName.CSharp, NoteName.F }, new[] { 3, 3, 4, 4, 4, 5 }, "CFCFC#F", "CFCFC#F (Sanshin Niage Style)", "CFCFC#F（沖縄三線・二揚げ風チューニング）") },
            { GuitarTuningPreset.CFCGCD, new TuningDefinition(new[] { NoteName.C, NoteName.F, NoteName.C, NoteName.G, NoteName.C, NoteName.D }, new[] { 3, 3, 4, 4, 4, 5 }, "CFCGCD", "CFCGCD (Sanshin Sansage Style)", "CFCGCD（沖縄三線・三下げ風チューニング）") }
        };

    // 現在選択されているチューニング
    [SerializeField] private GuitarTuningPreset tuningPreset = GuitarTuningPreset.EADGBE;

    public GuitarTuningPreset TuningPreset
    {
        get { return tuningPreset; }
    }

    // UI からチューニングを変更する
    public void SetTuningPreset(GuitarTuningPreset preset)
    {
        tuningPreset = preset;
    }

    // 開放弦を半音番号で返す
    public IReadOnlyList<int> OpenStringNotes
    {
        get
        {
            NoteName[] source = CurrentDefinition.OpenStringNotes;
            List<int> notes = new List<int>(source.Length);
            for (int i = 0; i < source.Length; i++)
            {
                notes.Add((int)source[i]);
            }

            return notes;
        }
    }

    // 開放弦を音名 enum で返す
    public IReadOnlyList<NoteName> OpenStringNoteNames
    {
        get { return CurrentDefinition.OpenStringNotes; }
    }

    // チューニング名の日本語表示
    public string TuningDisplayNameJapanese
    {
        get { return CurrentDefinition.DisplayNameJapanese; }
    }

    // チューニング名の英語表示
    public string TuningDisplayNameEnglish
    {
        get { return CurrentDefinition.DisplayNameEnglish; }
    }

    // チューニング名の短縮表示
    public string CompactTuningDisplayName
    {
        get { return CurrentDefinition.CompactDisplayName; }
    }

    // 開放弦の実オクターブ情報
    public IReadOnlyList<int> OpenStringOctaves
    {
        get { return CurrentDefinition.OpenStringOctaves; }
    }

    private TuningDefinition CurrentDefinition
    {
        get { return GetDefinition(tuningPreset); }
    }

    private static TuningDefinition GetDefinition(GuitarTuningPreset preset)
    {
        TuningDefinition definition;
        if (TuningDefinitions.TryGetValue(preset, out definition))
        {
            return definition;
        }

        return TuningDefinitions[GuitarTuningPreset.EADGBE];
    }

    // チューニング名の日本語表示を返す
    public static string GetTuningDisplayNameJapanese(GuitarTuningPreset preset)
    {
        return GetDefinition(preset).DisplayNameJapanese;
    }

    // チューニング名の英語表示を返す
    public static string GetTuningDisplayNameEnglish(GuitarTuningPreset preset)
    {
        return GetDefinition(preset).DisplayNameEnglish;
    }

    // チューニング名の短縮表示を返す
    public static string GetCompactTuningDisplayName(GuitarTuningPreset preset)
    {
        return GetDefinition(preset).CompactDisplayName;
    }
}

[Serializable]
public class TuningDefinition
{
    public TuningDefinition(NoteName[] openStringNotes, int[] openStringOctaves, string compactDisplayName, string displayNameEnglish, string displayNameJapanese)
    {
        OpenStringNotes = openStringNotes;
        OpenStringOctaves = openStringOctaves;
        CompactDisplayName = compactDisplayName;
        DisplayNameEnglish = displayNameEnglish;
        DisplayNameJapanese = displayNameJapanese;
    }

    public NoteName[] OpenStringNotes { get; private set; }

    public int[] OpenStringOctaves { get; private set; }

    public string CompactDisplayName { get; private set; }

    public string DisplayNameEnglish { get; private set; }

    public string DisplayNameJapanese { get; private set; }
}

// 利用可能なチューニング一覧
public enum GuitarTuningPreset
{
    EADGBE = 0,
    DADGBE = 1,
    CSharpGSharpCSharpFSharpASharpDSharp = 2,
    CGCFAD = 3,
    BFSharpBEGSharpCSharp = 4,
    ASharpFASharpDSharpGC = 5,
    AEADFSharpB = 6,
    GSharpDSharpGSharpCSharpFASharp = 7,
    DGDGBD = 8,
    EAEACSharpE = 9,
    DADFSharpAD = 10,
    EBEGSharpBE = 11,
    DGDGASharpD = 12,
    EAEACE = 13,
    EBEGBE = 14,
    DADFAD = 15,
    DADGAD = 16,
    DADEAD = 17,
    DGDGCD = 18,
    DGDGAD = 19,
    GBDGBD = 20,
    DSharpGASharpCDSharpG = 21,
    DSharpGASharpCFA = 22,
    DSharpFGASharpCD = 23,
    CFCFCF = 24,
    CGCGCG = 25,
    CFCGCF = 26,
    CFCGCC = 27,
    CFCFCSharpF = 28,
    CFCGCD = 29
}
