using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// チューニング・ルート音・スケール種別を選択する UI
public class ScaleSelectionUI : MonoBehaviour
{
    // 選択結果を反映する対象
    [SerializeField] private ScaleVisualizer scaleVisualizer;
    // UI 全体の言語設定
    [SerializeField] private UiLanguageSettings languageSettings;
    // チューニング選択用ドロップダウン
    [SerializeField] private TMP_Dropdown tuningDropdown;
    // ルート音選択用ドロップダウン
    [SerializeField] private TMP_Dropdown rootNoteDropdown;
    // スケール種別選択用ドロップダウン
    [SerializeField] private TMP_Dropdown scaleDropdown;
    // チューニング名を短縮表示するかどうか
    [SerializeField] private bool useCompactTuningDisplayName = true;

    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        SubscribeLanguageSettings();
    }

    private void OnDisable()
    {
        UnsubscribeLanguageSettings();
    }

    // 各ドロップダウンを初期化する
    public void Initialize()
    {
        if (scaleVisualizer == null)
        {
            return;
        }

        UiLanguageSettings settings = GetLanguageSettings();
        bool useEnglish = settings != null && settings.UseEnglish;

        SetupDropdown(
            tuningDropdown,
            GetEnumOptions(typeof(GuitarTuningPreset), value =>
            {
                GuitarTuningPreset preset = (GuitarTuningPreset)value;
                if (useCompactTuningDisplayName)
                {
                    return GuitarTuningData.GetCompactTuningDisplayName(preset);
                }

                return useEnglish
                    ? GuitarTuningData.GetTuningDisplayNameEnglish(preset)
                    : GuitarTuningData.GetTuningDisplayNameJapanese(preset);
            }),
            (int)scaleVisualizer.TuningData.TuningPreset,
            OnTuningChanged);

        SetupDropdown(
            rootNoteDropdown,
            GetEnumOptions(typeof(NoteName), value => NoteNameUtility.GetSharpEnumDisplayName((NoteName)value)),
            scaleVisualizer.ScaleDefinition.RootNote,
            OnRootNoteChanged);

        SetupDropdown(
            scaleDropdown,
            GetEnumOptions(typeof(ScalePreset), value =>
            {
                ScalePreset preset = (ScalePreset)value;
                return useEnglish
                    ? ScaleDefinition.GetPresetDisplayNameEnglish(preset)
                    : ScaleDefinition.GetPresetDisplayNameJapanese(preset);
            }),
            (int)scaleVisualizer.ScaleDefinition.Preset,
            OnScaleChanged);
    }

    private UiLanguageSettings GetLanguageSettings()
    {
        if (scaleVisualizer != null && scaleVisualizer.LanguageSettings != null)
        {
            return scaleVisualizer.LanguageSettings;
        }

        if (languageSettings != null)
        {
            return languageSettings;
        }

        return UiLanguageSettings.Instance;
    }

    private void SubscribeLanguageSettings()
    {
        UiLanguageSettings settings = GetLanguageSettings();
        if (settings == null)
        {
            return;
        }

        settings.LanguageChanged -= OnLanguageChanged;
        settings.LanguageChanged += OnLanguageChanged;
    }

    private void UnsubscribeLanguageSettings()
    {
        UiLanguageSettings settings = GetLanguageSettings();
        if (settings == null)
        {
            return;
        }

        settings.LanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(UiLanguage language)
    {
        Initialize();
    }

    // 共通的なドロップダウン初期化処理
    private static void SetupDropdown(TMP_Dropdown dropdown, List<string> options, int selectedIndex, UnityEngine.Events.UnityAction<int> listener)
    {
        if (dropdown == null)
        {
            return;
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.SetValueWithoutNotify(selectedIndex);
        dropdown.onValueChanged.RemoveListener(listener);
        dropdown.onValueChanged.AddListener(listener);
    }

    // enum から表示文字列一覧を生成する
    private static List<string> GetEnumOptions(Type enumType, Func<object, string> formatter)
    {
        List<string> options = new List<string>();
        Array values = Enum.GetValues(enumType);
        for (int i = 0; i < values.Length; i++)
        {
            options.Add(formatter(values.GetValue(i)));
        }

        return options;
    }

    // チューニング変更時の反映処理
    private void OnTuningChanged(int index)
    {
        scaleVisualizer.SetTuningPreset((GuitarTuningPreset)index);
    }

    // ルート音変更時の反映処理
    private void OnRootNoteChanged(int index)
    {
        scaleVisualizer.SetRootNote((NoteName)index);
    }

    // スケール変更時の反映処理
    private void OnScaleChanged(int index)
    {
        scaleVisualizer.SetScalePreset((ScalePreset)index);
    }
}
