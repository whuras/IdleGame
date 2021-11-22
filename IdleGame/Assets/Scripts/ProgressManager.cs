using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

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

    public int currentLevel = 1;
    private int maxLevel;
    public List<Goal> levelGoals;

    private void Awake()
    {
        levelGoals = new List<Goal>() // MUST be called in start or awake.. unity brakes otherwise..
        {
        // Level 1 - Tutorial 1 - 2x2
        { new Goal{level = 1, color32 = new Color32(0, 0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(128, 0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(255, 0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(0, 128, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(0, 255, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(0, 0, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(0, 0, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(255, 255, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },

        // Level 2 - 4x4
        { new Goal{level = 2, color32 = new Color32(64, 64, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(128, 128, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(192, 192, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(255, 255, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(64, 0, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(128, 0, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(192, 0, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(255, 0, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(0, 64, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(0, 128, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(0, 192, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(0, 255, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(64, 64, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(128, 128, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(192, 192, 192, 255), discovered = false, blockVisualElement = new VisualElement()} }        
        };

        maxLevel = levelGoals[levelGoals.Count - 1].level;
    }

    public void CompleteGColor(GColor gColor)
    {
        gameManager.effectManager.GradientBurstAtVE(gameManager.uiManager.activeGradientVisualElements[gColor.i, gColor.j], gColor);

        Color color32 = gColor.ToColor32();
        int r = gColor.rValue;
        int g = gColor.gValue;
        int b = gColor.bValue;

        if (!goals[r, g, b])
        {
            goals[r, g, b] = true;
            totalGColors += 1;   
        }

        for (int i = 0; i < levelGoals.Count; i++)
        {
            Goal goal = levelGoals[i];
            if (goal.color32 == color32)
            {
                goal.discovered = true;
                goal.blockVisualElement.style.backgroundColor = (Color) goal.color32;
                CheckCurrentLevel();
                gameManager.uiManager.UpdateProgressUI();
            }
        }
    }

    public void CheckCurrentLevel()
    {
        int level = 0;
        bool release = false;

        while (level <= maxLevel && !release)
        {
            level += 1;
            IEnumerable<Goal> query = levelGoals.Where(goal => goal.level == level);
            foreach (Goal goal in query)
            {
                if (goal.discovered == false)
                {
                    release = true;
                    break;
                }
            }
        }
        
        currentLevel = level;
    }

    // For save load, after uiManager assigns
    public void UpdateAllBlocks()
    {
        for (int i = 0; i < levelGoals.Count; i++)
        {
            Goal goal = levelGoals[i];
            if (goal.discovered)
                goal.blockVisualElement.style.backgroundColor = (Color) goal.color32;
        }
    }

    public void UpdateText()
    {
        progressFoldout.text = "PROGRESS GOALS [ " + (numberComplete / totalGColors).ToString("F2") + "% ]";
    }

}