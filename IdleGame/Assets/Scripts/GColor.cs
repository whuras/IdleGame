using System;
using UnityEngine;

public class GColor
{
    public GameManager gameManager;
    public int i { get; private set; }
    public int j { get; private set; }
    public int rValue { get; private set; }
    public int gValue { get; private set; }
    public int bValue { get; private set; }

    public Tuple<int, int, int> goalValues;

    public bool isGoalReached = false;
    public bool isRGoalReached = false;
    public bool isGGoalReached = false;
    public bool isBGoalReached = false;

    public void CheckGoal()
    {
        if (isRGoalReached && isGGoalReached && isBGoalReached)
        {
            isGoalReached = true;
            gameManager.currencyManager.IncrementPixelPoints();
            gameManager.gradientManager.numberCompleted += 1;
            gameManager.uiManager.UpdateLabelText();
        }

    }

    public void IncrementRValue(int amount)
    {
        if (isRGoalReached)
            return;

        rValue += amount;
        if (rValue > goalValues.Item1)
            rValue = goalValues.Item1;
    }

    public void IncrementGValue(int amount)
    {
        if (isGGoalReached)
            return;

        gValue += amount;
        if (gValue > goalValues.Item2)
            gValue = goalValues.Item2;
    }

    public void IncrementBValue(int amount)
    {
        if (isBGoalReached)
            return;

        bValue += amount;
        if (bValue > goalValues.Item3)
            bValue = goalValues.Item3;
    }

    public GColor()
    {
        i = 0;
        j = 0;

        rValue = 0;
        gValue = 0;
        bValue = 0;

        goalValues = new Tuple<int, int, int>(0, 0, 0);
    }

    public GColor(int i, int j, Tuple<int, int, int> goalValues)
    {
        this.i = i;
        this.j = j;

        rValue = 0;
        gValue = 0;
        bValue = 0;

        this.goalValues = goalValues;
    }
    public GColor(int i, int j, int rValue, int gValue, int bValue, Tuple<int, int, int> goalValues)
    {
        this.i = i;
        this.j = j;

        this.rValue = rValue;
        this.gValue = gValue;
        this.bValue = bValue;

        this.goalValues = goalValues;
    }

    public Color32 CurrentColor()
    {
        return new Color32(
            (byte)rValue,
            (byte)gValue,
            (byte)bValue,
            255
            );
    }

    public Color32 GoalColor()
    {
        return new Color32(
            (byte)goalValues.Item1,
            (byte)goalValues.Item2,
            (byte)goalValues.Item3,
            255
            );
    }

    public override string ToString()
    {
        return "(" + rValue + ", " + gValue + ", " + bValue + "), goal: " + goalValues.ToString();
    }
}
