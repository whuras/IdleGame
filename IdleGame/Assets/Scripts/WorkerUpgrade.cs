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
    public VisualElement automationEnabledIcon;
    public VisualElement automationDisabledIcon;
    public int automationCost = 10;
    
    [Header("Automation Upgrades")]
    public Button autoTickSpeedMuiltiplierButton; // speed of tick 
    public int autoTickSpeedLevel = 1;
    public float AutoTickSpeedMultiplier() => Mathf.Max(0.1f, 1 - autoTickSpeedLevel * 0.1f);
    public int AutoTickSpeedMultiplierCost() => (int) Mathf.Pow(2, autoTickSpeedLevel - 1);

    [Header("Recycle Upgrades")]
    public Button recycleButton;
    public int recycleLevel = 1;
    public int RecycleMultiplier() => recycleLevel; 
    public int RecycleMultiplierCost() => (int)Mathf.Pow(2, recycleLevel - 1);

    // Other
    public Dictionary<Button, UpgradeStatus> buttonStatuses = new Dictionary<Button, UpgradeStatus>();
    public enum UpgradeStatus
    {
        Locked, // needs to be unlocked via Prestige Store
        Unlocked, // unlocked but not yet purchased
        Purchased, // implies unlocked
        Enabled,
        Disabled
    }

    private void Awake()
    {
        if (myWorker == null)
            myWorker = GetComponent<Worker>();
    }

    public void UnlockAutomation()
    {
        buttonStatuses[automationButton] = UpgradeStatus.Purchased;
        buttonStatuses[autoTickSpeedMuiltiplierButton] = UpgradeStatus.Unlocked;
        automationEnabledIcon.style.display = DisplayStyle.None;
        automationDisabledIcon.style.display = DisplayStyle.Flex;
        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void AutomationButton()
    {
        if(buttonStatuses[automationButton] == UpgradeStatus.Enabled)
        {
            buttonStatuses[automationButton] = UpgradeStatus.Disabled;
            automationEnabledIcon.style.display = DisplayStyle.None;
            automationDisabledIcon.style.display = DisplayStyle.Flex;
        }
        else if (buttonStatuses[automationButton] == UpgradeStatus.Disabled)
        {
            buttonStatuses[automationButton] = UpgradeStatus.Enabled;
            automationEnabledIcon.style.display = DisplayStyle.Flex;
            automationDisabledIcon.style.display = DisplayStyle.None;
        }
        else if (buttonStatuses[automationButton] == UpgradeStatus.Purchased)
        {
            buttonStatuses[automationButton] = UpgradeStatus.Enabled;
            automationEnabledIcon.style.display = DisplayStyle.Flex;
            automationDisabledIcon.style.display = DisplayStyle.None;
        }

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void UnlockRecycle()
    {
        buttonStatuses[recycleButton] = UpgradeStatus.Purchased;
        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void RecycleButton()
    {
        if(buttonStatuses[recycleButton] == UpgradeStatus.Purchased && gameManager.currencyManager.pixelPoints >= RecycleMultiplierCost())
        {
            gameManager.currencyManager.PurchaseWithPixelPoints(RecycleMultiplierCost());
            recycleLevel += 1;
        }

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    public void ProductionMultiplierButton()
    {
        if (gameManager.currencyManager.pixelPoints >= ProductionMultiplierCost())
        {
            gameManager.currencyManager.PurchaseWithPixelPoints(ProductionMultiplierCost());
            productionLevel += 1;
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
        recycleLevel = 1;

        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }
}
