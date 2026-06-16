using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private SkillButton[] skills;
    [SerializeField] public Color _normalColor = Color.white, _dangerColor = Color.red, _disabledColor = new(0.2f, 0.2f, 0.2f, 1f), _selectedColor = Color.green;
    public static int _currentHighlightedSkill = -1;
    private Selectable currentButtonGO;

    void Start() => ClearUI();

    public void ClearUI()
    {
        healthText.text = "--/--";
        foreach (SkillButton skill in skills) skill.Clear();
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        EvaluateSkills(currentHealth);
    }

    private void EvaluateSkills(int currentHealth)
    {
        foreach (SkillButton skill in skills) skill.EvaluateSkill(_currentHighlightedSkill, currentHealth);
    }

    public void ResetSkillHighlight()
    {
        _currentHighlightedSkill = -1;
        if (FleshRipper.SelectedRipper != null)
        {
            EvaluateSkills(FleshRipper.SelectedRipper.Health);
        }
        else ClearUI();
    }

    public void SetButtonGO(Selectable s) => currentButtonGO = s;
    public void NextButtonGO()
    {
        currentButtonGO = currentButtonGO.FindSelectableOnDown();
        currentButtonGO.Select();
    }
}
