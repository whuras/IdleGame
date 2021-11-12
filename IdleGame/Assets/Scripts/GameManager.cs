using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get => instance; }
    private static GameManager instance;
    public UIManager uiManager;
    public GradientManager gradientManager;
    public CurrencyManager currencyManager;
    public WorkerManager workerManager;
    public PrestigeManager prestigeManager;
    public ProgressManager progressManager;

    public bool automationEnabled = false;
    public bool recycleEnabled = false;

    [Header("Testing Purposes")]
    public int startingPixelPoints = 0;
    public int startingPrestigePoints = 0;
    public bool randomGradient = false;
    public int size = 2;
    public int maxSize = 64;
    public int prestigePointIncrement = 1;
    public Color32 startColor = Color.black;
    public Color32 endColor = Color.white;

    private void Awake() => MaintainSingleInstance();

    private void Start()
    {
        GameSetup();
    }

    public void RandomColours()
    {
        startColor = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 1);
        endColor = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), 1);
    }

    private void GameSetup()
    {
        if (randomGradient)
            RandomColours();

        if (size == 0)
            size = 2;
        else if (size > maxSize)
            size = maxSize;

        gradientManager.InitializeGradientGColors(size,
            new Tuple<int, int, int>(
                startColor.r,
                startColor.g,
                startColor.b),
            new Tuple<int, int, int>(
                endColor.r,
                endColor.g,
                endColor.b));

        uiManager.NewGradient(size);
        uiManager.SetWorkerProgressBars();
    }

    public void RestartGame()
    {
        //Debug.Log("Note: GameManager->RestartGame currently set to not reset pixel points on game reset.");
        currencyManager.ResetPixelPoints(); // if not allow pixel points carry over
        currencyManager.IncrementPrestigePoints(prestigePointIncrement);
        //prestigePointIncrement = size;
        size *= 2;
        
        foreach (Worker w in workerManager.workers)
        {
            w.keepWorking = true;
            w.workerUpgrade.ResetMultipliers();
        }

        GameSetup();

        // Currency Setup
        //currencyManager.prestigePoints = startingPrestigePoints;
        currencyManager.pixelPoints = startingPixelPoints;
        currencyManager.UpdateText();

        uiManager.EnableRestartBG(false);
        uiManager.restartButton.style.display = DisplayStyle.None;
        uiManager.UpdateLevelCompletionText();
        uiManager.UpdateWorkerUpgradeButtons();
    }

    private void MaintainSingleInstance()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
}