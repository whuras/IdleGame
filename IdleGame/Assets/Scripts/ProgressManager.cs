using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProgressManager : MonoBehaviour
{
    public GameManager gameManager;

    public int rNumberCompleted = 0;
    public int gNumberCompleted = 0;
    public int bNumberCompleted = 0;

    public float numberComplete = 0;
    public float totalGColors = 256 * 256 * 256;
    public bool[,,] goals = new bool[256, 256, 256];

    public Foldout progressFoldout;
    
    public Label rProgressLabel;
    public VisualElement rProgressBarVisualElement;

    public Label gProgressLabel;
    public VisualElement gProgressBarVisualElement;

    public Label bProgressLabel;
    public VisualElement bProgressBarVisualElement;

    public void CompleteGColor(GColor gColor)
    {
        int r = gColor.rValue;
        int g = gColor.gValue;
        int b = gColor.bValue;

        if(!goals[r, g, b])
        {
            goals[r, g, b] = true;
            numberComplete += 1;

            rNumberCompleted += r == 0 ? 0 : 1;
            gNumberCompleted += g == 0 ? 0 : 1;
            bNumberCompleted += b == 0 ? 0 : 1;

            UpdateText();
            UpdateProgressBars();
        }
    }

    public void UpdateText()
    {
        progressFoldout.text = "PROGRESS [ " + (numberComplete / totalGColors).ToString("F2") + "% ]";
        rProgressLabel.text = "Red Progress " + (rNumberCompleted / (totalGColors - 256 * 256)).ToString("F2") + "% [ " + rNumberCompleted + " / " + (totalGColors - 256 * 256).ToString("N0") + " ]";
        gProgressLabel.text = "Green Progress " + (gNumberCompleted / (totalGColors - 256 * 256)).ToString("F2") + "% [ " + gNumberCompleted + " / " + (totalGColors - 256 * 256).ToString("N0") + " ]";
        bProgressLabel.text = "Blue Progress " + (bNumberCompleted / (totalGColors - 256 * 256)).ToString("F2") + "% [ " + bNumberCompleted + " / " + (totalGColors - 256 * 256).ToString("N0") + " ]";
    }

    public void UpdateProgressBars()
    {
        rProgressBarVisualElement.style.width = new Length(rNumberCompleted / (totalGColors - 256 * 256), LengthUnit.Percent);
        gProgressBarVisualElement.style.width = new Length(gNumberCompleted / (totalGColors - 256 * 256), LengthUnit.Percent);
        bProgressBarVisualElement.style.width = new Length(bNumberCompleted / (totalGColors - 256 * 256), LengthUnit.Percent);
    }
}
