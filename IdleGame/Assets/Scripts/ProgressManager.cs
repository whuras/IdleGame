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

    public int currentLevel = 0;
    private int maxLevel;
    public List<Goal> levelGoals;

    private void Awake()
    {
        levelGoals = new List<Goal>() // MUST be called in start or awake.. unity brakes otherwise..
        {
        // Level 1 - Tutorial 1 - 2x2
        { new Goal{level = 1, color = new Color(0, 0, 0), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color = new Color(128, 0, 0), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color = new Color(255, 0, 0), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color = new Color(0, 128, 0), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color = new Color(0, 255, 0), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color = new Color(0, 0, 128), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color = new Color(0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },

        // Level 2 - 4x4
        { new Goal{level = 2, color = new Color(128, 128, 0), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color = new Color(255, 255, 0), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color = new Color(128, 0, 128), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color = new Color(255, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color = new Color(0, 128, 128), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color = new Color(0, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color = new Color(128, 128, 128), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color = new Color(255, 255, 255), discovered = false, blockVisualElement = new VisualElement()} }
        };

        maxLevel = levelGoals[levelGoals.Count - 1].level;
    }

    public void CompleteGColor(GColor gColor)
    {
        Color color = gColor.ToColor();
        int r = gColor.rValue;
        int g = gColor.gValue;
        int b = gColor.bValue;

        if (!goals[r, g, b])
        {

            goals[r, g, b] = true;
            totalGColors += 1;

            for (int i = 0; i < levelGoals.Count; i++)
            {
                Goal goal = levelGoals[i];
                if (goal.discovered)
                    continue;

                if (goal.color == color)
                {
                    goal.discovered = true;
                    goal.blockVisualElement.style.backgroundColor = goal.color;
                }
            }
        }
    }

    public void CheckCurrentLevel()
    {
        int level = 0;
        while (level <= maxLevel)
        {
            level += 1;
            IEnumerable<Goal> query = levelGoals.Where(goal => goal.level == level);
            foreach (Goal goal in query)
            {
                if (goal.discovered == false)
                    break;
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
                goal.blockVisualElement.style.backgroundColor = goal.color;
        }
    }

    public void UpdateText()
    {
        progressFoldout.text = "PROGRESS [ " + (numberComplete / totalGColors).ToString("F2") + "% ]";
    }

}