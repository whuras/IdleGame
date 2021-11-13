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
    public bool keepWorking = true;

    [Header("General")]
    public int productionAmount = 255;

    [Header("Manual")]
    public int manualTickPercentStarting = 10;
    public int ManualTickPercent() => Mathf.FloorToInt(manualTickPercentStarting * workerUpgrade.ProductionMultiplier());

    [Header("Automation")]
    public float elapsedTime = 0;
    public float autoTickSpeed = 1; // how many seconds to fill the bar
    public bool ticking = false;


    private void Update()
    {
        if (gameManager.automationEnabled && workerUpgrade.buttonStatuses[workerUpgrade.automationButton] == WorkerUpgrade.UpgradeStatus.Enabled)
        {
            if (!ticking && keepWorking)
            {
                StartCoroutine(AutomatedProgressBar());
            }
            else if(!keepWorking)
            {
                progressBarVisualElement.style.width = new Length(100f, LengthUnit.Percent);
            }
        }
    }

    private IEnumerator AutomatedProgressBar()
    {
        ticking = true;
        float pWidth;

        while(elapsedTime < (autoTickSpeed * workerUpgrade.AutoTickSpeedMultiplier()))
        {
            if (workerUpgrade.buttonStatuses[workerUpgrade.automationButton] == WorkerUpgrade.UpgradeStatus.Disabled)
                break;

            pWidth = Mathf.Lerp(0, 100, elapsedTime / (autoTickSpeed * workerUpgrade.AutoTickSpeedMultiplier()));
            progressBarVisualElement.style.width = new Length(pWidth, LengthUnit.Percent);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameManager.uiManager.UpdateLevelCompletionText();
        if(elapsedTime > (autoTickSpeed * workerUpgrade.AutoTickSpeedMultiplier()))
        {
            AddByte(productionAmount);
            elapsedTime = 0;
        }
        ticking = false;
    }

    
    public void IncrementProgressBar()
    {
        if (keepWorking)
        {
            if (gameManager.automationEnabled && workerUpgrade.buttonStatuses[workerUpgrade.automationButton] == WorkerUpgrade.UpgradeStatus.Enabled)
            {
                elapsedTime += (ManualTickPercent() / 100f) * (autoTickSpeed * workerUpgrade.AutoTickSpeedMultiplier());
            }
            else
            {
                var prevWidth = progressBarVisualElement.style.width.value;
                var newWidth = prevWidth.value + new Length(ManualTickPercent(), LengthUnit.Percent).value;

                elapsedTime += (ManualTickPercent() / 100f) * (autoTickSpeed * workerUpgrade.AutoTickSpeedMultiplier());

                if (newWidth >= 100.0f && keepWorking)
                {
                    AddByte(productionAmount);
                    elapsedTime = 0;
                    newWidth = newWidth - 100f;
                }

                progressBarVisualElement.style.width = keepWorking ? new Length(newWidth, LengthUnit.Percent) : new Length(100f, LengthUnit.Percent);
            }
        }
        else if (!gameManager.automationEnabled)
        {
            progressBarVisualElement.style.width = new Length(100f, LengthUnit.Percent);
        }

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
