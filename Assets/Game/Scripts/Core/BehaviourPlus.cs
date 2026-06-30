public static class BehaviourPlus
{
#pragma warning disable IDE1006 // Estilos de nombres
    public static Core core { get; private set; }
    public static GameManager gameManager {  get; private set; }
    public static InputManager inputManager { get; private set; }
    public static AudioManager audioManager { get; private set; }
    public static UIManager uiManager { get; private set; }
    public static PrefsManager prefsManager { get; private set; }
#pragma warning restore IDE1006 // Estilos de nombres

    public static bool Init(Core c, GameManager gm, InputManager im, AudioManager am, UIManager um, PrefsManager pm)
    {
        if (core != null) return false;
        bool valid = true;
        if (c  == null) { UnityEngine.Debug.LogError("BehaviourPlus.Init: Core is null");         valid = false; }
        if (gm == null) { UnityEngine.Debug.LogError("BehaviourPlus.Init: GameManager is null");  valid = false; }
        if (im == null) { UnityEngine.Debug.LogError("BehaviourPlus.Init: InputManager is null"); valid = false; }
        if (am == null) { UnityEngine.Debug.LogError("BehaviourPlus.Init: AudioManager is null"); valid = false; }
        if (um == null) { UnityEngine.Debug.LogError("BehaviourPlus.Init: UIManager is null");    valid = false; }
        if (pm == null) { UnityEngine.Debug.LogError("BehaviourPlus.Init: PrefsManager is null"); valid = false; }
        if (!valid) return false;
        core = c;
        gameManager = gm;
        inputManager = im;
        audioManager = am;
        uiManager = um;
        prefsManager = pm;
        return true;
    }
}