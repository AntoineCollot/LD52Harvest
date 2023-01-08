using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleButton : HighlightButton
{
    RectTransform rectT;
    float idleWidth;
    public float selectedWidth = 500;
    float refWidth;
    const float WIDTH_SMOOTH =0.05f;
    float targetWidth;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        rectT = transform as RectTransform;
        idleWidth = rectT.sizeDelta.x;
        targetWidth = idleWidth;
    }

    private void OnEnable()
    {
        isSelected = false;
        isHovered = false;
    }

    void Update()
    {
        if (SelectedChanged)
        {
            if (IsSelectedOrHovered)
            {
                targetWidth = selectedWidth;
            }
            else
            {
                targetWidth = idleWidth;
            }
        }

        Vector2 sizeDelta = rectT.sizeDelta;
        sizeDelta.x = Mathf.SmoothDamp(sizeDelta.x, targetWidth, ref refWidth, WIDTH_SMOOTH);
        rectT.sizeDelta = sizeDelta;
    }

    protected override void OnHighlighted()
    {
        base.OnHighlighted();
    }

    protected override void OnUnhighlighted()
    {
    }
}
