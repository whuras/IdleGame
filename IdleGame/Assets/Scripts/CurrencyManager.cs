using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get => instance; }
    private static CurrencyManager instance;
    public GameManager gameManager;

    public Foldout pixelFoldout;
    public Foldout prestigeFoldout;

    public bool pixelPointMultipliedByLevelUnlocked = false;
    public int pixelPoints;
    public float pixelPointsMultiplier = 1f;
    public int prestigePoints;
    public float prestigePointsMultiplier = 1f;

    private void Awake() => MaintainSingleInstance();

    private void Start()
    {
        UpdatePixelPointText();
        UpdatePrestigePointText();
    }

    public void UpdateText()
    {
        UpdatePixelPointText();
        UpdatePrestigePointText();
        gameManager.uiManager.UpdatePrestigeButtons();
        gameManager.uiManager.UpdateWorkerUpgradeButtons();
    }

    private void UpdatePixelPointText() => pixelFoldout.text = "IDLE GRADIENT [ " + pixelPoints + " Pixel Points ]";
    private void UpdatePrestigePointText() => prestigeFoldout.text = "PRESTIGE STORE [ " + prestigePoints + " Prestige Points ]";

    public void IncrementPixelPoints(int amount = 1)
    {
        if (pixelPointMultipliedByLevelUnlocked)
            pixelPointsMultiplier = gameManager.size;
        else
            pixelPointsMultiplier = 1;

        pixelPoints += (int)(amount * pixelPointsMultiplier);
        UpdateText();
    }

    public void IncrementPrestigePoints(int amount = 1)
    {
        prestigePoints += (int)(amount * prestigePointsMultiplier);
        UpdateText();
    }

    public void ResetPixelPoints()
    {
        pixelPoints = 0;
        UpdateText();
    }

    public bool PurchaseWithPixelPoints(int cost)
    {
        if (pixelPoints - cost >= 0)
        {
            pixelPoints -= cost;
            UpdateText();
            return true;
        }

        return false;
    }

    public bool PurchaseWithPrestigePoints(int cost)
    {
        if (prestigePoints - cost >= 0)
        {
            prestigePoints -= cost;
            UpdateText();
            return true;
        }

        return false;
    }

    private void MaintainSingleInstance()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
}
