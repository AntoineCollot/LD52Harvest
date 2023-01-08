using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class HighlightButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected bool isSelected;
    protected bool isHovered;
    bool lastSelected;

    public bool SelectedChanged => lastSelected != IsSelectedOrHovered;
    public bool IsSelectedOrHovered => isSelected || isHovered;

    protected virtual void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { SFXManager.PlaySound(GlobalSFX.ButtonClick); });
    }

    public virtual void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        if (!IsSelectedOrHovered)
            OnUnhighlighted();
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        EventSystem.current.SetSelectedGameObject(null);

        OnHighlighted();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        if (!IsSelectedOrHovered)
            OnUnhighlighted();
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        isSelected = true;

        OnHighlighted();
    }

    void LateUpdate()
    {
        lastSelected = IsSelectedOrHovered;
    }

    protected virtual void OnHighlighted()
    {
        SFXManager.PlaySound(GlobalSFX.ButtonHover);
    }
    protected abstract void OnUnhighlighted();
}
