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

            UpdateProgress();
        }
    }

    public void UpdateProgress()
    {

    }

    public void UpdateText()
    {
        progressFoldout.text = "PROGRESS [ " + (numberComplete / totalGColors).ToString("F2") + "% ]";
    }

}
