using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/IdleGradientSaves";

    private static string saveFileName = "/IdleGradientSaveData.txt";

    public static void Init()
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Debug.Log("Save directory has been created.");
            Directory.CreateDirectory(SAVE_FOLDER);
        }
        else
        {
            Debug.Log("Save directory exists.");
        }
            
    }

    public static void Save()
    {
        SaveData saveDate = new SaveData
        {
            currentLevel = GameManager.Instance.progressManager.currentLevel,
            automationEnabled = GameManager.Instance.automationEnabled,
            recycleEnabled = GameManager.Instance.recycleEnabled,
            customColorEnabled = GameManager.Instance.customColorEnabled,
            customLockedCounter = GameManager.Instance.customLockedCounter,
            prestigePoints = GameManager.Instance.currencyManager.prestigePoints,
            startingPixelPoints = GameManager.Instance.startingPixelPoints,
            goals = GameManager.Instance.progressManager.goals,
        };

        string jsonData = JsonUtility.ToJson(saveDate);
        File.WriteAllText(SAVE_FOLDER + saveFileName, jsonData);
        Debug.LogError("Data has been saved.");
    }

    public static void Load()
    {
        if(File.Exists(SAVE_FOLDER + saveFileName))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + saveFileName);
            SaveData loadedSaveData = JsonUtility.FromJson<SaveData>(saveString);

            GameManager.Instance.progressManager.currentLevel = loadedSaveData.currentLevel;
            GameManager.Instance.automationEnabled = loadedSaveData.automationEnabled;
            GameManager.Instance.recycleEnabled = loadedSaveData.recycleEnabled;
            GameManager.Instance.customColorEnabled = loadedSaveData.customColorEnabled;
            GameManager.Instance.customLockedCounter = loadedSaveData.customLockedCounter;
            GameManager.Instance.currencyManager.prestigePoints = loadedSaveData.prestigePoints;
            GameManager.Instance.startingPixelPoints = loadedSaveData.startingPixelPoints;
            GameManager.Instance.progressManager.goals = loadedSaveData.goals;

            Debug.LogError("Data has been loaded.");
            GameManager.Instance.UpdateFromLoad();
        }
        else
        {
            Debug.LogError("Save does not exist and cannot be loaded.");
        }
    }

    public static void DeleteSaveFile()
    {
        File.Delete(SAVE_FOLDER + saveFileName);

        Debug.LogError("Saved data has been deleted.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
