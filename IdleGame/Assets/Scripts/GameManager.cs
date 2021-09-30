using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get => instance; }
    private static GameManager instance;
    public UIManager uiManager;
    public GradientManager gradientManager;
    public CurrencyManager currencyManager;
    public WorkerManager workerManager;

    [Header("Testing Purposes")]
    public bool randomGradient = false;
    public int size = 2;
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

        gradientManager.InitializeGradientGColors(size,
            new Tuple<int, int, int>(
                startColor.r,
                startColor.g,
                startColor.b),
            new Tuple<int, int, int>(
                endColor.r,
                endColor.g,
                endColor.b));

        if (size == 0)
            size = 2;
        else if (size > 128)
            size = 128;

        uiManager.NewGradient(size);

        uiManager.SetWorkerProgressBars();
    }

    public void RestartGame()
    {
        Debug.Log("Restart Game pressed");

        currencyManager.ResetPixelPoints();
        currencyManager.IncrementPrestigePoints(size);
        size *= 2;

        foreach (Worker w in workerManager.workers)
            w.keepWorking = true;

        GameSetup();
    }

    private void MaintainSingleInstance()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
}