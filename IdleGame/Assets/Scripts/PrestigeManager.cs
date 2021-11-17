using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PrestigeManager : MonoBehaviour
{
    public static PrestigeManager Instance { get => instance; }
    private static PrestigeManager instance;
    public GameManager gameManager;

    // Buttons
    public Button prestigeAutomationButton;
    public Button prestigeRecycleButton;
    public Button prestigeCustomStartAndEndButton;
    public Button prestigeIncreasePixelPointsButton;

    // Costs
    public int prestigeAutomationCost = 0;
    public int prestigeRecycleCost = 0;
    public int prestigeCustomStartAndEndCost = 0;
    public int prestigeIncreasePixelPointsCost = 0;

    private void Awake() => MaintainSingleInstance();

    private void MaintainSingleInstance()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public void PrestigeAutomationButton()
    {
        if (gameManager.currencyManager.prestigePoints >= prestigeAutomationCost)
        {
            gameManager.currencyManager.PurchaseWithPrestigePoints(prestigeAutomationCost);
            prestigeAutomationButton.SetEnabled(false);
            gameManager.automationEnabled = true;

            foreach(Worker worker in gameManager.workerManager.workers)
                worker.workerUpgrade.UnlockAutomation();
        }
    }

    public void PrestigeRecycleButton()
    {
        if (gameManager.currencyManager.prestigePoints >= prestigeRecycleCost)
        {
            gameManager.currencyManager.PurchaseWithPrestigePoints(prestigeRecycleCost);
            prestigeRecycleButton.SetEnabled(false);
            gameManager.recycleEnabled = true;

            foreach (Worker worker in gameManager.workerManager.workers)
                worker.workerUpgrade.UnlockRecycle();
        }
    }

    public void PrestigeCustomeStartAndEndButton()
    {
        if(gameManager.currencyManager.prestigePoints >= prestigeCustomStartAndEndCost)
        {
            gameManager.currencyManager.PurchaseWithPrestigePoints(prestigeCustomStartAndEndCost);
            prestigeCustomStartAndEndButton.SetEnabled(false);
            gameManager.customColorEnabled = true;
        }
    }

    public void PrestigeIncreasePixelPointsButton()
    {
        if (gameManager.currencyManager.prestigePoints >= prestigeIncreasePixelPointsCost)
        {
            gameManager.currencyManager.PurchaseWithPrestigePoints(prestigeIncreasePixelPointsCost);
            gameManager.startingPixelPoints += 1;
        }
    }
}
