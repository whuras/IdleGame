using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


// Credit: https://forum.unity.com/threads/tooltips-in-ui-toolkit.912896/
public class ToolTipManipulator : Manipulator
{
    private VisualElement rootVisualElement;
    private VisualElement element;
    public ToolTipManipulator(VisualElement rootVE)
    {
        rootVisualElement = rootVE;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseEnterEvent>(MouseIn);
        target.RegisterCallback<MouseOutEvent>(MouseOut);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseEnterEvent>(MouseIn);
        target.UnregisterCallback<MouseOutEvent>(MouseOut);
    }

    private void MouseIn(MouseEnterEvent e)
    {
        if (element == null)
        {
            element = new VisualElement();
            element.style.backgroundColor = Color.blue;
            element.style.position = Position.Absolute;
            element.style.left = this.target.worldBound.center.x;
            element.style.top = this.target.worldBound.yMin;
            var label = new Label(this.target.tooltip);
            label.style.color = Color.white;

            element.Add(label);
            var root = rootVisualElement;
            root.Add(element);

        }
        element.style.visibility = Visibility.Visible;
        element.BringToFront();
    }

    private void MouseOut(MouseOutEvent e)
    {
        element.style.visibility = Visibility.Hidden;
    }
}
