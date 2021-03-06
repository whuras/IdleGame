using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Worker : MonoBehaviour
{
    public GameManager gameManager;
    public WorkerUpgrade workerUpgrade;
    public Type myType;

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

    public enum Type
    {
        Red,
        Green,
        Blue
    }

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
        gameManager.gradientManager.CheckGradientStatus();

        if (elapsedTime > (autoTickSpeed * workerUpgrade.AutoTickSpeedMultiplier()))
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
        gameManager.gradientManager.CheckGradientStatus();
    }

    public void AddByte(int byteAmount)
    {
        List<GColor> queue = gameManager.gradientManager.GetQueue(myType);
        if (queue.Count > 0)
        {
            queue[0].IncrementValue(myType, byteAmount);
            gameManager.uiManager.UpdateVEColor(queue[0].i, queue[0].j);
            gameManager.gradientManager.SortQueue(myType);
        }

        if (keepWorking && gameManager.recycleEnabled)
        {
            List<Type> typeList = new List<Type>() { Type.Red, Type.Green, Type.Blue };
            for(int i = 0; i < workerUpgrade.recycleLevel; i++)
            {
                Type rndType = typeList[Random.Range(0, typeList.Count)];
                List<GColor> recycleQueue = gameManager.gradientManager.GetQueue(rndType);

                while(recycleQueue.Count <= 0)
                {
                    typeList.Remove(rndType);
                    
                    if (typeList.Count == 0)
                    {
                        keepWorking = false;
                        return;
                    }   

                    rndType = typeList[Random.Range(0, typeList.Count)];
                    recycleQueue = gameManager.gradientManager.GetQueue(rndType);
                }

                recycleQueue[0].IncrementValue(rndType, byteAmount);
                gameManager.uiManager.UpdateVEColor(recycleQueue[0].i, recycleQueue[0].j);
                gameManager.gradientManager.SortQueue(rndType);
            }
        }

        if (queue.Count <= 0)
            keepWorking = false;

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }
}
