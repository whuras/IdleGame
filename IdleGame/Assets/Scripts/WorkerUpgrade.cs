using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WorkerUpgrade : MonoBehaviour
{
    [Header("Managers")]
    public GameManager gameManager;
    public Worker myWorker;
    public Button workerButton;

    [Header("General Upgrades")]
    public Button productionMultiplierButton;
    public float productionMultiplier = 1;
    public int ProductionMultiplierCost() => (int)Mathf.Pow(2, productionMultiplier - 1);

    [Header("Automation Unlock")]
    public Button automationButton; // enabled automation
    public bool autoEnabled = false;
    public int automationCost = 10;
    
    [Header("Automation Upgrades")]
    public Button autoTickSpeedMuiltiplierButton; // speed of tick 
    public int autoTickSpeedLevel = 1;
    public float autoTickSpeedMultiplier = 1;
    public int AutoTickSpeedMultiplierCost() => (int)Mathf.Pow(2, autoTickSpeedLevel - 1);

    [Header("Recycle Upgrades")]
    public bool recycleUnlocked = false;
    public Button recycleButton;
    public bool recycleEnabled = false;
    public float recycleMultiplier = 1;
    public int recycleMultiplierCost = 10;

    // Other
    public Dictionary<Button, UpgradeStatus> buttonStatuses = new Dictionary<Button, UpgradeStatus>();
    public enum UpgradeStatus
    {
        Locked, // needs to be unlocked via Prestige Store
        Unlocked, // unlocked but not yet purchased
        Purchased // implies unlocked
    }

    private void Awake()
    {
        if (myWorker == null)
            myWorker = GetComponent<Worker>();
    }

    public void UnlockAutomation()
    {
        buttonStatuses[automationButton] = UpgradeStatus.Unlocked;
        buttonStatuses[autoTickSpeedMuiltiplierButton] = UpgradeStatus.Unlocked;
        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void AutomationButton()
    {
        if (buttonStatuses[automationButton] == UpgradeStatus.Purchased)
            return;

        if (gameManager.currencyManager.pixelPoints >= automationCost)
        {
            buttonStatuses[automationButton] = UpgradeStatus.Purchased;
            autoEnabled = true;
            gameManager.currencyManager.PurchaseWithPixelPoints(automationCost);
            automationButton.SetEnabled(false);
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Automation.");
        }

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void RecycleButton()
    {
        Debug.Log("Recycle for " + transform.name + " clicked.");
        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void ProductionMultiplierButton()
    {
        if (gameManager.currencyManager.pixelPoints >= ProductionMultiplierCost())
        {
            gameManager.currencyManager.PurchaseWithPixelPoints(ProductionMultiplierCost());
            productionMultiplier += 1;
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Automated Prod. Increase.");
        }

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void AutoTickSpeedMultiplierButton()
    {
        if(gameManager.currencyManager.pixelPoints >= AutoTickSpeedMultiplierCost())
        {
            gameManager.currencyManager.PurchaseWithPixelPoints(AutoTickSpeedMultiplierCost());
            autoTickSpeedLevel += 1;
            autoTickSpeedMultiplier *= 2;
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Auto Tick Speed Multiplier.");
        }

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void ResetMultipliers()
    {
        productionMultiplier = 1;

        autoTickSpeedLevel = 1;
        autoTickSpeedMultiplier = 1;

        recycleMultiplier = 1;

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }
}
