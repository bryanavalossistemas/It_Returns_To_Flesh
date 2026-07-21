using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
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
        _input.Refresh();
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LocaleNames = LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.LocaleName).ToArray();
        LoadPlayerPrefs();
    }

    public static void LoadPlayerPrefs()
    {
        AudioManager.SetMasterVolume(PlayerPrefs.GetFloat("vol_master", 1f));
        AudioManager.SetMusicVolume(PlayerPrefs.GetFloat("vol_music", 1f));
        AudioManager.SetSfxVolume(PlayerPrefs.GetFloat("vol_sfx", 1f));
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

    public static void SetSelectedGO(GameObject go)
    {
        EventSystem.current.SetSelectedGameObject(go);
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