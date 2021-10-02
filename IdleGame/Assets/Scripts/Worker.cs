using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Worker : MonoBehaviour
{
    public GameManager gameManager;
    public WorkerUpgrade workerUpgrade;

    public VisualElement progressBarVisualElement;

    [Header("Progress Bar")]
    private float timer = 0f;
    public bool keepWorking = true;

    [Header("Manual")]
    public int manualTickPercent = 100; // not upgradable yet
    public int manualProductionAmount = 255;

    [Header("Automation")]
    public float autoTickAmount = 1; // not upgradable yet - 1 is 1% per tick
    public float autoTickSpeed = 1;
    public int autoProductionAmount= 10;


    private void Update()
    {
        if (workerUpgrade.autoEnabled)
        {
            timer += Time.deltaTime;
            if (timer >= (autoTickAmount / (autoTickSpeed * workerUpgrade.autoTickSpeedMultiplier)))
            {
                AutomatedIncrement();
                timer = 0f;
            }
        }
    }

    public void ManualIncrement()
    {
        var prevWidth = progressBarVisualElement.style.width.value;
        var newWidth = prevWidth.value + new Length(manualTickPercent, LengthUnit.Percent).value;

        if (newWidth >= 100.0f && keepWorking)
        {
            AddByte((int)(manualProductionAmount * workerUpgrade.manualProductionMultiplier));
            newWidth = newWidth - 100f;
        }

        progressBarVisualElement.style.width = new Length(newWidth, LengthUnit.Percent);
        gameManager.uiManager.UpdateLevelCompletionText();
    }

    public void AutomatedIncrement()
    {
        var prevWidth = progressBarVisualElement.style.width.value;
        var newWidth = prevWidth.value + new Length(autoTickAmount, LengthUnit.Percent).value;

        if (newWidth >= 100.0f && keepWorking)
        {
            AddByte((int)(autoProductionAmount * workerUpgrade.autoProductionMultiplier));
            newWidth = newWidth - 100f;
        }

        progressBarVisualElement.style.width = new Length(newWidth, LengthUnit.Percent);
        gameManager.uiManager.UpdateLevelCompletionText();
    }

    private void AddByte(int byteAmount)
    {
        if (this == gameManager.workerManager.workers[0])
        {
            List<GColor> queue = gameManager.gradientManager.RQueue;
            if (queue.Count > 0)
            {
                queue[0].IncrementRValue(byteAmount);
                gameManager.uiManager.UpdateVEColor(queue[0].i, queue[0].j);
                gameManager.gradientManager.SortRQueue();
            }
            else
            {
                keepWorking = false;
            }
        }
        else if (this == gameManager.workerManager.workers[1])
        {
            List<GColor> queue = gameManager.gradientManager.GQueue;
            if (queue.Count > 0)
            {
                queue[0].IncrementGValue(byteAmount);
                gameManager.uiManager.UpdateVEColor(queue[0].i, queue[0].j);
                gameManager.gradientManager.SortGQueue();
            }
            else
            {
                keepWorking = false;
            }
        }
        else if (this == gameManager.workerManager.workers[2])
        {
            List<GColor> queue = gameManager.gradientManager.BQueue;
            if (queue.Count > 0)
            {
                queue[0].IncrementBValue(byteAmount);
                gameManager.uiManager.UpdateVEColor(queue[0].i, queue[0].j);
                gameManager.gradientManager.SortBQueue();
            }
            else
            {
                keepWorking = false;
            }
        }
        else
        {
            Debug.LogError("Worker->AddByte: Invalid worker.");
        }
    }
}
