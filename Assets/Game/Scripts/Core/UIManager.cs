using TMPro;
using UnityEngine;
using static BehaviourPlus;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private SkillButton[] skills;
    [SerializeField] public Color _normalColor = Color.white, _dangerColor = Color.red, _disabledColor = new(0.2f, 0.2f, 0.2f, 1f), _selectedColor = Color.green;

    void Start() => ClearUI();

    public void ClearUI()
    {
        healthText.text = "--/--";
        foreach (SkillButton skill in skills) skill.SetColor(_normalColor);
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        EvaluateSkills(currentHealth);
    }

    private void EvaluateSkills(int currentHealth)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i].SetColor(GetColor());

            Color GetColor()
            {
                if (gameManager.selectedSkill == i) return uiManager._selectedColor;
                if (!true) return uiManager._disabledColor;
                if (currentHealth <= 4) return uiManager._dangerColor;
                return uiManager._normalColor;
            }
        }
    }

    public void ResetSkillHighlight()
    {
        gameManager.selectedSkill = -1;
        if (FleshRipper.SelectedRipper != null)
        {
            EvaluateSkills(FleshRipper.SelectedRipper.Health);
        }
        else ClearUI();
    }

    public void SelectButton(int pos)
    {
        gameManager.selectedSkill = pos;
        skills[pos].Select();
    }
    /*public void NextButtonGO()
    {
        currentButtonGO = currentButtonGO.FindSelectableOnDown();
        currentButtonGO.Select();
    }*/
}