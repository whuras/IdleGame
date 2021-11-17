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

    public void SaveGame()
    {
        saveExists = 1;

        PlayerPrefs.SetInt("saveExists", 1);
        PlayerPrefs.SetInt("automationEnabled", automationEnabled ? 1 : 0);
        PlayerPrefs.SetInt("recycleEnabled", recycleEnabled ? 1 : 0);
        PlayerPrefs.SetInt("customColorEnabled", customColorEnabled ? 1 : 0);
        PlayerPrefs.SetInt("prestigePoints", currencyManager.prestigePoints);
        PlayerPrefs.SetInt("gradientSize", Mathf.Max(1, size / 2));

        PlayerPrefs.Save();
        Debug.LogWarning("Game Saved!");
    }

    public void LoadSave()
    {
        if (PlayerPrefs.HasKey("saveExists"))
        {
            automationEnabled = PlayerPrefs.GetInt("automationEnabled") == 1 ? true : false;
            recycleEnabled = PlayerPrefs.GetInt("recycleEnabled") == 1 ? true : false;
            customColorEnabled = PlayerPrefs.GetInt("customColorEnabled") == 1 ? true : false;
            currencyManager.prestigePoints = PlayerPrefs.GetInt("prestigePoints");
            size = PlayerPrefs.GetInt("gradientSize");

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