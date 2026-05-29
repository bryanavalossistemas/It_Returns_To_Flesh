using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Skills")]
    [SerializeField] private Image skill1Vomit;
    [SerializeField] private Image skill2Sores;
    [SerializeField] private Image skill3Explode;
    [SerializeField] private Image skill4Cephalic;
    [SerializeField] private Image skill5Frenzy;

    private Color _normalColor = Color.white;
    private Color _dangerColor = Color.red;
    private Color _disabledColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private Color _selectedColor = Color.green;

    private bool _canVomit = true;
    private bool _canSores = true;
    private bool _canExplode = true;
    private bool _canCephalic = true;
    private bool _canFrenzy = true;
    private int _currentHighlightedSkill = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        healthText.text = "Health:--/--";
        skill1Vomit.color = _disabledColor;
        skill2Sores.color = _disabledColor;
        skill3Explode.color = _disabledColor;
        skill4Cephalic.color = _disabledColor;
        skill5Frenzy.color = _disabledColor;
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = "Health:" + currentHealth + "/" + maxHealth;
        EvaluateSkills(currentHealth);
    }

		public void SetHealthTextToVoid()
    {
        healthText.text = "Health:--/--";
    }

    private void EvaluateSkills(int currentHealth)
    {
        int vomitCost = 1;
        int soresCost = 2;
        int explodeCost = 4;
        int cephalicCost = 5;
        int frenzyCost = 3;

        if (_currentHighlightedSkill == 1) skill1Vomit.color = _selectedColor;
        else if (!_canVomit) skill1Vomit.color = _disabledColor;
        else skill1Vomit.color = (vomitCost >= currentHealth) ? _dangerColor : _normalColor;

        if (_currentHighlightedSkill == 2) skill2Sores.color = _selectedColor;
        else if (!_canSores) skill2Sores.color = _disabledColor;
        else skill2Sores.color = (soresCost >= currentHealth) ? _dangerColor : _normalColor;

        if (_currentHighlightedSkill == 3) skill3Explode.color = _selectedColor;
        else if (!_canExplode) skill3Explode.color = _disabledColor;
        else skill3Explode.color = (explodeCost >= currentHealth) ? _dangerColor : _normalColor;

        if (_currentHighlightedSkill == 4) skill4Cephalic.color = _selectedColor;
        else if (!_canCephalic) skill4Cephalic.color = _disabledColor;
        else skill4Cephalic.color = (cephalicCost >= currentHealth) ? _dangerColor : _normalColor;

        if (_currentHighlightedSkill == 5) skill5Frenzy.color = _selectedColor;
        else if (!_canFrenzy) skill5Frenzy.color = _disabledColor;
        else skill5Frenzy.color = (frenzyCost >= currentHealth) ? _dangerColor : _normalColor;

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
