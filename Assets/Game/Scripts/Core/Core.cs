using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class Core : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private SelectionManager selectionManager;
    [SerializeField] private UIManager uiManager;
    public static string[] LocaleNames { get; private set; }
    public static string CurrentLocaleName => LocalizationSettings.SelectedLocale.LocaleName;

    void Awake()
    {
        if (!BehaviourPlus.Init(gameManager, inputManager, selectionManager, uiManager))
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LocaleNames = LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.LocaleName).ToArray();
        LoadPlayerPrefs();
    }

    public static void LoadPlayerPrefs()
    {
        AudioManager.SetMasterVolume(PlayerPrefs.GetInt("vol_master", 100));
        AudioManager.SetMusicVolume(PlayerPrefs.GetInt("vol_music", 100));
        AudioManager.SetSfxVolume(PlayerPrefs.GetInt("vol_sfx", 100));
    }

    public static void ChangeLanguage(string localeName)
    {
        Locale newLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(l => l.LocaleName == localeName);
        LocalizationSettings.SelectedLocale = newLocale;
    }

    public static void ChangeScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_WEBGL
        Application.Quit();
#else
        Debug.LogError("No compatible");
#endif
    }
}