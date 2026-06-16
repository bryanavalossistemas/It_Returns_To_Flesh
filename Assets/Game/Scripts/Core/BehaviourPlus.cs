public static class BehaviourPlus
{
#pragma warning disable IDE1006 // Estilos de nombres
    public static GameManager gameManager {  get; private set; }
    public static InputManager inputManager { get; private set; }
    public static SelectionManager selectionManager { get; private set; }
    public static UIManager uiManager { get; private set; }
#pragma warning restore IDE1006 // Estilos de nombres

    public static bool Init(GameManager gm, InputManager im, SelectionManager sm, UIManager um)
    {
        if (gameManager != null) return false;
        gameManager = gm;
        inputManager = im;
        selectionManager = sm;
        uiManager = um;
        return true;
    }
}