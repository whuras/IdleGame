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
    public int ManualProductionMultiplierCost() => (int)Mathf.Pow(2, manualProductionMultiplier - 1);
    public float manualProductionPenalty = 0.1f;
    
    //public Button manualTickMultiplierButton; // not implemented
    //public float manualTickMultiplier = 1; // not implemented
    //public int manualTickMultiplierCost = 1; // not implemented

    [Header("Automation Unlock")]
    public bool automationUnlocked = false; // change with prestige points
    public Button automationButton; // enabled automation
    public bool autoEnabled = false;
    public int automationCost = 10;
    
    [Header("Automation Upgrades")]
    public Button autoTickSpeedMuiltiplierButton; // speed of tick 
    public int autoTickSpeedLevel = 1;
    public float autoTickSpeedMultiplier = 1;
    public int AutoTickSpeedMultiplierCost() => (int)Mathf.Pow(2, autoTickSpeedLevel - 1);

    public Button autoProductionMultiplierButton;
    public float autoProductionMultiplier = 1;
    public int AutoProductionMultiplierCost() => (int) Mathf.Pow(2, autoProductionMultiplier - 1);

    //public Button autoTickAmountMultiplierButton; // % increase per tick // not implemented
    //public float autoTickAmountMultiplier; // not implemented
    //public int autoTickAmountMultipliercost; // not implemented

    [Header("Recycle Upgrades")]
    public bool recycleUnlocked = false;
    public Button recycleButton;
    public bool recycleEnabled = false;
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
        autoTickSpeedMuiltiplierButton.clickable.clicked += AutoTickSpeedMultiplierButton;
        manualProductionMultiplierButton.clickable.clicked += ManualProductionMultiplierButton;
        recycleButton.clickable.clicked += RecycleButton;

        automationButton.clickable.clicked += ButtonTextUpdate;
        autoProductionMultiplierButton.clickable.clicked += ButtonTextUpdate;
        autoTickSpeedMuiltiplierButton.clickable.clicked += ButtonTextUpdate;
        manualProductionMultiplierButton.clickable.clicked += ButtonTextUpdate;
        recycleButton.clickable.clicked += ButtonTextUpdate;

        ButtonTextUpdate();
        ButtonStatusUpdate();
    }

    private void ButtonTextUpdate()
    {
        automationButton.Q<Label>().text = automationCost.ToString();
        autoProductionMultiplierButton.Q<Label>().text = "(" + autoProductionMultiplier.ToString() + ")\n" + AutoProductionMultiplierCost().ToString();
        autoTickSpeedMuiltiplierButton.Q<Label>().text = "(" + autoTickSpeedMultiplier.ToString() + ")\n" + AutoTickSpeedMultiplierCost().ToString();
        manualProductionMultiplierButton.Q<Label>().text = "(" + manualProductionMultiplier.ToString() + ")\n" + ManualProductionMultiplierCost().ToString();
        recycleButton.Q<Label>().text = recycleMultiplierCost.ToString();
    }

    private void ButtonStatusUpdate()
    {
        manualProductionMultiplierButton.SetEnabled(true);

        if (automationUnlocked)
        {
            automationButton.SetEnabled(true);
            autoProductionMultiplierButton.SetEnabled(true);
            autoTickSpeedMuiltiplierButton.SetEnabled(true);
        }
        else
        {
            automationButton.SetEnabled(false);
            autoProductionMultiplierButton.SetEnabled(false);
            autoTickSpeedMuiltiplierButton.SetEnabled(false);
        }
        
        if(recycleUnlocked)
            recycleButton.SetEnabled(true);
        else
            recycleButton.SetEnabled(false);
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
        if (currencyManager.pixelPoints >= AutoProductionMultiplierCost())
        {
            currencyManager.PurchaseWithPixelPoints(AutoProductionMultiplierCost());
            autoProductionMultiplier += 1;

            Debug.Log("Auto Prod. Multiplier for " + transform.name + " has been purchased! Now production: " + autoTickSpeedMultiplier);
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Automated Prod. Increase.");
        }
    }

    private void ManualProductionMultiplierButton()
    {
        if (currencyManager.pixelPoints >= ManualProductionMultiplierCost())
        {
            currencyManager.PurchaseWithPixelPoints(ManualProductionMultiplierCost());
            manualProductionMultiplier += 1;

            Debug.Log("Manual Prod. Multiplier for " + transform.name + " has been purchased!");
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Manual Prod. Multiplier.");
        }
    }

    private void AutoTickSpeedMultiplierButton()
    {
        if(currencyManager.pixelPoints >= AutoTickSpeedMultiplierCost())
        {
            currencyManager.PurchaseWithPixelPoints(AutoTickSpeedMultiplierCost());
            autoTickSpeedLevel += 1;
            autoTickSpeedMultiplier *= 2;

            Debug.Log("Auto Tick Speed Multiplier for " + transform.name + " has been purchased!");
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Auto Tick Speed Multiplier.");
        }
    }
}
