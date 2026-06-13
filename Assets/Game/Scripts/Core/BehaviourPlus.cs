public static class BehaviourPlus
{
    public static GameManager gameManager;
    public static UIManager uiManager;

    public static bool Init(GameManager gm, UIManager um)
    {
        if (gameManager != null) return false;
        gameManager = gm;
        uiManager = um;
        return true;
    }
}