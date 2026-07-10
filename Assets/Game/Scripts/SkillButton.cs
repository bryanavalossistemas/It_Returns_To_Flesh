using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static BehaviourPlus;

public class SkillButton : MonoBehaviour, IColor, ISelectHandler,IDeselectHandler
{
    [SerializeField] private Image img;
    [SerializeField] private Selectable selectable;
    private int pos;
    //private bool canBeSelected=true;

    void Start() => pos = transform.GetSiblingIndex();

    public void SetColor(Color color) => img.color = color;
    //public void Clear() => img.color = uiManager._normalColor;

    public void OnClick() => uiManager.SelectButton(pos);
    public void OnSelect(BaseEventData eventData)
    {
        gameManager.TriggerSkill(pos);
        SetColor(uiManager._selectedColor);
    }
    public void OnDeselect(BaseEventData eventData) => SetColor(uiManager._normalColor);
    public void PaintSelected() => selectable.Select();
}