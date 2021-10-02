using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkerUpgrade : MonoBehaviour
{
    [Header("Managers")]
    public CurrencyManager currencyManager;
    public Worker myWorker;
    public Button workerButton;

    [Header("Manual Upgrades")]
    public Button manualProductionMultiplierButton; // increase bytes per full progress bar
    public float manualProductionMultiplier = 1;
    public int manualProductionMultiplierCost = 1;
    public float manualProductionPenalty = 0.1f;
    
    //public Button manualTickMultiplierButton; // not implemented
    //public float manualTickMultiplier = 1; // not implemented
    //public int manualTickMultiplierCost = 1; // not implemented

    [Header("Automation Unlock")]
    public Button automationButton; // enabled automation
    public bool automationUnlocked = false; // change with prestige points
    public bool autoEnabled = false;
    public int automationCost = 10;
    
    [Header("Automation Upgrades")]
    public Button autoTickSpeedMuiltiplierButton; // speed of tick 
    public float autoTickSpeedMultiplier = 1;
    public int autoTickSpeedMultiplierCost = 1;

    public Button autoProductionMultiplierButton;
    public float autoProductionMultiplier = 1;
    public int autoProductionMultiplierCost = 1;

    //public Button autoTickAmountMultiplierButton; // % increase per tick // not implemented
    //public float autoTickAmountMultiplier; // not implemented
    //public int autoTickAmountMultipliercost; // not implemented

    [Header("Recycle Upgrades")]
    public bool recycleEnabled = false;
    public Button recycleButton;
    public float recycleMultiplier = 1;
    public int recycleMultiplierCost = 10;


    private void Awake()
    {
        if (myWorker == null)
            myWorker = GetComponent<Worker>();
    }

    public void ButtonSetup()
    {
        workerButton.clickable.clicked += myWorker.ManualIncrement;
        automationButton.clickable.clicked += AutomationButton;
        autoProductionMultiplierButton.clickable.clicked += AutoProductionMultiplierButton;
        autoTickSpeedMuiltiplierButton.clickable.clicked += AutoTickMultiplierButton;
        manualProductionMultiplierButton.clickable.clicked += ManualProductionMultiplierButton;
        recycleButton.clickable.clicked += RecycleButton;

        automationButton.Q<Label>().text = automationCost.ToString();
        autoProductionMultiplierButton.Q<Label>().text = autoProductionMultiplierCost.ToString();
        autoTickSpeedMuiltiplierButton.Q<Label>().text = autoTickSpeedMultiplierCost.ToString();
        manualProductionMultiplierButton.Q<Label>().text = manualProductionMultiplierCost.ToString();
        recycleButton.Q<Label>().text = recycleMultiplierCost.ToString();
    }

    private void AutomationButton()
    {
        if (currencyManager.pixelPoints >= automationCost)
        {
            if (autoEnabled)
            {
                currencyManager.PurchaseWithPixelPoints(automationCost);
                autoEnabled = false;
                Debug.Log("Automation for " + transform.name + " has been DISABLED!");
            }
            else
            {
                currencyManager.PurchaseWithPixelPoints(automationCost);
                autoEnabled = true;
                Debug.Log("Automation for " + transform.name + " has been ENABLED!");
            }
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

    private void AutoProductionMultiplierButton()
    {
        if (currencyManager.pixelPoints >= autoProductionMultiplierCost)
        {
            currencyManager.PurchaseWithPixelPoints(autoProductionMultiplierCost);


            Debug.Log("Auto Prod. Multiplier for " + transform.name + " has been purchased! Now production: " + autoTickSpeedMultiplier);
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Automated Prod. Increase.");
        }
    }

    private void ManualProductionMultiplierButton()
    {
        if (currencyManager.pixelPoints >= manualProductionMultiplierCost)
        {
            currencyManager.PurchaseWithPixelPoints(manualProductionMultiplierCost);

            Debug.Log("Manual Prod. Multiplier for " + transform.name + " has been purchased!");
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Manual Prod. Increase.");
        }
    }

    private void AutoTickMultiplierButton()
    {
        Debug.Log("Auto Tick Multiplier for " + transform.name + " clicked.");
    }
}
