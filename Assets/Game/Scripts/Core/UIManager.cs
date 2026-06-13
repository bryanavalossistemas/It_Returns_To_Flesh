using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Skills")]
    private Image[] skillImages;
    [SerializeField] private Image skill1Vomit;
    [SerializeField] private Image skill2Sores;
    [SerializeField] private Image skill3Explode;
    [SerializeField] private Image skill4Cephalic;
    [SerializeField] private Image skill5Frenzy;

    private Color _normalColor = Color.white, _dangerColor = Color.red, _disabledColor = new(0.2f, 0.2f, 0.2f, 1f), _selectedColor = Color.green;

    private bool _canVomit = true;
    private bool _canSores = true;
    private bool _canExplode = true;
    private bool _canCephalic = true;
    private bool _canFrenzy = true;
    private int _currentHighlightedSkill = -1;

    void Start()
    {
        healthText.text = "Health:--/--";
        skillImages = new Image[skillImages.Length];
        foreach (Image skill in skillImages) skill.color = _disabledColor;
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"Health: {currentHealth}/{maxHealth}";
        EvaluateSkills(currentHealth);
    }

    private void EvaluateSkills(int currentHealth)
    {
        (Func<int, bool> condition, Color color)[] rules =
        {
            (i => _currentHighlightedSkill == i, _selectedColor),
            (_ => false, _disabledColor), //!_canVomit
            (_ => int.MaxValue >= currentHealth, _dangerColor), //cost
            (_ => true, _normalColor)
        };
        for (int i = 0; i < skillImages.Length; i++)
        {
            foreach (var (condition, color) in rules)
            {
                if (condition(i)) skillImages[i].color = color;
                break;
            }
        }
    }

    public void ConfigureBiologicalLimits(bool vomit, bool sores,bool explode, bool cephalic, bool frenzy)
    {
        _canVomit = vomit;
        _canSores = sores;
        _canExplode = explode;
        _canCephalic = cephalic;
        _canFrenzy = frenzy;
    }

    public void ClearUI()
    {
        healthText.text = "Health:--/--";
        skill1Vomit.color = _normalColor;
        skill2Sores.color = _normalColor;
        skill3Explode.color = _normalColor;
        skill4Cephalic.color = _normalColor;
        skill5Frenzy.color = _normalColor;
    }

    public void ResetSkillHighlight()
    {
        _currentHighlightedSkill = -1;
        if (FleshRipper.SelectedRipper != null)
        {
            EvaluateSkills(FleshRipper.SelectedRipper.Health);
        }
        else
        {
            ClearUI();
        }
    }

    public void TriggerSkillVomit()
    {
        SelectionManager.CurrentState = SelectionManager.InputState.QuickCast;
        SelectionManager.PendingSkillID = 1;
        FleshRipper.SelectedRipper = null;
        ClearUI();
        _currentHighlightedSkill = 1;
        skill1Vomit.color = _selectedColor;
    }

    public void TriggerSkillFrenzy()
    {
        SelectionManager.CurrentState = SelectionManager.InputState.QuickCast;
        SelectionManager.PendingSkillID = 5;
        FleshRipper.SelectedRipper = null;
        ClearUI();
        _currentHighlightedSkill = 5;
        skill5Frenzy.color = _selectedColor;
    }

    public void TriggerSkillExplosion()
    {
        SelectionManager.CurrentState = SelectionManager.InputState.TargetingLimb;
        FleshRipper.SelectedRipper = null;
        ClearUI();
        _currentHighlightedSkill = 3;
        skill3Explode.color = _selectedColor;
    }

    public void TriggerSkillSores()
    {
        SelectionManager.CurrentState = SelectionManager.InputState.QuickCast;
        SelectionManager.PendingSkillID = 2;
        FleshRipper.SelectedRipper = null;
        ClearUI();
        _currentHighlightedSkill = 2;
        skill2Sores.color = _selectedColor;
    }
}
