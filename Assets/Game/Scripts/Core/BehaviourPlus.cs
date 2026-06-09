using UnityEngine;

public static class BehaviourPlus
{
    public static GameManager gameManager;
    public static UIManager uiManager;

    public static void Init(GameManager gm, UIManager um)
    {
        gameManager = gm;
        uiManager = um;
    }
}