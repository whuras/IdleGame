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

    [Header("General")]
    public int productionAmount = 255;

    [Header("Manual")]
    public int manualTickPercentStarting = 10;
    public int ManualTickPercent() => Mathf.FloorToInt(manualTickPercentStarting * workerUpgrade.productionMultiplier);

    [Header("Automation")]
    public float autoTickAmount = 1; // not upgradable yet - 1 is 1% per tick
    public float autoTickSpeed = 1;


    private void Update()
    {
        if (workerUpgrade.autoEnabled)
        {
            timer += Time.deltaTime;
            if (timer >= (autoTickAmount / (autoTickSpeed * workerUpgrade.autoTickSpeedMultiplier)))
            {
                IncrementProgressBar();
                timer = 0f;
            }
        }
    }

    public void IncrementProgressBar()
    {
        if (!keepWorking)
        {
            progressBarVisualElement.style.width = new Length(100f, LengthUnit.Percent);
            return;
        }

        var prevWidth = progressBarVisualElement.style.width.value;
        var newWidth = prevWidth.value + new Length(ManualTickPercent(), LengthUnit.Percent).value;

        if (newWidth >= 100.0f && keepWorking)
        {
            AddByte((int)(productionAmount * workerUpgrade.productionMultiplier));
            newWidth = newWidth - 100f;
        }

        progressBarVisualElement.style.width = keepWorking ? new Length(newWidth, LengthUnit.Percent) : new Length(100f, LengthUnit.Percent);
        gameManager.uiManager.UpdateLevelCompletionText();
    }

    public void AddByte(int byteAmount)
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

            if(queue.Count <= 0)
                keepWorking = false;
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

            if (queue.Count <= 0)
                keepWorking = false;
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

            if (queue.Count <= 0)
                keepWorking = false;
        }
        else
        {
            Debug.LogError("Worker->AddByte: Invalid worker.");
        }

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }
}
