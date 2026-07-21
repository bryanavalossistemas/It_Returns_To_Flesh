using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static BehaviourPlus;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    private VisualElement levelList;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        levelList = root.Q<VisualElement>("levels");
        SetupLevels();

        void SetupLevels()
        {
            LevelSO[] levels = gameManager.GetLevels();
            for (int i = 0; i < levels.Length; i++)
            {
                int index = i;
                LevelSO level = levels[i];
                levelList.Add(CreateLevelCard(index, level));
            }

            VisualElement CreateLevelCard(int i, LevelSO level)
            {
                VisualElement buttonLevel = new();
                buttonLevel.AddToClassList("buttonLevel");

                Button button = new()
                {
                    text = (i + 1).ToString()
                };
                Label label = new()
                {
                    text = level.levelName
                };
                label.AddToClassList("text_white");

                button.clicked += () => PlayLevel(i);

                buttonLevel.Add(button);
                buttonLevel.Add(label);
                return buttonLevel;
            }
        }
    }

    private void PlayLevel(int n) => gameManager.StartLevel(n);
}