using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 指板上のスケール構成音を uGUI で可視化するコンポーネント
[ExecuteAlways]
public class ScaleVisualizer : MonoBehaviour
{
    // 自動生成する UI コンテナ名
    private const string GuidesContainerName = "GeneratedGuides";
    private const string NotesContainerName = "GeneratedNotes";
    private const string FretNumbersContainerName = "GeneratedFretNumbers";

    [Header("References")]
    // チューニング設定
    [SerializeField] private GuitarTuningData tuningData = new GuitarTuningData();
    // スケール設定
    [SerializeField] private ScaleDefinition scaleDefinition = new ScaleDefinition();
    // UI 全体の言語設定
    [SerializeField] private UiLanguageSettings languageSettings;
    // 通常フレット位置のマーカー
    [SerializeField] private Image markerPrefab;
    // 開放弦専用マーカー
    [SerializeField] private Image openStringMarkerPrefab;
    // 音名ラベル
    [SerializeField] private TextMeshProUGUI noteLabelPrefab;
    // 生成物を配置する親 RectTransform
    [SerializeField] private RectTransform markerRoot;
    // スケール情報表示 UI
    [SerializeField] private ScaleInfoTextUI scaleInfoTextUI;
    // 選択 UI
    [SerializeField] private ScaleSelectionUI scaleSelectionUI;

    [Header("Fretboard Layout")]
    // 表示する最大フレット数
    [SerializeField] private int fretCount = 12;
    // 弦同士の縦方向間隔
    [SerializeField] private float stringSpacing = 35f;
    // フレット同士の横方向間隔
    [SerializeField] private float fretSpacing = 50f;
    // 指板描画の基準位置
    [SerializeField] private Vector2 origin = Vector2.zero;
    // 通常ラベルのオフセット
    [SerializeField] private Vector2 labelOffset = new Vector2(0f, 15f);
    // 開放弦マーカーの位置補正
    [SerializeField] private Vector2 openStringMarkerOffset = new Vector2(-30f, 0f);
    // 開放弦ラベルの位置補正
    [SerializeField] private Vector2 openStringLabelOffset = new Vector2(-30f, 15f);
    // 1弦〜6弦の上下表示順を反転するか
    [SerializeField] private bool invertStringOrder = false;
    // Start 時に自動生成するか
    [SerializeField] private bool regenerateOnStart = true;
    // Inspector 変更時に自動再生成するか
    [SerializeField] private bool regenerateOnValidate = true;
    // ラベルにオクターブ番号を含めるか
    [SerializeField] private bool showOctaveInLabel = true;
    // オクターブ情報が不足した場合の基準値
    [SerializeField] private int baseOctave = 1;
    // シャープ/フラット表記ルール
    [SerializeField] private AccidentalDisplay accidentalDisplay = AccidentalDisplay.Auto;

    [Header("Marker Colors")]
    // 通常音のマーカー色
    [SerializeField] private Color defaultMarkerColor = Color.white;
    // ルート音のマーカー色
    [SerializeField] private Color rootMarkerColor = Color.red;
    // 通常音のラベル色
    [SerializeField] private Color defaultLabelColor = Color.white;
    // ルート音のラベル色
    [SerializeField] private Color rootLabelColor = Color.yellow;
    // 開放弦のマーカー色
    [SerializeField] private Color openStringMarkerColor = Color.cyan;
    // 開放弦かつルート音のマーカー色
    [SerializeField] private Color openStringRootMarkerColor = new Color(1f, 0.5f, 0f);
    // 開放弦のラベル色
    [SerializeField] private Color openStringLabelColor = Color.cyan;
    // 開放弦かつルート音のラベル色
    [SerializeField] private Color openStringRootLabelColor = new Color(1f, 0.85f, 0.2f);

    [Header("Fretboard Guides")]
    // 弦線・フレット線を生成するか
    [SerializeField] private bool generateFretboardGuides = true;
    // ガイド線用 Image プレハブ
    [SerializeField] private Image guideLinePrefab;
    // 弦線の色
    [SerializeField] private Color stringLineColor = new Color(1f, 1f, 1f, 0.35f);
    // フレット線の色
    [SerializeField] private Color fretLineColor = new Color(1f, 1f, 1f, 0.35f);
    // 弦線の太さ
    [SerializeField] private float stringLineThickness = 2f;
    // 通常フレット線の太さ
    [SerializeField] private float fretLineThickness = 2f;
    // ナット線の太さ
    [SerializeField] private float nutLineThickness = 6f;

