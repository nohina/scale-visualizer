using System;
using UnityEngine;

// UI 全体で共有する言語設定
public class UiLanguageSettings : MonoBehaviour
{
    private static UiLanguageSettings instance;

    [SerializeField] private UiLanguage language = UiLanguage.Japanese;

    public static UiLanguageSettings Instance
    {
        get { return instance; }
    }

    public UiLanguage Language
    {
        get { return language; }
    }

    public bool UseEnglish
    {
        get { return language == UiLanguage.English; }
    }

    public event Action<UiLanguage> LanguageChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void SetLanguage(UiLanguage value)
    {
        if (language == value)
        {
            return;
        }

        language = value;
        if (LanguageChanged != null)
        {
            LanguageChanged(language);
        }
    }
}

public enum UiLanguage
{
    Japanese = 0,
    English = 1
}
