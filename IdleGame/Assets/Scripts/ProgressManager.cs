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
    //public bool[,,] goals = new bool[256, 256, 256];
    public List<Vector3> goals = new List<Vector3>();

    public Foldout progressFoldout;

    public int currentLevel = 1;
    private int maxLevel;
    public List<Goal> levelGoals;

    public void CreateLevelGoals()
    {
        levelGoals = new List<Goal>() // MUST be called in start or awake.. unity brakes otherwise..
        {
        // Level 1 - Tutorial 1 Monochrome - 2x2
        { new Goal{level = 1, color32 = new Color32(0, 0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(128, 128, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 1, color32 = new Color32(255, 255, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },

        // Level 2 - Tutorial 2 Basic RGB - 2x2
        { new Goal{level = 2, color32 = new Color32(128, 0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(255, 0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(0, 128, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(0, 255, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(0, 0, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 2, color32 = new Color32(0, 0, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },

        // Level 3 - Secondaries 4x4
        { new Goal{level = 3, color32 = new Color32(128, 128, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 3, color32 = new Color32(255, 255, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 3, color32 = new Color32(128, 0, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 3, color32 = new Color32(255, 0, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 3, color32 = new Color32(0, 128, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 3, color32 = new Color32(0, 255, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },

        // Level 4 - Tertiaries 8x8
        { new Goal{level = 4, color32 = new Color32(128, 255, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 4, color32 = new Color32(255, 128, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 4, color32 = new Color32(128, 0, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 4, color32 = new Color32(255, 0, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 4, color32 = new Color32(0, 128, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 4, color32 = new Color32(0, 255, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        
        // Level 5 - The Light Side of the Rainbow 16x16
        { new Goal{level = 5, color32 = new Color32(255, 255, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(255, 255, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(255, 192, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(255, 192, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(255, 192, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(255, 128, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(255, 128, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(255, 128, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(192, 255, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(192, 255, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(192, 255, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(192, 192, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(192, 192, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(192, 128, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(192, 128, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(192, 128, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(128, 255, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(128, 255, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(128, 255, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(128, 192, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(128, 192, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(128, 192, 128, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(128, 128, 255, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 5, color32 = new Color32(128, 128, 192, 255), discovered = false, blockVisualElement = new VisualElement()} },


        // Level 6 - The Dark Side of the Rainbow 32x32
        { new Goal{level = 6, color32 = new Color32(0, 0, 32, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(0, 0, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(0, 32, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(0, 32, 32, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(0, 32, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(0, 64, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(0, 64, 32, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(0, 64, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(32, 0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(32, 0, 32, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(32, 0, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(32, 32, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(32, 32, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(32, 64, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(32, 64, 32, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(32, 64, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(64, 0, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(64, 0, 32, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(64, 0, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(64, 32, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(64, 32, 32, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(64, 32, 64, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(64, 64, 0, 255), discovered = false, blockVisualElement = new VisualElement()} },
        { new Goal{level = 6, color32 = new Color32(64, 64, 32, 255), discovered = false, blockVisualElement = new VisualElement()} }

        // 6+ is 64x64

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

        Vector3 checkGoal = new Vector3(r, g, b);
        if (!goals.Contains(checkGoal))
        {
            goals.Add(checkGoal);
            totalGColors += 1;
        }

        for (int i = 0; i < levelGoals.Count; i++)
        {
            Goal goal = levelGoals[i];
            if (!goal.discovered && goal.color32 == color32)
            {
                StartCoroutine(gameManager.uiManager.PopUp(600, 1500, 1200));

                goal.discovered = true;
                goal.blockVisualElement.style.backgroundColor = (Color)goal.color32;
                goal.blockVisualElement.Q<Label>().style.color = (Color)new Color32(0, 0, 0, 0);
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
            if(goal.discovered || goals.Contains(new Vector3(goal.color32.r, goal.color32.g, goal.color32.b)))
            {
                goal.discovered = true;
                goal.blockVisualElement.style.backgroundColor = (Color)goal.color32;
                goal.blockVisualElement.Q<Label>().style.color = (Color)new Color32(0, 0, 0, 0);
            }
        }
    }

    public void UpdateText()
    {
        progressFoldout.text = "PROGRESS GOALS [ " + (numberComplete / totalGColors).ToString("F10") + "% ]";
    }

}