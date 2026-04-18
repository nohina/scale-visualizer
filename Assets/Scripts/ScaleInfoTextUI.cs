using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 現在選択されているスケール情報を TextMeshPro で表示する UI
public class ScaleInfoTextUI : MonoBehaviour
{
    // スケール情報の取得元
    [SerializeField] private ScaleVisualizer scaleVisualizer;
    // UI 全体の言語設定
    [SerializeField] private UiLanguageSettings languageSettings;
    // 「C メジャースケール」のようなタイトル表示先
    [SerializeField] private TMP_Text scaleTitleText;
    // 度数ベースの構成音表示先
    [SerializeField] private TMP_Text scaleDegreesText;
    // 構成音同士の区切り文字
    [SerializeField] private string degreeSeparator = "\n";

    private void Start()
    {
        Refresh();
    }

    private void OnEnable()
    {
        SubscribeLanguageSettings();
    }

    private void OnDisable()
    {
        UnsubscribeLanguageSettings();
    }

    // 現在の ScaleVisualizer の状態から UI を更新する
    public void Refresh()
    {
        if (scaleVisualizer == null || scaleVisualizer.ScaleDefinition == null)
        {
            return;
        }

        UiLanguageSettings settings = GetLanguageSettings();
        bool useEnglish = settings != null && settings.UseEnglish;
        ScaleDefinition scaleDefinition = scaleVisualizer.ScaleDefinition;
        string rootName = scaleVisualizer.GetDisplayNoteName(scaleDefinition.RootNote);

        if (scaleTitleText != null)
        {
            scaleTitleText.text = useEnglish
                ? rootName + " " + scaleDefinition.PresetDisplayNameEnglish
                : scaleVisualizer.GetScaleTitleJapanese();
        }

        if (scaleDegreesText != null)
        {
            IReadOnlyList<string> degreeNames = scaleDefinition.DegreeNames;
            IReadOnlyList<string> noteNames = scaleVisualizer.GetScaleDegreeDisplayNames();
            List<string> lines = new List<string>(degreeNames.Count);

            for (int i = 0; i < degreeNames.Count && i < noteNames.Count; i++)
            {
                int separatorIndex = noteNames[i].IndexOf(':');
                string notePart = separatorIndex >= 0 ? noteNames[i].Substring(separatorIndex + 1).Trim() : noteNames[i];
                lines.Add(degreeNames[i] + ": " + notePart);
            }

            scaleDegreesText.text = string.Join(degreeSeparator, lines);
        }
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
        Refresh();
    }
}
