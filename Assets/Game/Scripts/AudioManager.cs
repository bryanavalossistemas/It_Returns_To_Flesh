using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
#pragma warning disable IDE1006 // Estilos de nombres
    public static float masterVolume { get; private set; } = 1f;
    public static float musicVolume { get; private set; } = 1f;
    public static float sfxVolume { get; private set; } = 1f;
#pragma warning restore IDE1006 // Estilos de nombres
    private static Bus masterBus, musicBus, sfxBus;
    private static EventInstance bgmInstance;
    private static SongLibraries songs;
    [SerializeField] private SongLibraries _songLibrary;
    private static PARAMETER_ID id;

    void Awake() => songs = _songLibrary;

    void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/BGM");
        RuntimeManager.StudioSystem.getParameterDescriptionByName(songs.Parameter, out var desc);
        id = desc.id;
    }

    void OnEnable() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        ReleaseBGMInstance();
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode) => PlayBGM(songs.Scene_Themes[scene.name]);

    public static EventReference GetEventFromName(string bgmName) => songs.Events_BGM[bgmName];
    public static EventInstance CreateEventInstance(string bgmName) => CreateEventInstance(GetEventFromName(bgmName));
    public static EventInstance CreateEventInstance(EventReference soundEvent) => RuntimeManager.CreateInstance(soundEvent);

    public static void PlayBGM(string bgmName)
    {
        ReleaseBGMInstance(true);
        bgmInstance = CreateEventInstance(bgmName);
        bgmInstance.start();
    }

    private static void ReleaseBGMInstance(bool allowFadeOut = false)
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(allowFadeOut? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            bgmInstance.release();
        }
    }

    public void PlayOneShot(EventReference soundEvent, Vector2 position)
    {
        RuntimeManager.PlayOneShot(soundEvent, position);
    }

    public static void SetMasterVolume(float value)
    {
        masterVolume = value;
        masterBus.setVolume(masterVolume);
        PlayerPrefs.SetFloat("vol_master", value);
    }
    public static void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicBus.setVolume(musicVolume);
        PlayerPrefs.SetFloat("vol_music", value);
    }
    public static void SetSfxVolume(float value)
    {
        sfxVolume = value;
        sfxBus.setVolume(sfxVolume);
        PlayerPrefs.SetFloat("vol_sfx", value);
    }

    private static EventInstance stepEventInstance;
    public static void UpdateSound(bool playSfx)
    {
        //stepEventInstance = CreateEventInstance(playerStepSound);
        if (playSfx)
        {
            stepEventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                stepEventInstance.start();
            }
        }
        else
        {
            stepEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    public static void UpdatePlaystate()
    {
        RuntimeManager.StudioSystem.setParameterByID(id, Time.timeScale == 0? 1 : 0);
    }
}