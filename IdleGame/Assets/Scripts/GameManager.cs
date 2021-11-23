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
    public EffectManager effectManager;

    public bool automationEnabled = false;
    public bool recycleEnabled = false;
    public bool customColorEnabled = false;

    public int startingPixelPoints = 0;
    public int startingPrestigePoints = 0;
    public int size = 2;
    public int maxSize = 64;
    public int prestigePointIncrement = 1;

    public Color32 startColor = Color.white;
    public Color32 endColor = Color.black;

    public int customLockedCounter = 1; // if custom colors are not unlocked, this counter works to give variety

    private void Awake() => MaintainSingleInstance();

    private void Start()
    {
        SaveSystem.Init();
        progressManager.CreateLevelGoals();
        GameSetup();
        uiManager.InitialUISetup();
        gradientManager.SortQueue(Worker.Type.Red);
        gradientManager.SortQueue(Worker.Type.Green);
        gradientManager.SortQueue(Worker.Type.Blue);
    }

    public void UpdateFromLoad()
    {
        foreach (Worker worker in workerManager.workers)
        {
            if (automationEnabled)
                worker.workerUpgrade.UnlockAutomation();

            if (recycleEnabled)
                worker.workerUpgrade.UnlockRecycle();
        }

        uiManager.EnableRestartVisualElement(true);
        uiManager.UpdateRestartButtonText();
        
        progressManager.CheckCurrentLevel();
        uiManager.UpdateProgressUI();
        progressManager.UpdateAllBlocks();
    }

    private void GameSetup()
    {
        size = SizeBasedOnLevel();

        if (customColorEnabled)
        {
            gradientManager.InitializeGradientGColors(size,
            new Tuple<int, int, int>(
                uiManager.startColor.r,
                uiManager.startColor.g,
                uiManager.startColor.b),
            new Tuple<int, int, int>(
                uiManager.endColor.r,
                uiManager.endColor.g,
                uiManager.endColor.b));
        }
        else
        {
            ColorBasedOnCustomLocked();

            gradientManager.InitializeGradientGColors(size,
            new Tuple<int, int, int>(
                startColor.r,
                startColor.g,
                startColor.b),
            new Tuple<int, int, int>(
                endColor.r,
                endColor.g,
                endColor.b));
        }

        uiManager.NewGradient(size);
        uiManager.SetWorkerProgressBars();
    }

    private void ColorBasedOnCustomLocked()
    {
        switch (customLockedCounter)
        {
            case 1:
                startColor = new Color32(0, 0, 0, 255);
                endColor = new Color32(255, 255, 255, 255);
                break;
            case 2:
                startColor = new Color32(255, 255, 255, 255);
                endColor = new Color32(255, 0, 0, 255);
                break;
            case 3:
                startColor = new Color32(255, 255, 255, 255);
                endColor = new Color32(0, 255, 0, 255);
                break;
            case 4:
                startColor = new Color32(255, 255, 255, 255);
                endColor = new Color32(0, 0, 255, 255);
                break;
            case 5:
                startColor = new Color32(255, 255, 255, 255);
                endColor = new Color32(0, 0, 255, 255);
                customLockedCounter = 0;
                break;
        }
    }

    public int SizeBasedOnLevel()
    {
        if (progressManager.currentLevel <= 2)
            return 2;
        else if (progressManager.currentLevel <= 3)
            return 4;
        else if (progressManager.currentLevel <= 4)
            return 8;
        else if (progressManager.currentLevel <= 5)
            return 16;
        else if (progressManager.currentLevel <= 6)
            return 32;
        return 64;
    }

    public void RestartGame()
    {
        currencyManager.ResetPixelPoints(); // if not allow pixel points carry over
        
        foreach (Worker w in workerManager.workers)
        {
            w.keepWorking = true;
            w.workerUpgrade.ResetMultipliers();
        }

        GameSetup();
        gradientManager.SortQueue(Worker.Type.Red);
        gradientManager.SortQueue(Worker.Type.Green);
        gradientManager.SortQueue(Worker.Type.Blue);

        // Currency Setup
        currencyManager.pixelPoints = startingPixelPoints;
        currencyManager.UpdateText();

        uiManager.EnableRestartVisualElement(false);
        uiManager.UpdateLevelCompletionText();
        gradientManager.CheckGradientStatus();
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