public static class BehaviourPlus
{
#pragma warning disable IDE1006 // Estilos de nombres
    public static Core core { get; private set; }
    public static GameManager gameManager {  get; private set; }
    public static InputManager inputManager { get; private set; }
    public static AudioManager audioManager { get; private set; }
    public static UIManager uiManager { get; private set; }
#pragma warning restore IDE1006 // Estilos de nombres

    public static bool Init(Core c, GameManager gm, InputManager im, AudioManager am, UIManager um)
    {
        if (core != null) return false;
        core = c;
        gameManager = gm;
        inputManager = im;
        audioManager = am;
        uiManager = um;
        return true;
    }
}