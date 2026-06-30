using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using static BehaviourPlus;

public class Core : MonoBehaviour
{
    public const int MenusIndex = 0;
    [SerializeField] private GameManager _game;
    [SerializeField] private InputManager _input;
    [SerializeField] private AudioManager _audio;
    [SerializeField] private UIManager _ui;
    [SerializeField] private PrefsManager _prefs;
    public static string[] LocaleNames { get; private set; }
    public static string CurrentLocaleName => LocalizationSettings.SelectedLocale.LocaleName;
    public Camera MainCamera { get; private set; }

    void Awake()
    {
        if (!Init(this, _game, _input, _audio, _ui, _prefs))
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        MainCamera = Camera.main;
    }

    void Start()
    {
        prefsManager.LoadPlayerPrefs();
        StartTask().Forget();

        static async UniTaskVoid StartTask()
        {
            try
            {
                await LocalizationSettings.InitializationOperation.Task.AsUniTask();
                LocaleNames = LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.LocaleName).ToArray();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Core: localization initialization failed: {ex}");
            }
        }
    }

    public void ChangeLanguage(string localeName)
    {
        Locale newLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(l => l.LocaleName == localeName);
        if (newLocale == null)
        {
            Debug.LogWarning($"Core: locale '{localeName}' not found; language unchanged");
            return;
        }
        LocalizationSettings.SelectedLocale = newLocale;
    }

    public void ChangeScene(int buildIndex) => SceneManager.LoadScene(buildIndex);
    public void ReloadScene() => ChangeScene(SceneManager.GetActiveScene().buildIndex);
    public void NextScene() => ChangeScene(SceneManager.GetActiveScene().buildIndex+1);

    public void QuitGame()
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