using System;
using UnityEngine;
using TMPro;
using static BehaviourPlus;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] public Color _normalColor = Color.white, _dangerColor = Color.red, _disabledColor = new(0.2f, 0.2f, 0.2f, 1f), _selectedColor = Color.green;
    public event Action<Color?> SetColor;

    //void Start() => ClearUI();

    public void ClearUI()
    {
        healthText.text = "--/--";
        SetColor(_normalColor);
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        SetColor(null);
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
        //skills[pos].Select();
    }
    /*public void NextButtonGO()
    {
        currentButtonGO = currentButtonGO.FindSelectableOnDown();
        currentButtonGO.Select();
    }*/
}