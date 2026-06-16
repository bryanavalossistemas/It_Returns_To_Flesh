public static class BehaviourPlus
{
#pragma warning disable IDE1006 // Estilos de nombres
    public static GameManager gameManager {  get; private set; }
    public static InputHandler inputHandler { get; private set; }
    public static UIManager uiManager { get; private set; }
#pragma warning restore IDE1006 // Estilos de nombres

    public static bool Init(GameManager gm, InputHandler ih, UIManager um)
    {
        if (gameManager != null) return false;
        gameManager = gm;
        inputHandler = ih;
        uiManager = um;
        return true;
    }
}