    [Header("Fret Numbers")]
    // フレット番号を表示するか
    [SerializeField] private bool generateFretNumbers = true;
    // フレット番号用 TextMeshPro プレハブ
    [SerializeField] private TextMeshProUGUI fretNumberPrefab;
    // フレット番号の位置補正
    [SerializeField] private Vector2 fretNumberOffset = new Vector2(0f, -35f);
    // フレット番号の色
    [SerializeField] private Color fretNumberColor = Color.white;

    // 自動生成したガイド線の管理リスト
    private readonly List<GameObject> spawnedGuideObjects = new List<GameObject>();
    // 自動生成したノート表示の管理リスト
    private readonly List<GameObject> spawnedNoteObjects = new List<GameObject>();
    // 自動生成したフレット番号の管理リスト
    private readonly List<GameObject> spawnedFretNumberObjects = new List<GameObject>();

    private bool regenerateQueued;

    public ScaleDefinition ScaleDefinition
    {
        get { return scaleDefinition; }
    }

    public GuitarTuningData TuningData
    {
        get { return tuningData; }
    }

    public UiLanguageSettings LanguageSettings
    {
        get { return languageSettings != null ? languageSettings : UiLanguageSettings.Instance; }
    }

    // 指定した半音番号を現在の表記ルールで文字列化する
    public string GetDisplayNoteName(int normalizedNote)
    {
        return NoteNameUtility.GetDisplayName(normalizedNote, ResolveAccidentalDisplay());
    }

    // スケール構成音を度数付きで返す
    public IReadOnlyList<string> GetScaleDegreeDisplayNames()
    {
        List<string> degreeNames = new List<string>();
        IReadOnlyList<int> intervals = scaleDefinition.Intervals;
        IReadOnlyList<string> noteNames = GetSpelledScaleNoteNames();

        for (int i = 0; i < intervals.Count; i++)
        {
            int note = NormalizeNote(scaleDefinition.RootNote + intervals[i]);
            degreeNames.Add((i + 1).ToString() + ": " + noteNames[note]);
        }

        return degreeNames;
    }

    // UI 表示用のスケールタイトルを返す
    public string GetScaleTitleJapanese()
    {
        return GetDisplayNoteName(scaleDefinition.RootNote) + " " + scaleDefinition.PresetDisplayNameJapanese;
    }

    private void Start()
    {
        if (regenerateOnStart)
        {
            Regenerate();
        }
    }

    private void OnEnable()
    {
        SubscribeLanguageSettings();

        if (!Application.isPlaying && regenerateOnValidate)
        {
            QueueRegenerate();
        }
    }

    private void OnDisable()
    {
        UnsubscribeLanguageSettings();
    }

    private void OnValidate()
    {
        if (!regenerateOnValidate)
        {
            return;
        }

        SubscribeLanguageSettings();
        QueueRegenerate();
    }

    private void QueueRegenerate()
    {
        regenerateQueued = true;
    }

    private void LateUpdate()
    {
        if (!regenerateQueued)
        {
            return;
        }

        regenerateQueued = false;
        Regenerate();
    }

    private void SubscribeLanguageSettings()
    {
        UiLanguageSettings settings = LanguageSettings;
        if (settings == null)
        {
            return;
        }

        settings.LanguageChanged -= OnLanguageChanged;
        settings.LanguageChanged += OnLanguageChanged;
    }

