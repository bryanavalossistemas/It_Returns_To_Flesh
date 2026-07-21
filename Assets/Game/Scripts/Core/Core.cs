using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static BehaviourPlus;

public class Core : MonoBehaviour
{
    public const int MenusIndex = 1;
    [SerializeField] private GameManager _game;
    [SerializeField] private InputManager _input;
    [SerializeField] private AudioManager _audio;
    [SerializeField] private UIManager _ui;
    [SerializeField] private PrefsManager _prefs;
    [SerializeField] private GameObject eventSystem;
    public static string[] LocaleNames { get; private set; }
    public static string CurrentLocaleName => LocalizationSettings.SelectedLocale.LocaleName;
    public Camera MainCamera { get; private set; }
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private CameraController cameraController;

    void Awake()
    {
        if (!Init(this, _game, _input, _audio, _ui, _prefs))
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        await UniTask.WhenAll(Init(), LocalizationInit());
        if (SceneManager.GetActiveScene().buildIndex == 0) NextScene();

        async UniTask Init()
        {
            eventSystem.SetActive(true);
            MainCamera = Camera.main;
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

    public void LoadScene(int sceneIndex, TransitionMode loadMode = TransitionMode.None)
    {
        switch (loadMode)
        {
            case TransitionMode.None:
                SceneManager.LoadScene(sceneIndex);
                break;
            case TransitionMode.SlideRight:
                throw new NotImplementedException();
        }
    }
    public void LoadScene(SceneTypes sceneType, TransitionMode loadMode = TransitionMode.None) => LoadScene((int)sceneType, loadMode);
    public void ReloadScene() => LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void NextScene() => LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    public async void LoadSceneAsync(int sceneIndex, TransitionMode tMode, UniTask waitTask)
    {
        bool noTransition = tMode == TransitionMode.None;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex, noTransition? LoadSceneMode.Single : LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        await UniTask.WhenAll(waitTask, AsyncLoaded());
        op.allowSceneActivation = true;
        if (!noTransition) StartCoroutine(MakeTransition());

        async UniTask AsyncLoaded()
        {
            while (op.progress < 0.9f)
                await Task.Yield();
        }

        IEnumerator MakeTransition()
        {
            Scene oldScene = SceneManager.GetActiveScene(), newScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            while (!op.isDone)
                yield return null;

            Time.timeScale = 0f;
            //newScene.GetRootGameObjects().
            switch (tMode)
            {
                case TransitionMode.SlideRight:
                    yield return cameraController.Slide(Vector3.forward * -10, duration);
                    break;
            }
            SceneManager.SetActiveScene(newScene);
            Time.timeScale = 1f;
            SceneManager.UnloadSceneAsync(oldScene);
        }
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

public enum SceneTypes
{
    Menu = 1,
}
public enum TransitionMode
{
    None,
    SlideRight
}