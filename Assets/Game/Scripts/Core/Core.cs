using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using static BehaviourPlus;
using System.Threading.Tasks;

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

    async void Start()
    {
        await UniTask.WhenAll(Init(), LocalizationInit());
        NextScene();

        static async UniTask Init()
        {
            prefsManager.LoadPlayerPrefs();
        }

        static async UniTask LocalizationInit()
        {
            await LocalizationSettings.InitializationOperation.Task.AsUniTask();
            LocaleNames = LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.LocaleName).ToArray();
        }
    }

    public void ChangeLanguage(string localeName)
    {
        Locale newLocale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(l => l.LocaleName == localeName);
        LocalizationSettings.SelectedLocale = newLocale;
    }

    public void ChangeScene(int buildIndex) => SceneManager.LoadScene(buildIndex);
    public void ReloadScene() => ChangeScene(SceneManager.GetActiveScene().buildIndex);
    public void NextScene()
    {
        int n = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        ChangeScene(n);
    }

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