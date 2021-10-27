using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get => instance; }
    private static UIManager instance;
    public GameManager gameManager;

    // Buttons
    public Button restartButton;

    // Workers
    public WorkerUpgrade wur;
    public WorkerUpgrade wug;
    public WorkerUpgrade wub;

    // Prestige
    private PrestigeManager pm;

    // Other UI Elements
    [SerializeField] public UnityEngine.UIElements.UIDocument uiDocment { get; private set; }
    private VisualElement rootVisualElement;    
    public VisualElement gradientArea { get; private set; }
    public VisualElement[,] activeGradientVisualElements { get; private set; }
    private int prevSize = 0;
    private Label levelProgressionLabel;

    // Foldouts
    private List<Foldout> foldouts = new List<Foldout>();
    private Foldout gameFoldout;
    private Foldout prestigeStoreFoldout;
    private Foldout progressFoldout;
    private Foldout settingsFoldout;


    private void Awake()
    {
        MaintainSingleInstance();

        if (uiDocment == null)
            uiDocment = FindObjectOfType<UIDocument>();

        rootVisualElement = uiDocment.rootVisualElement;
        gradientArea = rootVisualElement.Q<VisualElement>("GradientVisualElement");
        gameManager.currencyManager.pixelFoldout = rootVisualElement.Q<Foldout>("FoldoutGame");
        gameManager.currencyManager.prestigeFoldout = rootVisualElement.Q<Foldout>("FoldoutStore");
        restartButton = rootVisualElement.Q<Button>("RestartButton");
        levelProgressionLabel = rootVisualElement.Q<Label>("LevelProgressLabel");

        // Progress UI Assignment

    }

    private void Start()
    {
        AssignProgressUIComponents();
        BindWorkerUpgradeButtons();
        BindPrestigeButtons();
        BindRestartButton();
        UpdateLevelCompletionText();
        InstantiateFoldouts();
    }

    private void AssignProgressUIComponents()
    {
        gameManager.progressManager.progressFoldout = rootVisualElement.Q<Foldout>("FoldoutProgress");
        gameManager.progressManager.rProgressLabel = rootVisualElement.Q<Label>("RProgressLabel");
        gameManager.progressManager.rProgressBarVisualElement = rootVisualElement.Q<VisualElement>("RWorkerOverallProgressBarVisualElement");

        gameManager.progressManager.gProgressLabel = rootVisualElement.Q<Label>("GProgressLabel");
        gameManager.progressManager.gProgressBarVisualElement = rootVisualElement.Q<VisualElement>("GWorkerOverallProgressBarVisualElement");

        gameManager.progressManager.bProgressLabel = rootVisualElement.Q<Label>("BProgressLabel");
        gameManager.progressManager.bProgressBarVisualElement = rootVisualElement.Q<VisualElement>("BWorkerOverallProgressBarVisualElement");

        gameManager.progressManager.UpdateText();
        gameManager.progressManager.UpdateProgressBars();
    }

    private void InstantiateFoldouts()
    {
        gameFoldout = rootVisualElement.Q<Foldout>("FoldoutGame");
        prestigeStoreFoldout = rootVisualElement.Q<Foldout>("FoldoutStore");
        progressFoldout = rootVisualElement.Q<Foldout>("FoldoutProgress");
        settingsFoldout = rootVisualElement.Q<Foldout>("FoldoutSettings");

        foldouts.Add(gameFoldout);
        foldouts.Add(prestigeStoreFoldout);
        foldouts.Add(progressFoldout);
        foldouts.Add(settingsFoldout);


        gameFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(gameFoldout));
        prestigeStoreFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(prestigeStoreFoldout));
        progressFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(progressFoldout));
        settingsFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(settingsFoldout));
    }

    public void CloseOtherFoldouts(Foldout exceptThisOne)
    {
        foreach(Foldout foldout in foldouts)
        {
            if (foldout == exceptThisOne)
                continue;

            foldout.SetValueWithoutNotify(false);
        }
    }

    public void UpdateLevelCompletionText()
    {
        levelProgressionLabel.text = gameManager.gradientManager.numberCompleted + " / " + (gameManager.size * gameManager.size);
        
        if(gameManager.gradientManager.numberCompleted >= gameManager.size * gameManager.size)
        {
            restartButton.style.display = DisplayStyle.Flex;
            UpdateRestartButtonText();
        }
    }

    public void BindRestartButton()
    {
        restartButton.clickable.clicked += gameManager.RestartGame;
        restartButton.style.display = DisplayStyle.None;
    }

    public void UpdateRestartButtonText()
    {
        restartButton.text = "RESTART\n" +
            "Restart the game, double the gradient size (max size: " + gameManager.maxSize + "x" + gameManager.maxSize + "), " +
            "and received +" + gameManager.prestigePointIncrement + " Prestige Points.";
    }

    public void BindWorkerUpgradeButtons()
    {
        // Bind Upgrade Buttons - Red
        wur = gameManager.workerManager.workers[0].GetComponent<WorkerUpgrade>();
        wur.workerButton = rootVisualElement.Q<Button>("Button_R");
        wur.automationButton = rootVisualElement.Q<Button>("AutomateButton_R");
        wur.productionMultiplierButton = rootVisualElement.Q<Button>("IncreaseProductionButton_R");
        wur.autoTickSpeedMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_R");
        wur.recycleButton = rootVisualElement.Q<Button>("RecycleButton_R");
        wur.ButtonSetup();

        // Bind Upgrade Buttons - Green
        wug = gameManager.workerManager.workers[1].GetComponent<WorkerUpgrade>();
        wug.workerButton = rootVisualElement.Q<Button>("Button_G");
        wug.automationButton = rootVisualElement.Q<Button>("AutomateButton_G");
        wug.productionMultiplierButton = rootVisualElement.Q<Button>("IncreaseProductionButton_G");
        wug.autoTickSpeedMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_G");
        wug.recycleButton = rootVisualElement.Q<Button>("RecycleButton_G");
        wug.ButtonSetup();

        // Bind Upgrade Buttons - Blue
        wub = gameManager.workerManager.workers[2].GetComponent<WorkerUpgrade>();
        wub.workerButton = rootVisualElement.Q<Button>("Button_B");
        wub.automationButton = rootVisualElement.Q<Button>("AutomateButton_B");
        wub.productionMultiplierButton = rootVisualElement.Q<Button>("IncreaseProductionButton_B");
        wub.autoTickSpeedMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_B");
        wub.recycleButton = rootVisualElement.Q<Button>("RecycleButton_B");
        wub.ButtonSetup();
    }

    public void BindPrestigeButtons()
    {
        pm = gameManager.prestigeManager;
        pm.prestigeAutomationButton = rootVisualElement.Q<Button>("pButton_UnlockAutomation");
        pm.prestigeRecycleButton = rootVisualElement.Q<Button>("pButton_UnlockRecycle");
        pm.prestigeCustomStartAndEndButton = rootVisualElement.Q<Button>("pButton_UnlockCustomStartAndEndColors");
        pm.prestigeIncreasePixelPointsButton = rootVisualElement.Q<Button>("pButton_IncreaseStartingPixelPoints");
        pm.ButtonSetup();
    }

    public void NewGradient(int size)
    {
        if (activeGradientVisualElements != null && activeGradientVisualElements.Length != 0)
        {
            for (int i = 0; i < prevSize; i++)
            {
                for (int j = 0; j < prevSize; j++)
                {
                    VisualElement ve = activeGradientVisualElements[i, j];
                    gradientArea.contentContainer.Remove(ve);
                    Destroy(ve.visualTreeAssetSource);
                }
            }
        }

        prevSize = size;
        activeGradientVisualElements = new VisualElement[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Color color = Color.black;
                //if (gameManager.testGradient)
                //    color = gameManager.gradientManager.gradientGColors[i, j].GoalColor();

                VisualElement gradientImage = new VisualElement();
                gradientImage.style.width = new Length(256 / size, LengthUnit.Pixel);
                gradientImage.style.height = new Length(256 / size, LengthUnit.Pixel);
                gradientImage.style.backgroundColor = color;

                /*
                gradientImage.style.borderBottomWidth = 0.5f;
                gradientImage.style.borderLeftWidth = 0.5f;
                gradientImage.style.borderRightWidth = 0.5f;
                gradientImage.style.borderTopWidth = 0.5f;

                gradientImage.style.borderBottomColor = Color.black;
                gradientImage.style.borderLeftColor = Color.black;
                gradientImage.style.borderRightColor = Color.black;
                gradientImage.style.borderTopColor = Color.black;
                */

                activeGradientVisualElements[i, j] = gradientImage;
                gradientArea.contentContainer.Add(gradientImage);
            }
        }
    }

    public void UnlockAutomation()
    {
        wur.UnlockAutomation();
        wug.UnlockAutomation();
        wub.UnlockAutomation();
    }

    public void UpdateVEColor(int i, int j)
    {
        activeGradientVisualElements[i, j].style.backgroundColor = (Color)gameManager.gradientManager.gradientGColors[i, j].CurrentColor();
    }

    public void SetWorkerProgressBars()
    {
        for (int i = 0; i < gameManager.workerManager.workers.Length; i++)
        {
            if (i == 0) // red worker
                gameManager.workerManager.workers[i].progressBarVisualElement = uiDocment.rootVisualElement.Q<VisualElement>("RWorkerProgressBarVisualElement");
            else if (i == 1) // green worker
                gameManager.workerManager.workers[i].progressBarVisualElement = uiDocment.rootVisualElement.Q<VisualElement>("GWorkerProgressBarVisualElement");
            else if (i == 2) // blue worker
                gameManager.workerManager.workers[i].progressBarVisualElement = uiDocment.rootVisualElement.Q<VisualElement>("BWorkerProgressBarVisualElement");
            else
                Debug.LogError("UIManager->SetWorkerProgressBars: Too many workers in workerManager.workers.");

            gameManager.workerManager.workers[i].progressBarVisualElement.style.width = new Length(0, LengthUnit.Percent);
        }
    }

    public void UpdateWorkerUpgradeButtons()
    {
        foreach (Worker worker in gameManager.workerManager.workers)
            worker.workerUpgrade.UpdateButtons();
    }

    private void MaintainSingleInstance()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
}