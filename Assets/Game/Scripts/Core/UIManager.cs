using EditorAttributes;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BehaviourPlus;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] public Color _normalColor = Color.white, _dangerColor = Color.red, _disabledColor = new(0.2f, 0.2f, 0.2f, 1f), _selectedColor = Color.green;
    [SerializeField, TypeFilter(typeof(IColor))] private Component[] skillButtons;
    private IColor[] skills;

    void Awake() => SceneManager.sceneLoaded += SceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= SceneLoaded;

    private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (skills != null) ClearUI();
    }

    void Start() => skills = skillButtons.Select(s => s.GetComponent<IColor>()).ToArray();

    public void ClearUI()
    {
        for (int i = 0; i < skills.Length; i++) skills[i].SetColor(_normalColor);
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        //for (int i = 0; i < skills.Length; i++) skills[i].SetColor(GetColor(i));

        static Color GetColor(int i)
        {
            if (gameManager.selectedSkill == i) return uiManager._selectedColor;
            if (!true) return uiManager._disabledColor;
            //if (gameManager.currentHealth <= 4) return uiManager._dangerColor;
            return uiManager._normalColor;
        }
    }

    public void ResetSkillHighlight()
    {
        gameManager.selectedSkill = -1;
        //if (FleshRipper.SelectedRipper != null) EvaluateSkills(FleshRipper.SelectedRipper.Health);
        //else ClearUI();
    }

    public void SelectButton(int pos)
    {
        gameManager.selectedSkill = pos;
        skills[pos].PaintSelected();
    }
    /*public void NextButtonGO()
    {
        currentButtonGO = currentButtonGO.FindSelectableOnDown();
        currentButtonGO.Select();
    }*/
}