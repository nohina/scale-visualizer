using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 言語切替用のセレクトボックス UI
public class LanguageSelectionUI : MonoBehaviour
{
    [SerializeField] private UiLanguageSettings languageSettings;
    [SerializeField] private TMP_Dropdown languageDropdown;

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

    public void Initialize()
    {
        UiLanguageSettings settings = GetLanguageSettings();
        if (languageDropdown == null || settings == null)
        {
            return;
        }

        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(new List<string> { "日本語", "English" });
        languageDropdown.SetValueWithoutNotify((int)settings.Language);
        languageDropdown.onValueChanged.RemoveListener(OnLanguageChangedFromDropdown);
        languageDropdown.onValueChanged.AddListener(OnLanguageChangedFromDropdown);
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

    private void OnLanguageChangedFromDropdown(int index)
    {
        UiLanguageSettings settings = GetLanguageSettings();
        if (settings == null)
        {
            return;
        }

        settings.SetLanguage((UiLanguage)index);
    }

    private void OnLanguageChanged(UiLanguage language)
    {
        if (languageDropdown != null)
        {
            languageDropdown.SetValueWithoutNotify((int)language);
        }
    }
}
