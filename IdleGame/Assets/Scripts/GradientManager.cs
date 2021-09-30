using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GradientManager : MonoBehaviour
{
    public static GradientManager Instance { get => instance; }
    private static GradientManager instance;
    public GameManager gameManager;

    public int numberCompleted = 0;
    public GColor[,] gradientGColors;
    public List<GColor> RQueue = new List<GColor>();
    public List<GColor> GQueue = new List<GColor>();
    public List<GColor> BQueue = new List<GColor>();

    private void Awake() => MaintainSingleInstance();

    private void CreateQueues(int size)
    {
        numberCompleted = 0;
        RQueue.Clear();
        GQueue.Clear();
        BQueue.Clear();

        int[] offset = new int[size * 2 - 1];
        int val = -size + 1;
        for (int i = 0; i < offset.Length; i++)
        {
            offset[i] = val;
            val += 1;
        }

        int[] range = new int[size];
        int val2 = size - 1;
        for (int i = 0; i < range.Length; i++)
        {
            range[i] = val2;
            val2 -= 1;
        }

        for (int i = 0; i < offset.Length; i++)
        {
            for (int j = 0; j < range.Length; j++)
            {
                int index = offset[i] + range[j];
                if (index < 0 || index >= size)
                    continue;

                RQueue.Add(gradientGColors[range[j], index]);
                GQueue.Add(gradientGColors[range[j], index]);
                BQueue.Add(gradientGColors[range[j], index]);
            }
        }
    }

    public void InitializeGradientGColors(int size, Tuple<int, int, int> startColor, Tuple<int, int, int> endColor)
    {
        if (gameManager.uiManager.IsSizeValid(size))
        {
            gradientGColors = new GColor[size, size];

            // range linked to offset
            int[] range = new int[size * 2 - 1];
            int val = -size + 1;
            for (int i = 0; i < range.Length; i++)
            {
                range[i] = val;
                val += 1;
            }

            // goalValues
            Tuple<int, int, int>[] goalValues = new Tuple<int, int, int>[size * 2 - 1];
            goalValues[0] = startColor;
            goalValues[goalValues.Length - 1] = endColor;
            Tuple<int, int, int> step = new Tuple<int, int, int>(
                Mathf.FloorToInt((endColor.Item1 - startColor.Item1) / (goalValues.Length - 1)),
                Mathf.FloorToInt((endColor.Item2 - startColor.Item2) / (goalValues.Length - 1)),
                Mathf.FloorToInt((endColor.Item3 - startColor.Item3) / (goalValues.Length - 1))
                );

            for (int i = 1; i < goalValues.Length - 1; i++)
                goalValues[i] = new Tuple<int, int, int>(
                    startColor.Item1 + step.Item1 * i,
                    startColor.Item2 + step.Item2 * i,
                    startColor.Item3 + step.Item3 * i
                    );

            int offset;
            for (int i = 0; i < goalValues.Length; i++)
            {
                offset = range[i];
                for (int j = 0; j < size; j++)
                {
                    if (j + offset >= size || j + offset < 0)
                        continue;

                    GColor gc = new GColor(j, j + offset, goalValues[i]);
                    gc.gameManager = gameManager;
                    gradientGColors[j, j + offset] = gc;
                }
            }

            CreateQueues(size);
        }
    }

    private void MaintainSingleInstance()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public void SortRQueue()
    {
        for (int i = 0; i < RQueue.Count; i++)
        {
            GColor gc = RQueue[i];
            if (gc.goalValues.Item1 - gc.rValue <= 0)
            {
                gc.isRGoalReached = true;
                gc.CheckGoal();
                RQueue.Remove(gc);
            }
        }
    }

    public void SortGQueue()
    {
        for (int i = 0; i < GQueue.Count; i++)
        {
            GColor gc = GQueue[i];
            if (gc.goalValues.Item2 - gc.gValue <= 0)
            {
                gc.isGGoalReached = true;
                gc.CheckGoal();
                GQueue.Remove(gc);
            }
        }
    }

    public void SortBQueue()
    {
        for (int i = 0; i < BQueue.Count; i++)
        {
            GColor gc = BQueue[i];
            if (gc.goalValues.Item3 - gc.bValue <= 0)
            {
                gc.isBGoalReached = true;
                gc.CheckGoal();
                BQueue.Remove(gc);
            }

        }
    }
}
