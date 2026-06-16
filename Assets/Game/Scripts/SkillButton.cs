#if UNITY_EDITOR
using System;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static BehaviourPlus;

public class SkillButton : MonoBehaviour, ISelectHandler,IDeselectHandler
{
    [Header("Punteros")]
    [SerializeField] private Image img;
    [SerializeField] private Selectable selectable;
    private int pos;
    private bool canBeSelected=true;

    void Start()
    {
        img.color = uiManager._disabledColor;
        pos = transform.GetSiblingIndex();
    }

    public void EvaluateSkill(int currentSkill, int currentHealth)
    {
        img.color = GetColor();

        Color GetColor()
        {
            if (currentSkill == pos) return uiManager._selectedColor;
            if (!canBeSelected) return uiManager._disabledColor;
            if (currentHealth <= 4) return uiManager._dangerColor;
            return uiManager._normalColor;
        }
    }

    public void Clear() => img.color = uiManager._normalColor;
    public void OnClick() => uiManager.SetButtonGO(selectable);
    public void OnSelect(BaseEventData eventData)
    {
        UIManager._currentHighlightedSkill = pos;
        img.color = uiManager._selectedColor;
        gameManager.TriggerSkill(pos);
    }
    public void OnDeselect(BaseEventData eventData)
    {

    }
}