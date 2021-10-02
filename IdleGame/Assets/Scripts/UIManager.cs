using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get => instance; }
    private static UIManager instance;
    public GameManager gameManager;

    [SerializeField] public UIDocument uiDocment { get; private set; }
    private VisualElement rootVisualElement;
    private Button restartButton;
    public VisualElement gradientArea { get; private set; }
    public VisualElement[,] activeGradientVisualElements { get; private set; }
    private int[] ValidSizes = new int[] { 2, 4, 8, 16, 32, 64, 128 }; // 256 takes too long to load
    private int prevSize = 0;
    private Label levelProgressionLabel;

    public WorkerUpgrade wur;
    public WorkerUpgrade wug;
    public WorkerUpgrade wub;

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
    }

    private void Start()
    {
        BindWorkerUpgradeButtons();
        BindRestartButton();
        UpdateLabelText();
    }

    public void UpdateLabelText()
    {
        levelProgressionLabel.text = gameManager.gradientManager.numberCompleted + " / " + (gameManager.size * gameManager.size);
    }

    public void CheckButtonStatus()
    {
        for (int i = 0; i < gameManager.size; i++)
        {
            for (int j = 0; j < gameManager.size; j++)
            {

            }
        }
    }

    public void BindRestartButton()
    {
        restartButton.clickable.clicked += gameManager.RestartGame;
    }

    public void BindWorkerUpgradeButtons()
    {
        // Bind Upgrade Buttons - Red
        wur = gameManager.workerManager.workers[0].GetComponent<WorkerUpgrade>();
        wur.workerButton = rootVisualElement.Q<Button>("Button_R");
        wur.automationButton = rootVisualElement.Q<Button>("AutomateButton_R");
        wur.autoProductionMultiplierButton = rootVisualElement.Q<Button>("IncreaseAutomatedProductionButton_R");
        wur.autoTickMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_R");
        wur.recycleButton = rootVisualElement.Q<Button>("RecycleButton_R");
        wur.manualProductionMultiplierButton = rootVisualElement.Q<Button>("IncreaseManualProductionButton_R");
        wur.ButtonSetup();

        // Bind Upgrade Buttons - Green
        wug = gameManager.workerManager.workers[1].GetComponent<WorkerUpgrade>();
        wug.workerButton = rootVisualElement.Q<Button>("Button_G");
        wug.automationButton = rootVisualElement.Q<Button>("AutomateButton_G");
        wug.autoProductionMultiplierButton = rootVisualElement.Q<Button>("IncreaseAutomatedProductionButton_G");
        wug.autoTickMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_G");
        wug.recycleButton = rootVisualElement.Q<Button>("RecycleButton_G");
        wug.manualProductionMultiplierButton = rootVisualElement.Q<Button>("IncreaseManualProductionButton_G");
        wug.ButtonSetup();

        // Bind Upgrade Buttons - Blue
        wub = gameManager.workerManager.workers[2].GetComponent<WorkerUpgrade>();
        wub.workerButton = rootVisualElement.Q<Button>("Button_B");
        wub.automationButton = rootVisualElement.Q<Button>("AutomateButton_B");
        wub.autoProductionMultiplierButton = rootVisualElement.Q<Button>("IncreaseAutomatedProductionButton_B");
        wub.autoTickMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_B");
        wub.recycleButton = rootVisualElement.Q<Button>("RecycleButton_B");
        wub.manualProductionMultiplierButton = rootVisualElement.Q<Button>("IncreaseManualProductionButton_B");
        wub.ButtonSetup();
    }

    public void NewGradient(int size)
    {
        if (IsSizeValid(size))
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
    }

    public void UpdateVEColor(int i, int j)
    {
        activeGradientVisualElements[i, j].style.backgroundColor = (Color)gameManager.gradientManager.gradientGColors[i, j].CurrentColor();
    }

    public bool IsSizeValid(int size)
    {
        if (!Array.Exists(ValidSizes, x => x == size))
        {
            Debug.LogError("UIManager->PopulateGradient: Size is not valid (" + size + ").");
            return false;
        }

        return true;
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

    private void MaintainSingleInstance()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }
}