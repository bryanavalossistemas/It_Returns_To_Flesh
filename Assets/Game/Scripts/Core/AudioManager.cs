using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using static BehaviourPlus;

public class AudioManager : MonoBehaviour
{
#pragma warning disable IDE1006 // Estilos de nombres
    [HideInInspector] public int masterVolume = 100, musicVolume = 100, sfxVolume = 100;
#pragma warning restore IDE1006 // Estilos de nombres
    private Bus masterBus, musicBus, sfxBus;
    private EventInstance bgmInstance;
    [SerializeField] private SongLibraries songs;
    [SerializeField, ParamRef] private string playState;
    private PARAMETER_ID pState;

    void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/BGM");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        RuntimeManager.StudioSystem.getParameterDescriptionByName(playState, out var desc);
        pState = desc.id;
    }

    void OnEnable() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        ReleaseBGMInstance();
    }
    private void SceneLoaded(Scene scene, LoadSceneMode loadMode) => PlayBGM(songs.Scene_Themes[scene.name]);

    public EventReference GetEventFromName(string bgmName) => songs.Events_BGM[bgmName];
    public EventInstance CreateEventInstance(string bgmName) => CreateEventInstance(GetEventFromName(bgmName));
    public EventInstance CreateEventInstance(EventReference soundEvent) => RuntimeManager.CreateInstance(soundEvent);

    private void ReleaseBGMInstance(bool allowFadeOut = false)
    {
        if (bgmInstance.isValid())
        {
            bgmInstance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            bgmInstance.release();
        }
    }

    public void PlayBGM(string bgmName)
    {
        ReleaseBGMInstance(true);
        bgmInstance = CreateEventInstance(bgmName);
        bgmInstance.start();
    }
    private void PlayOneShot(EventReference soundEvent, Vector2 position) => RuntimeManager.PlayOneShot(soundEvent, position);

    public void SetMasterVolume(int value) => SetVolume(ref masterVolume, masterBus, "vol_master", value);
    public void SetMusicVolume(int value) => SetVolume(ref musicVolume, musicBus, "vol_music", value);
    public void SetSfxVolume(int value) => SetVolume(ref sfxVolume, sfxBus, "vol_sfx", value);
    private void SetVolume(ref int volume, Bus bus, string key, int value)
    {
        volume = value;
        bus.setMute(volume == 0);
        bus.setVolume(value / 100f);
        prefsManager.SetInt(key, value);
    }

    public void PlaySfx(SFXEnum sfxEnum, Vector2 position)
    {
        EventReference sfx = sfxEnum switch
        {
            SFXEnum.Eating => songs.eatingSfx,
            SFXEnum.Frenzy => songs.frenzySfx,
            SFXEnum.Sores => songs.soresSfx,
            SFXEnum.GameOver => songs.gameOverSfx,
            SFXEnum.Explode => songs.explodeSfx,
            SFXEnum.Guillotine => songs.guillotineSfx,
            SFXEnum.RipperDead => songs.ripperDeadSfx,
            SFXEnum.Vomit => songs.vomitSfx,
            SFXEnum.DestroyableWall => songs.destroyableWallSfx
        };
        PlayOneShot(sfx, position);
    }

    /*private EventInstance stepEventInstance;
    public void UpdateSound(bool playSfx)
    {
        //stepEventInstance = CreateEventInstance(playerStepSound);
        if (playSfx)
        {
            stepEventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED)) stepEventInstance.start();
        }
        else stepEventInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }*/

    public void UpdatePlaystate() => RuntimeManager.StudioSystem.setParameterByID(pState, Time.timeScale == 0? 1 : 0);
}

public enum SFXEnum
{
    Eating,
    Frenzy,
    Sores,
    GameOver,
    Explode,
    Guillotine,
    RipperDead,
    DestroyableWall,
    Vomit
}