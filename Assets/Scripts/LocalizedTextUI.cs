using TMPro;
using UnityEngine;

// 言語設定に応じて TextMeshPro の文字列を切り替える
public class LocalizedTextUI : MonoBehaviour
{
    [SerializeField] private UiLanguageSettings languageSettings;
    [SerializeField] private TMP_Text targetText;
    [SerializeField] private string japaneseText;
    [SerializeField] private string englishText;

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

    public void Refresh()
    {
        if (targetText == null)
        {
            return;
        }

        UiLanguageSettings settings = GetLanguageSettings();
        bool useEnglish = settings != null && settings.UseEnglish;
        targetText.text = useEnglish ? englishText : japaneseText;
    }

    private UiLanguageSettings GetLanguageSettings()
    {
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
