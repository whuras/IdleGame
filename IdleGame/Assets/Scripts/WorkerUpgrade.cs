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
    public int productionLevel = 1;
    public float ProductionMultiplier() => Mathf.Min(10, productionLevel); // % of the bar to fill when manually clicking (multiplied in Worker by starting amount (10))
    public int ProductionMultiplierCost() => (int)Mathf.Pow(2, productionLevel - 1);

    [Header("Automation Unlock")]
    public Button automationButton; // enabled automation
    public int automationCost = 10;
    
    [Header("Automation Upgrades")]
    public Button autoTickSpeedMuiltiplierButton; // speed of tick 
    public int autoTickSpeedLevel = 1;
    public float AutoTickSpeedMultiplier() => Mathf.Max(0.1f, 1 - autoTickSpeedLevel * 0.1f);
    public int AutoTickSpeedMultiplierCost() => (int) Mathf.Pow(2, autoTickSpeedLevel - 1);

    [Header("Recycle Upgrades")]
    public bool recycleUnlocked = false;
    public Button recycleButton;
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
        buttonStatuses[automationButton] = UpgradeStatus.Purchased;
        AutomationButton();
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
            gameManager.automationEnabled = true;
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
            productionLevel += 1;
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
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Auto Tick Speed Multiplier.");
        }

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void ResetMultipliers()
    {
        productionLevel = 1;
        autoTickSpeedLevel = 1;

        recycleMultiplier = 1;

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }
}
