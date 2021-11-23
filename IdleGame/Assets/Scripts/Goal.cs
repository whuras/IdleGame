using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class Goal
{
    public int level;
    public Color32 color32;
    public bool discovered;
    public VisualElement blockVisualElement;
}