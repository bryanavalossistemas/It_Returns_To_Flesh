using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static BehaviourPlus;

public class SkillButton : MonoBehaviour, ISelectHandler,IDeselectHandler
{
    [SerializeField] private Image img;
    [SerializeField] private Selectable selectable;
    private int pos;
    //private bool canBeSelected=true;

    void Start()
    {
        pos = transform.GetSiblingIndex();
        uiManager.SetColor += SetColor;
    }

    public void SetColor(Color? color) => img.color = color ?? GetColor();
    Color GetColor()
    {
        if (gameManager.selectedSkill == pos) return uiManager._selectedColor;
        if (!true) return uiManager._disabledColor;
        //if (gameManager.currentHealth <= 4) return uiManager._dangerColor;
        return uiManager._normalColor;
    }
    //public void Clear() => img.color = uiManager._normalColor;

    public void OnClick() => uiManager.SelectButton(pos);
    public void OnSelect(BaseEventData eventData)
    {
        gameManager.TriggerSkill(pos);
        SetColor(uiManager._selectedColor);
    }
    public void OnDeselect(BaseEventData eventData) => SetColor(uiManager._normalColor);
    public void Select() => selectable.Select();
}