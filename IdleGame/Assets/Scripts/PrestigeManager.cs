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

    public void ButtonSetup()
    {
        prestigeAutomationButton.clickable.clicked += PrestigeAutomationButton;
        prestigeRecycleButton.clickable.clicked += PrestigeRecycleButton;
        prestigeCustomStartAndEndButton.clickable.clicked += PrestigeCustomeStartAndEndButton;
        prestigeIncreasePixelPointsButton.clickable.clicked += PrestigeIncreasePixelPointsButton;

        ButtonTextUpdate();
    }

    public void ButtonTextUpdate()
    {
        prestigeAutomationButton.text = "Unlock Automation\nCost: " + prestigeAutomationCost.ToString();
        prestigeRecycleButton.text = "Unlock Recycle\nCost: " + prestigeRecycleCost.ToString();
        prestigeCustomStartAndEndButton.text = "Unlock Custom Start\nand End Colors\nCost: " + prestigeCustomStartAndEndCost.ToString();
        prestigeIncreasePixelPointsButton.text = "Increase Starting Pixel\nPoints +1\n(Current: " + gameManager.startingPixelPoints + ")\nCost: " + prestigeCustomStartAndEndCost.ToString();
    }

    public void PrestigeAutomationButton()
    {
        if (gameManager.currencyManager.prestigePoints >= prestigeAutomationCost)
        {
            gameManager.currencyManager.PurchaseWithPrestigePoints(prestigeAutomationCost);
            prestigeAutomationButton.SetEnabled(false);
            gameManager.uiManager.UnlockAutomation();
            Debug.Log("Automation has been unlocked!");
        }
        else
        {
            Debug.Log("Not enough Pixel Points to buy Automation.");
        }
    }

    public void PrestigeRecycleButton()
    {

    }

    public void PrestigeCustomeStartAndEndButton()
    {

    }

    public void PrestigeIncreasePixelPointsButton()
    {
        if (gameManager.currencyManager.prestigePoints >= prestigeIncreasePixelPointsCost)
        {
            gameManager.currencyManager.PurchaseWithPrestigePoints(prestigeIncreasePixelPointsCost);
            gameManager.startingPixelPoints += 1;
            ButtonTextUpdate();
        }
    }
}
