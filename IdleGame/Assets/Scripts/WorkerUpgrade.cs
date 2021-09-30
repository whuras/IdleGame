using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkerUpgrade : MonoBehaviour
{
    public CurrencyManager currencyManager;
    public Worker myWorker;
    public Button workerButton;

    public Button automationButton;
    public int automationCost = 10;
    public bool unlockedAutomation = false;

    public Button increaseAutomatedProductionButton;
    public int automatedProductionIncreaseCost = 10;
    public int automatedProductionIncreaseAmount = 0;

    public Button speedIncreaseButton;
    public int speedIncreaseCost = 10;
    public float speedIncreaseAmount = 0;

    public Button increaseManualProductionButton;
    public int manualProductionIncreaseCost = 10;
    public int manualProductionIncreaseAmount = 0;

    public Button recycleButton;
    public int recycleCost = 10;
    public bool unlockedRecycle = false;

    private void Awake()
    {
        if (myWorker == null)
            myWorker = GetComponent<Worker>();
    }

    public void ButtonSetup()
    {
        workerButton.clickable.clicked += myWorker.ManualIncrement;
        automationButton.clickable.clicked += AutomationButton;
        increaseAutomatedProductionButton.clickable.clicked += AutomatedProductionIncreaseButton;
        speedIncreaseButton.clickable.clicked += SpeedIncreaseButton;
        increaseManualProductionButton.clickable.clicked += ManualProductionIncreaseButton;
        recycleButton.clickable.clicked += RecycleButton;

        automationButton.text = automationCost + "\nAutomation";
        increaseAutomatedProductionButton.text = automatedProductionIncreaseCost + "\n+1";
        speedIncreaseButton.text = speedIncreaseCost + "\n>>>";
        increaseManualProductionButton.text = manualProductionIncreaseCost + "\n+1";
        recycleButton.text = recycleCost + "\nRecycle";
    }

    private void AutomationButton()
    {
        if (currencyManager.pixelPoints >= automationCost)
        {
            currencyManager.PurchaseWithPixelPoints(automationCost);
            unlockedAutomation = true;
            automationButton.text = "Automation\nEnabled";
            automationButton.SetEnabled(false);
            Debug.Log("Automation for " + transform.name + " has been unlocked!");
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Automation.");
        }
    }

    private void RecycleButton()
    {
        Debug.Log("Recycle for " + transform.name + " clicked.");
    }

    private void AutomatedProductionIncreaseButton()
    {
        if (currencyManager.pixelPoints >= automatedProductionIncreaseCost)
        {
            currencyManager.PurchaseWithPixelPoints(automatedProductionIncreaseCost);
            Debug.Log("Automated Prod. Increase for " + transform.name + " has been purchased!");
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Automated Prod. Increase.");
        }
    }

    private void ManualProductionIncreaseButton()
    {
        if (currencyManager.pixelPoints >= manualProductionIncreaseCost)
        {
            currencyManager.PurchaseWithPixelPoints(manualProductionIncreaseCost);

            Debug.Log("Manual Prod. Increase for " + transform.name + " has been purchased!");
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Manual Prod. Increase.");
        }
    }

    private void SpeedIncreaseButton()
    {
        Debug.Log("Speed Increase for " + transform.name + " clicked.");
    }
}
