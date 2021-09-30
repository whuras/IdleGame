using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Worker : MonoBehaviour
{
    public GameManager gameManager;
    public WorkerUpgrade workerUpgrade;

    public VisualElement progressBarVisualElement;

    private float timer = 0f;
    public float tickPerSecond = 1f;
    public int automatedIncrementAmount = 1;
    public int automatedByteAmount = 10;

    public int manualIncrementAmount = 100;
    public int manualByteAmount = 255;

    public bool keepWorking = true;

    private void Update()
    {
        if (workerUpgrade.unlockedAutomation)
        {
            timer += Time.deltaTime;
            if (timer >= (1f / tickPerSecond))
            {
                AutomatedIncrement();
                timer = 0f;
            }
        }
    }

    public void ManualIncrement()
    {
        var prevWidth = progressBarVisualElement.style.width.value;
        var newWidth = prevWidth.value + new Length(manualIncrementAmount, LengthUnit.Percent).value;

        if (newWidth > 100.0f && keepWorking)
        {
            AddByte(manualByteAmount);
            newWidth = newWidth - 100f;
        }

        progressBarVisualElement.style.width = new Length(newWidth, LengthUnit.Percent);
        gameManager.uiManager.UpdateLabelText();
    }

    public void AutomatedIncrement()
    {
        var prevWidth = progressBarVisualElement.style.width.value;
        var newWidth = prevWidth.value + new Length(automatedIncrementAmount, LengthUnit.Percent).value;

        if (newWidth > 100.0f && keepWorking)
        {
            AddByte(automatedByteAmount);
            newWidth = newWidth - 100f;
        }

        progressBarVisualElement.style.width = new Length(newWidth, LengthUnit.Percent);
        gameManager.uiManager.UpdateLabelText();
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
