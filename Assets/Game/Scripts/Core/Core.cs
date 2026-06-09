using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class Core : MonoBehaviour
{
    private static bool NotNull;
    public static string[] LocaleNames { get; private set; }
    public static string CurrentLocaleName => LocalizationSettings.SelectedLocale.LocaleName;
    [SerializeField] private GameObject gameplayOnly;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;

    void Awake()
    {
        if (NotNull == true) Destroy(gameObject);
        else
        {
            NotNull = true;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoaded;
            BehaviourPlus.Init(gameManager, uiManager);
        }
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

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        gameplayOnly.SetActive(scene.buildIndex > 1);
        Time.timeScale = 1f;
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