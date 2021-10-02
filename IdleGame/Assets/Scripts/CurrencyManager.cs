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

    private void UpdatePixelPointText() => pixelFoldout.text = "IDLE GRADIENT [ " + pixelPoints + " Pixel Points ]";
    private void UpdatePrestigePointText() => prestigeFoldout.text = "PRESTIGE STORE [ " + prestigePoints + " Prestige Points ]";

    public void IncrementPixelPoints(int amount = 1)
    {
        if (pixelPointMultipliedByLevelUnlocked)
            pixelPointsMultiplier = gameManager.size;
        else
            pixelPointsMultiplier = 1;

        pixelPoints += (int)(amount * pixelPointsMultiplier);
        UpdatePixelPointText();
    }

    public void IncrementPrestigePoints(int amount = 1)
    {
        prestigePoints += (int)(amount * prestigePointsMultiplier);
        UpdatePrestigePointText();
    }

    public void ResetPixelPoints()
    {
        pixelPoints = 0;
        UpdatePixelPointText();
    }

    public bool PurchaseWithPixelPoints(int cost)
    {
        if (pixelPoints - cost >= 0)
        {
            pixelPoints -= cost;
            UpdatePixelPointText();
            return true;
        }

        return false;
    }

    public bool PurchaseWithPrestigePoints(int cost)
    {
        if (prestigePoints - cost >= 0)
        {
            prestigePoints -= cost;
            UpdatePrestigePointText();
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