    private void UnsubscribeLanguageSettings()
    {
        UiLanguageSettings settings = LanguageSettings;
        if (settings == null)
        {
            return;
        }

        settings.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(UiLanguage language)
    {
        if (Application.isPlaying)
        {
            Regenerate();
            return;
        }

        QueueRegenerate();
    }

    // 指板表示を全て作り直す
    public void Regenerate()
    {
        ClearMarkers();

        if (!IsReady())
        {
            RefreshLinkedUi();
            return;
        }

        Dictionary<string, RectTransform> containers = GetOrCreateContainers(
            GuidesContainerName,
            NotesContainerName,
            FretNumbersContainerName);

        if (generateFretboardGuides)
        {
            GenerateFretboardGuides(containers[GuidesContainerName]);
        }

        if (generateFretNumbers)
        {
            GenerateFretNumbers(containers[FretNumbersContainerName]);
        }

        HashSet<int> scaleNotes = scaleDefinition.GetScaleNotes();
        IReadOnlyList<int> openStrings = tuningData.OpenStringNotes;
        IReadOnlyList<int> openStringOctaves = tuningData.OpenStringOctaves;
        int rootNote = NormalizeNote(scaleDefinition.RootNote);

        // 各弦・各フレットを走査し、スケール内の音だけ表示する
        for (int stringIndex = 0; stringIndex < openStrings.Count; stringIndex++)
        {
            int openNote = NormalizeNote(openStrings[stringIndex]);
            int openOctave = stringIndex < openStringOctaves.Count ? openStringOctaves[stringIndex] : baseOctave;

            for (int fret = 0; fret <= fretCount; fret++)
            {
                int note = NormalizeNote(openNote + fret);
                if (!scaleNotes.Contains(note))
                {
                    continue;
                }

                bool isRoot = note == rootNote;
                bool isOpenString = fret == 0;
                Vector2 position = GetNotePosition(stringIndex, fret, openStrings.Count);
                string noteLabel = GetNoteLabel(openNote, openOctave, fret);
                SpawnMarker(containers[NotesContainerName], position, isRoot, isOpenString);
                SpawnLabel(containers[NotesContainerName], GetLabelPosition(position, isOpenString), noteLabel, isRoot, isOpenString);
            }
        }

        RefreshLinkedUi();
    }

    // 連動している情報 UI を更新する
    private void RefreshLinkedUi()
    {
        if (scaleSelectionUI != null)
        {
            scaleSelectionUI.Initialize();
        }

        if (scaleInfoTextUI != null)
        {
            scaleInfoTextUI.Refresh();
        }
    }

    // 自動生成した表示物を削除する
    public void ClearMarkers()
    {
        ClearSpawnedObjects(spawnedGuideObjects);
        ClearSpawnedObjects(spawnedNoteObjects);
        ClearSpawnedObjects(spawnedFretNumberObjects);
        CleanupContainer(GuidesContainerName);
        CleanupContainer(NotesContainerName);
        CleanupContainer(FretNumbersContainerName);
    }

    // 描画に必要な参照が揃っているか確認する
    private bool IsReady()
    {
        if (scaleDefinition == null || markerPrefab == null)
        {
            Debug.LogWarning("ScaleVisualizer is missing required references.", this);
            return false;
        }

        if (markerRoot == null)
        {
            Debug.LogWarning("ScaleVisualizer requires a RectTransform markerRoot for uGUI placement.", this);
            return false;
        }

        if (tuningData.OpenStringNotes == null || tuningData.OpenStringNotes.Count == 0)
        {
            Debug.LogWarning("GuitarTuningData does not contain any open string notes.", this);
            return false;
        }

        if (generateFretboardGuides && guideLinePrefab == null)
        {
            Debug.LogWarning("ScaleVisualizer requires a guideLinePrefab when fretboard guides are enabled.", this);
            return false;
        }

        if (generateFretNumbers && fretNumberPrefab == null)
        {
            Debug.LogWarning("ScaleVisualizer requires a fretNumberPrefab when fret numbers are enabled.", this);
            return false;
        }

        return true;
    }

    // 弦番号とフレット番号からマーカー位置を計算する
    private Vector2 GetNotePosition(int stringIndex, int fret, int stringCount)
    {
        float x = fret == 0
            ? origin.x + openStringMarkerOffset.x
            : origin.x + (fret * fretSpacing) - (fretSpacing * 0.5f);

        int displayStringIndex = GetDisplayStringIndex(stringIndex, stringCount);
        float y = origin.y - (displayStringIndex * stringSpacing);
        return new Vector2(x, y);
    }

    // 弦の上下表示順を必要に応じて反転する
    private int GetDisplayStringIndex(int stringIndex, int stringCount)
    {
        if (!invertStringOrder)
        {
            return stringIndex;
        }

        return (stringCount - 1) - stringIndex;
    }

    // ラベル位置をマーカー位置から計算する
    private Vector2 GetLabelPosition(Vector2 markerPosition, bool isOpenString)
    {
        return markerPosition + (isOpenString ? openStringLabelOffset - openStringMarkerOffset : labelOffset);
    }

    // 音名ラベル文字列を生成する
    private string GetNoteLabel(int openNote, int openOctave, int fret)
    {
        int semitoneFromOpen = openNote + fret;
        string noteName = GetDisplayNoteName(semitoneFromOpen);

        if (!showOctaveInLabel)
        {
            return noteName;
        }

        int octave = openOctave + ((openNote + fret) / 12);
        return noteName + octave;
    }

    // 現在の表記ルールに応じた 12 音の名前一覧を返す
    private IReadOnlyList<string> GetSpelledScaleNoteNames()
    {
        return NoteNameUtility.GetDisplayNames(ResolveAccidentalDisplay());
    }

    // Auto 指定時にキーから表記ルールを決定する
    private AccidentalDisplay ResolveAccidentalDisplay()
    {
        UiLanguageSettings settings = LanguageSettings;
        if (settings != null && settings.UseEnglish && accidentalDisplay == AccidentalDisplay.Auto)
        {
            return AccidentalDisplay.Sharp;
        }

        if (accidentalDisplay != AccidentalDisplay.Auto)
        {
            return accidentalDisplay;
        }

        switch (scaleDefinition.RootNoteName)
        {
            case NoteName.F:
            case NoteName.ASharp:
            case NoteName.DSharp:
            case NoteName.GSharp:
                return AccidentalDisplay.Flat;
            case NoteName.FSharp:
            case NoteName.CSharp:
                return AccidentalDisplay.TheoreticalSharp;
            default:
                return AccidentalDisplay.Sharp;
        }
    }

    // マーカーを生成する
    private void SpawnMarker(RectTransform parent, Vector2 position, bool isRoot, bool isOpenString)
    {
        Image prefab = isOpenString && openStringMarkerPrefab != null ? openStringMarkerPrefab : markerPrefab;
        Image marker = Instantiate(prefab, parent);
        RectTransform rectTransform = marker.rectTransform;
        rectTransform.anchoredPosition = position;
        marker.color = GetMarkerColor(isRoot, isOpenString);
        RegisterSpawnedObject(marker.gameObject, spawnedNoteObjects);
    }

    // 音名ラベルを生成する
    private void SpawnLabel(RectTransform parent, Vector2 position, string noteName, bool isRoot, bool isOpenString)
    {
        if (noteLabelPrefab == null)
        {
            return;
        }

        TextMeshProUGUI label = Instantiate(noteLabelPrefab, parent);
        RectTransform rectTransform = label.rectTransform;
        rectTransform.anchoredPosition = position;
        label.text = noteName;
        label.color = GetLabelColor(isRoot, isOpenString);
        RegisterSpawnedObject(label.gameObject, spawnedNoteObjects);
    }

    // 弦線とフレット線を生成する
    private void GenerateFretboardGuides(RectTransform parent)
    {
        IReadOnlyList<int> openStrings = tuningData.OpenStringNotes;
        int stringCount = openStrings.Count;
        float width = fretCount * fretSpacing;
        float height = (stringCount - 1) * stringSpacing;

        // 先にフレット線を生成する
        for (int fret = 0; fret <= fretCount; fret++)
        {
            float thickness = fret == 0 ? nutLineThickness : fretLineThickness;
            Vector2 position = new Vector2(origin.x + (fret * fretSpacing), origin.y - (height * 0.5f));
            SpawnGuideLine(parent, position, new Vector2(thickness, height + stringLineThickness), fretLineColor);
        }

        // 後から弦線を生成して前面に表示する
        for (int stringIndex = 0; stringIndex < stringCount; stringIndex++)
        {
            int displayStringIndex = GetDisplayStringIndex(stringIndex, stringCount);
            Vector2 position = new Vector2(origin.x + (width * 0.5f), origin.y - (displayStringIndex * stringSpacing));
            SpawnGuideLine(parent, position, new Vector2(width + fretLineThickness, stringLineThickness), stringLineColor);
        }
    }

    // フレット番号を生成する
    private void GenerateFretNumbers(RectTransform parent)
    {
        IReadOnlyList<int> openStrings = tuningData.OpenStringNotes;
        float y = origin.y - ((openStrings.Count - 1) * stringSpacing) + fretNumberOffset.y;

        for (int fret = 1; fret <= fretCount; fret++)
        {
            Vector2 position = new Vector2(origin.x + (fret * fretSpacing) - (fretSpacing * 0.5f) + fretNumberOffset.x, y);
            SpawnFretNumber(parent, position, fret.ToString());
        }
    }

    // フレット番号ラベルを生成する
    private void SpawnFretNumber(RectTransform parent, Vector2 position, string value)
    {
        TextMeshProUGUI fretNumber = Instantiate(fretNumberPrefab, parent);
        RectTransform rectTransform = fretNumber.rectTransform;
        rectTransform.anchoredPosition = position;
        fretNumber.text = value;
        fretNumber.color = fretNumberColor;
        RegisterSpawnedObject(fretNumber.gameObject, spawnedFretNumberObjects);
    }

    // ガイド線を生成する
    private void SpawnGuideLine(RectTransform parent, Vector2 position, Vector2 size, Color color)
    {
        Image guide = Instantiate(guideLinePrefab, parent);
        RectTransform rectTransform = guide.rectTransform;
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = size;
        guide.color = color;
        RegisterSpawnedObject(guide.gameObject, spawnedGuideObjects);
    }

    // 指定名の子コンテナ群を取得し、無ければ生成する
    private Dictionary<string, RectTransform> GetOrCreateContainers(params string[] containerNames)
    {
        Dictionary<string, RectTransform> containers = new Dictionary<string, RectTransform>();
        for (int i = 0; i < containerNames.Length; i++)
        {
            containers[containerNames[i]] = GetOrCreateContainer(containerNames[i]);
        }

        return containers;
    }

    // 指定名の子コンテナを取得し、無ければ生成する
    private RectTransform GetOrCreateContainer(string containerName)
    {
        Transform existing = markerRoot.Find(containerName);
        if (existing != null)
        {
            return existing as RectTransform;
        }

        GameObject containerObject = new GameObject(containerName, typeof(RectTransform));
        RectTransform container = containerObject.GetComponent<RectTransform>();
        container.SetParent(markerRoot, false);
        container.anchorMin = new Vector2(0.5f, 0.5f);
        container.anchorMax = new Vector2(0.5f, 0.5f);
        container.pivot = new Vector2(0.5f, 0.5f);
        container.anchoredPosition = Vector2.zero;
        container.sizeDelta = Vector2.zero;
        container.localScale = Vector3.one;
        return container;
    }

    // マーカー色を状態に応じて決定する
    private Color GetMarkerColor(bool isRoot, bool isOpenString)
    {
        if (isOpenString)
        {
            return isRoot ? openStringRootMarkerColor : openStringMarkerColor;
        }

        return isRoot ? rootMarkerColor : defaultMarkerColor;
    }

    // ラベル色を状態に応じて決定する
    private Color GetLabelColor(bool isRoot, bool isOpenString)
    {
        if (isOpenString)
        {
            return isRoot ? openStringRootLabelColor : openStringLabelColor;
        }

        return isRoot ? rootLabelColor : defaultLabelColor;
    }

    // 生成物を管理リストへ登録する
    private void RegisterSpawnedObject(GameObject target, List<GameObject> registry)
    {
        if (target == null)
        {
            return;
        }

        target.name = target.name.Replace("(Clone)", string.Empty).Trim();
        target.hideFlags = HideFlags.DontSave;
        registry.Add(target);
    }

    // 管理している生成物をまとめて削除する
    private void ClearSpawnedObjects(List<GameObject> registry)
    {
        for (int i = registry.Count - 1; i >= 0; i--)
        {
            DestroySpawnedObject(registry[i]);
        }

        registry.Clear();
    }

    // コンテナ配下に残っている生成物も削除する
    private void CleanupContainer(string containerName)
    {
        if (markerRoot == null)
        {
            return;
        }

        Transform container = markerRoot.Find(containerName);
        if (container == null)
        {
            return;
        }

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            DestroySpawnedObject(container.GetChild(i).gameObject);
        }
    }

    // Play モードかどうかで削除方法を切り替える
    private void DestroySpawnedObject(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(target);
            return;
        }

        DestroyImmediate(target);
    }

    // 半音番号を 0〜11 に正規化する
    private static int NormalizeNote(int note)
    {
        return NoteNameUtility.Normalize(note);
    }

    // UI からチューニングを変更する
    public void SetTuningPreset(GuitarTuningPreset preset)
    {
        tuningData.SetTuningPreset(preset);
        Regenerate();
    }

    // UI からルート音を変更する
    public void SetRootNote(NoteName noteName)
    {
        scaleDefinition.SetRootNote(noteName);
        Regenerate();
    }

    // UI からスケール種別を変更する
    public void SetScalePreset(ScalePreset preset)
    {
        scaleDefinition.SetScalePreset(preset);
        Regenerate();
    }
}
