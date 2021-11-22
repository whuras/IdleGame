using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int saveExists;
    public int SaveExists
    {
        get => saveExists;
        private set
        {
            saveExists = value;
        }
    }

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
    public bool customColorEnabled = false;

    public int startingPixelPoints = 0;
    public int startingPrestigePoints = 0;
    public int size = 2;
    public int maxSize = 64;
    public int prestigePointIncrement = 1;

    public Color32 startColor = Color.black;
    public Color32 endColor = Color.white;

    private int customLockedCounter = 0; // if custom colors are not unlocked, this counter works to give variety

    private void Awake() => MaintainSingleInstance();

    private void Start()
    {
        GameSetup();
    }

    public void SaveGame()
    {
        saveExists = 1;

        PlayerPrefs.SetInt("saveExists", 1);
        PlayerPrefs.SetInt("currentLevel", progressManager.currentLevel);
        PlayerPrefs.SetInt("automationEnabled", automationEnabled ? 1 : 0);
        PlayerPrefs.SetInt("recycleEnabled", recycleEnabled ? 1 : 0);
        PlayerPrefs.SetInt("customColorEnabled", customColorEnabled ? 1 : 0);
        PlayerPrefs.SetInt("prestigePoints", currencyManager.prestigePoints - 1);

        PlayerPrefs.Save();
        Debug.LogWarning("Game Saved!");
    }

    public void LoadSave()
    {
        if (PlayerPrefs.HasKey("saveExists"))
        {
            progressManager.currentLevel = PlayerPrefs.GetInt("currentLevel");
            automationEnabled = PlayerPrefs.GetInt("automationEnabled") == 1 ? true : false;
            recycleEnabled = PlayerPrefs.GetInt("recycleEnabled") == 1 ? true : false;
            customColorEnabled = PlayerPrefs.GetInt("customColorEnabled") == 1 ? true : false;
            currencyManager.prestigePoints = PlayerPrefs.GetInt("prestigePoints");

            UpdateFromLoad();
            Debug.LogError("Game loaded from save file.");
        }
        else
        {
            Debug.LogError("Save does not exist!");
        }
    }

    private void UpdateFromLoad()
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
    }

    public void ResetSaveFile()
    {
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("Save file has been deleted!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GameSetup()
    {
        size = (int) Mathf.Pow(2, progressManager.currentLevel);

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
            customLockedCounter += 1;
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
                startColor = Color.white;
                endColor = Color.black;
                break;
            case 2:
                startColor = Color.white;
                endColor = Color.red;
                break;
            case 3:
                startColor = Color.white;
                endColor = Color.green;
                break;
            case 4:
                startColor = Color.white;
                endColor = Color.blue;
                customLockedCounter = 0;
                break;
        }
    }

    public void RestartGame()
    {
        currencyManager.ResetPixelPoints(); // if not allow pixel points carry over
        currencyManager.IncrementPrestigePoints(prestigePointIncrement);
        size *= 2;
        
        foreach (Worker w in workerManager.workers)
        {
            w.keepWorking = true;
            w.workerUpgrade.ResetMultipliers();
        }

        GameSetup();

        // Currency Setup
        currencyManager.pixelPoints = startingPixelPoints;
        currencyManager.UpdateText();

        uiManager.EnableRestartVisualElement(false);
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