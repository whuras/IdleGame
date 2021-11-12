using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get => instance; }
    private static UIManager instance;
    public GameManager gameManager;

    [Header("Tooltips")]
    public Color toolTipBGColor = Color.black;
    public Color toolTipTextColor = Color.white;

    [Header("Worker Upgrade References")]
    public WorkerUpgrade wur;
    public WorkerUpgrade wug;
    public WorkerUpgrade wub;

    [Header("Restart Button")]
    public Button restartButton;
    public VisualElement restartButtonBG;

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

    // Prestige Button Tooltip Texts
    private string[,] prestigeButtonTooltipTexts = new string[4, 4]
    {
        { // 0 Automation Texts
            "prestigeAutomationTooltip",  // 0 Tooltip Name
            "Buy to enable Automation", // 1 Can Afford
            "Buy to enable Automation\nYou need more Prestige Points to buy this", // 2 Can NOT Afford
            "Automation Purchased", // 3 Purchased
        },
        { // 1 Recycle Texts
            "prestigeRecycleTooltip",  // 0 Tooltip Name
            "Buy to enable Recycle", // 1 Can Afford
            "Buy to enable Recycle\nYou need more Prestige Points to buy this", // 2 Can NOT Afford
            "Recycle Purchased", // 3 Purchased
        },
        { // 2 Custom Start and End Texts
            "prestigeCustomStartAndEndTooltip",  // 0 Tooltip Name
            "Buy to enable custom start and end colours on game restart", // 1 Can Afford
            "Buy to enable custom start and end colours on game restart\nYou need more Prestige Points to buy this", // 2 Can NOT Afford
            "Custom Colors Purchased", // 3 Purchased
        },
        { // 3 Increase Pixel Points Texts
            "prestigeIncreasePixelPointsTooltip",  // 0 Tooltip Name
            "Buy to increase the amount of Pixel Points you restart with", // 1 Can Afford
            "Buy to increase the amount of Pixel Points you restart with\nYou need more Prestige Points to buy this", // 2 Can NOT Afford
            "PURCAHSED IS AN INVALID STATE FOR INCREASE PIXEL POINTS", // 3 Purchased
        }
    };

    // Worker Upgarde Button Tooltip Texts
    private string[,] workerUpgradeButtonTooltipTexts = new string[4, 6]
    {
        {   // 0 Production Texts
            "productionMultiplierTooltip", // 0 Tooltip Name
            "LOCKED IS INVALID STATUS FOR PROD MULTI", // 1 Locked
            "UNLOCKED IS INVALID STATUS FOR PROD MULTI", // 2 Unlocked CAN afford
            "UNLOCKED IS INVALID STATUS FOR PROD MULTI", // 3 Unlocked CAN NOT afford
            "Increase Production\n(The tick amount for manual clicking)", // 4 Purchased CAN afford
            "Increase Production\nYou need more Pixel Points to buy this", // 5 Purchased CAN NOT afford
        },
        {   // 1 Automation Texts
            "automationTooltip", // 0 Tooltip Name
            "Unlock Automation\nin the Prestige Store", // 1 Locked
            "Enable Automation", // 2 Unlocked CAN afford
            "Automation\nYou need more Pixel Points to buy this", // 3 Unlocked CAN NOT afford
            "Automation Enabled", // 4 Purchased CAN afford
            "Automation Enabled", // 5 Purchased CAN NOT afford
        },
        {   // 2 TickSpeed Texts
            "autoTickSpeedTootip", // 0 Tooltip Name
            "Unlock Automation\nin the Prestige Store", // 1 Locked
            "Increase Speed", // 2 Unlocked CAN afford
            "Increase Speed\nYou need more Pixel Points to buy this", // 3 Unlocked CAN NOT afford
            "Increase Speed\n(The speed of automated filling)", // 4 Purchased CAN afford
            "Increase Speed\nYou need more Pixel Points to buy this", // 5 Purchased CAN NOT afford
        },
        {   // 3 Recycle Texts
            "recycleTooltip", // 0 Tooltip Name
            "Unlock Recycle\nin the Prestige Store", // 1 Locked
            "Enable Recycle", // 2 Unlocked CAN afford
            "Enable Recycle\nYou need more Pixel Points to buy this", // 3 Unlocked CAN NOT afford
            "Increase Recycle Amount", // 4 Purchased CAN afford
            "Increase Recycle Amount\nYou need more Pixel Points to buy this", // 5 Purchased CAN NOT afford
        }
    };


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
        restartButtonBG = rootVisualElement.Q<VisualElement>("RestartVisualElement");
        levelProgressionLabel = rootVisualElement.Q<Label>("LevelProgressLabel");

        // Progress UI Assignment

    }

    private void Start()
    {
        AssignProgressUIComponents();

        BindWorkerUpgradeButtons();
        BindPrestigeButtons();
        BindRestartButton();

        SetupToolTips(); // must be after bind but before setup

        SetupWorkerUpgradeButtons();
        SetupPrestigeButtons();

        UpdateLevelCompletionText();
        InstantiateFoldouts();
    }

    private void AssignProgressUIComponents()
    {
        ProgressManager pm = gameManager.progressManager;
        pm.progressFoldout = rootVisualElement.Q<Foldout>("FoldoutProgress");
        pm.rProgressLabel = rootVisualElement.Q<Label>("RProgressLabel");
        pm.rProgressBarVisualElement = rootVisualElement.Q<VisualElement>("RWorkerOverallProgressBarVisualElement");

        pm.gProgressLabel = rootVisualElement.Q<Label>("GProgressLabel");
        pm.gProgressBarVisualElement = rootVisualElement.Q<VisualElement>("GWorkerOverallProgressBarVisualElement");

        pm.bProgressLabel = rootVisualElement.Q<Label>("BProgressLabel");
        pm.bProgressBarVisualElement = rootVisualElement.Q<VisualElement>("BWorkerOverallProgressBarVisualElement");

        pm.UpdateText();
        pm.UpdateProgressBars();
    }

    public void UpdateLevelCompletionText()
    {
        levelProgressionLabel.text = gameManager.gradientManager.numberCompleted + " / " + (gameManager.size * gameManager.size);
        
        if(gameManager.gradientManager.numberCompleted >= gameManager.size * gameManager.size)
        {
            gameManager.uiManager.EnableRestartBG(true);
            restartButton.style.display = DisplayStyle.Flex;
            UpdateRestartButtonText();
        }
    }

    public void BindRestartButton()
    {
        restartButton.clickable.clicked += gameManager.RestartGame;
        restartButton.style.display = DisplayStyle.None;
        EnableRestartBG(false);
    }

    public void EnableRestartBG(bool vis)
    {
        restartButtonBG.style.display = vis ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void UpdateRestartButtonText()
    {
        restartButton.text = "RESTART\n" +
            "Restart the game, double the gradient size (max size: " + gameManager.maxSize + "x" + gameManager.maxSize + "), " +
            "and received +" + gameManager.prestigePointIncrement + " Prestige Points.";
    }

    public void SetupToolTips()
    {
        // Correct Foldout overflow which cannot be changed in ui builder which blocks tooltip visuals
        rootVisualElement.Query("unity-content-viewport").ForEach( e => e.style.overflow = Overflow.Visible);

        CreateTooltip("workerTooltip", wur.workerButton, "Click to add RED components to the gradient");
        CreateTooltip("workerTooltip", wug.workerButton, "Click to add GREEN components to the gradient");
        CreateTooltip("workerTooltip", wub.workerButton, "Click to add BLUE components to the gradient");

        foreach(Worker worker in gameManager.workerManager.workers)
        {
            CreateTooltip(workerUpgradeButtonTooltipTexts[0, 0], worker.workerUpgrade.productionMultiplierButton, "");
            CreateTooltip(workerUpgradeButtonTooltipTexts[1, 0], worker.workerUpgrade.automationButton, "");
            CreateTooltip(workerUpgradeButtonTooltipTexts[2, 0], worker.workerUpgrade.autoTickSpeedMuiltiplierButton, "");
            CreateTooltip(workerUpgradeButtonTooltipTexts[3, 0], worker.workerUpgrade.recycleButton, "");
        }

        CreateTooltip(prestigeButtonTooltipTexts[0, 0], gameManager.prestigeManager.prestigeAutomationButton, "");
        CreateTooltip(prestigeButtonTooltipTexts[1, 0], gameManager.prestigeManager.prestigeRecycleButton, "");
        CreateTooltip(prestigeButtonTooltipTexts[2, 0], gameManager.prestigeManager.prestigeCustomStartAndEndButton, "");
        CreateTooltip(prestigeButtonTooltipTexts[3, 0], gameManager.prestigeManager.prestigeIncreasePixelPointsButton, "");
    }

    public void UpdateTooltipText(string name, VisualElement parentVE, string tooltipText)
    {
        parentVE.Q<VisualElement>(name).Q<Label>().text = tooltipText;
    } 

    public VisualElement CreateTooltip(string name, VisualElement parentVE, string tooltipText)
    {
        // Create and style tooltip VE
        VisualElement toolTip = new VisualElement();
        toolTip.name = name;
        toolTip.style.backgroundColor = toolTipBGColor;
        toolTip.style.position = Position.Absolute;
        toolTip.style.top = -30;
        toolTip.style.borderBottomLeftRadius = 5;
        toolTip.style.borderBottomRightRadius = 5;
        toolTip.style.borderTopLeftRadius = 5;
        toolTip.style.borderTopRightRadius = 5;
        SetAllMarginAndPadding(toolTip, 0);

        // Create and style tooltip label/text
        Label toolTipLabel = new Label();
        toolTipLabel.style.fontSize = 10;
        toolTipLabel.style.color = toolTipTextColor;
        toolTip.Add(toolTipLabel);
        SetAllMarginAndPadding(toolTipLabel, 1);

        // Add to parent
        toolTip.style.visibility = Visibility.Hidden;
        parentVE.Add(toolTip);

        // Create hover behaviour
        parentVE.RegisterCallback<MouseEnterEvent>(e => toolTip.style.visibility = Visibility.Visible);
        parentVE.RegisterCallback<MouseLeaveEvent>(e => toolTip.style.visibility = Visibility.Hidden);

        UpdateTooltipText(name, parentVE, tooltipText);;

        return toolTip;
    }

    private void SetAllMarginAndPadding(VisualElement el, float value)
    {
        el.style.marginBottom = value;
        el.style.marginTop = value;
        el.style.marginLeft = value;
        el.style.marginRight = value;
        el.style.paddingBottom = value;
        el.style.paddingTop = value;
        el.style.paddingLeft = value;
        el.style.paddingRight = value;
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

        // Bind Upgrade Buttons - Green
        wug = gameManager.workerManager.workers[1].GetComponent<WorkerUpgrade>();
        wug.workerButton = rootVisualElement.Q<Button>("Button_G");
        wug.automationButton = rootVisualElement.Q<Button>("AutomateButton_G");
        wug.productionMultiplierButton = rootVisualElement.Q<Button>("IncreaseProductionButton_G");
        wug.autoTickSpeedMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_G");
        wug.recycleButton = rootVisualElement.Q<Button>("RecycleButton_G");

        // Bind Upgrade Buttons - Blue
        wub = gameManager.workerManager.workers[2].GetComponent<WorkerUpgrade>();
        wub.workerButton = rootVisualElement.Q<Button>("Button_B");
        wub.automationButton = rootVisualElement.Q<Button>("AutomateButton_B");
        wub.productionMultiplierButton = rootVisualElement.Q<Button>("IncreaseProductionButton_B");
        wub.autoTickSpeedMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_B");
        wub.recycleButton = rootVisualElement.Q<Button>("RecycleButton_B");
    }

    public void SetupWorkerUpgradeButtons()
    {
        foreach(Worker worker in gameManager.workerManager.workers)
        {
            WorkerUpgrade wupg = worker.workerUpgrade;

            // Setup initial button statuses
            wupg.buttonStatuses.Add(wupg.workerButton, WorkerUpgrade.UpgradeStatus.Purchased);
            wupg.buttonStatuses.Add(wupg.automationButton, WorkerUpgrade.UpgradeStatus.Locked);
            wupg.buttonStatuses.Add(wupg.productionMultiplierButton, WorkerUpgrade.UpgradeStatus.Purchased);
            wupg.buttonStatuses.Add(wupg.autoTickSpeedMuiltiplierButton, WorkerUpgrade.UpgradeStatus.Locked);
            wupg.buttonStatuses.Add(wupg.recycleButton, WorkerUpgrade.UpgradeStatus.Locked);

            // Add button functions
            wupg.workerButton.clickable.clicked += wupg.myWorker.IncrementProgressBar;
            wupg.automationButton.clickable.clicked += wupg.AutomationButton;
            wupg.productionMultiplierButton.clickable.clicked += wupg.ProductionMultiplierButton;
            wupg.autoTickSpeedMuiltiplierButton.clickable.clicked += wupg.AutoTickSpeedMultiplierButton;
            wupg.recycleButton.clickable.clicked += wupg.RecycleButton;

            // Update text when any button is pressed
            wupg.automationButton.clickable.clicked += UpdateWorkerUpgradeButtons;
            wupg.productionMultiplierButton.clickable.clicked += UpdateWorkerUpgradeButtons;
            wupg.autoTickSpeedMuiltiplierButton.clickable.clicked += UpdateWorkerUpgradeButtons;
            wupg.recycleButton.clickable.clicked += UpdateWorkerUpgradeButtons;
        }

        UpdateWorkerUpgradeButtons();
    }

    public void SetupPrestigeButtons()
    {
        PrestigeManager pm = gameManager.prestigeManager;

        // Add button functions
        pm.prestigeAutomationButton.clickable.clicked += pm.PrestigeAutomationButton;
        pm.prestigeRecycleButton.clickable.clicked += pm.PrestigeRecycleButton;
        pm.prestigeCustomStartAndEndButton.clickable.clicked += pm.PrestigeCustomeStartAndEndButton;
        pm.prestigeIncreasePixelPointsButton.clickable.clicked += pm.PrestigeIncreasePixelPointsButton;

        // Update text when any button is pressed
        pm.prestigeAutomationButton.clickable.clicked += UpdatePrestigeButtons;
        pm.prestigeRecycleButton.clickable.clicked += UpdatePrestigeButtons;
        pm.prestigeCustomStartAndEndButton.clickable.clicked += UpdatePrestigeButtons;
        pm.prestigeIncreasePixelPointsButton.clickable.clicked += UpdatePrestigeButtons;

        UpdatePrestigeButtons();

    }

    public void UpdateWorkerUpgradeButtons()
    {
        UpdateWorkerUpgradeButtonDisplay();
        UpdateWorkerUpgradeButtonText();
    }

    public void UpdatePrestigeButtons()
    {
        UpdatePrestigeButtonDisplay();
        UpdatePrestigeButtonText();
    }

    public void UpdateWorkerUpgradeButtonDisplay()
    {
        foreach (Worker worker in gameManager.workerManager.workers)
        {
            UpdateWorkerUpgradeButtonTooltipText(worker);
        }   
    }

    public void UpdatePrestigeButtonDisplay()
    {
        PrestigeManager pm = gameManager.prestigeManager;

        // Automation Button
        if (gameManager.automationEnabled)
        {
            pm.prestigeAutomationButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[0, 0], pm.prestigeAutomationButton, prestigeButtonTooltipTexts[0, 3]);
        }
        else if(gameManager.currencyManager.prestigePoints >= pm.prestigeAutomationCost)
        {
            pm.prestigeAutomationButton.SetEnabled(true);
            UpdateTooltipText(prestigeButtonTooltipTexts[0, 0], pm.prestigeAutomationButton, prestigeButtonTooltipTexts[0, 1]);
        }
        else
        {
            pm.prestigeAutomationButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[0, 0], pm.prestigeAutomationButton, prestigeButtonTooltipTexts[0, 2]);
        }

        // Recycle Button - NOT IMPLEMENTED
        if (gameManager.recycleEnabled)
        {
            pm.prestigeRecycleButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[1, 0], pm.prestigeRecycleButton, prestigeButtonTooltipTexts[1, 3]);
        }
        else if (false)
        {
            pm.prestigeRecycleButton.SetEnabled(true);
            UpdateTooltipText(prestigeButtonTooltipTexts[1, 0], pm.prestigeRecycleButton, prestigeButtonTooltipTexts[1, 1]);
        }
        else if(false)
        {
            pm.prestigeRecycleButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[1, 0], pm.prestigeRecycleButton, prestigeButtonTooltipTexts[1, 2]);
        }

        // CustomStartAndEnd Button - NOT IMPLEMENTED
        if (false)
        {
            pm.prestigeCustomStartAndEndButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[2, 0], pm.prestigeCustomStartAndEndButton, prestigeButtonTooltipTexts[2, 3]);
        }
        else if (false)
        {
            pm.prestigeCustomStartAndEndButton.SetEnabled(true);
            UpdateTooltipText(prestigeButtonTooltipTexts[2, 0], pm.prestigeCustomStartAndEndButton, prestigeButtonTooltipTexts[2, 1]);
        }
        else if (false)
        {
            pm.prestigeCustomStartAndEndButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[2, 0], pm.prestigeCustomStartAndEndButton, prestigeButtonTooltipTexts[2, 2]);
        }

        // IncreasePixelPoints Button - NOT IMPLEMENTED
        if (gameManager.currencyManager.prestigePoints >= pm.prestigeIncreasePixelPointsCost)
        {
            pm.prestigeIncreasePixelPointsButton.SetEnabled(true);
            UpdateTooltipText(prestigeButtonTooltipTexts[3, 0], pm.prestigeIncreasePixelPointsButton, prestigeButtonTooltipTexts[3, 1]);
        }
        else
        {
            pm.prestigeIncreasePixelPointsButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[3, 0], pm.prestigeIncreasePixelPointsButton, prestigeButtonTooltipTexts[3, 2]);
        }
    }

    public void UpdateWorkerUpgradeButtonText()
    {
        foreach (Worker worker in gameManager.workerManager.workers)
        {
            WorkerUpgrade wupg = worker.workerUpgrade;
            wupg.automationButton.Q<Label>().text = ""; //wupg.buttonStatuses[wupg.automationButton] == WorkerUpgrade.UpgradeStatus.Purchased ? "" : "Cost: " + wupg.automationCost.ToString();
            wupg.productionMultiplierButton.Q<Label>().text = "Level: " + wupg.productionLevel.ToString() + "\n" + "Cost: " + wupg.ProductionMultiplierCost().ToString();
            wupg.autoTickSpeedMuiltiplierButton.Q<Label>().text = "Level: " + wupg.autoTickSpeedLevel.ToString() + "\n" + "Cost: " + wupg.AutoTickSpeedMultiplierCost().ToString();
            wupg.recycleButton.Q<Label>().text = "Level: 0\nCost: " + wupg.recycleMultiplierCost.ToString();
        }
    }

    public void UpdatePrestigeButtonText()
    {
        PrestigeManager pm = gameManager.prestigeManager;

        pm.prestigeAutomationButton.Q<Label>().text = "Unlock Automation\nCost: " + pm.prestigeAutomationCost.ToString();
        pm.prestigeRecycleButton.Q<Label>().text = "Unlock Recycle\nCost: " + pm.prestigeRecycleCost.ToString();
        pm.prestigeCustomStartAndEndButton.Q<Label>().text = "Unlock Custom Start\nand End Colors\nCost: " + pm.prestigeCustomStartAndEndCost.ToString();
        pm.prestigeIncreasePixelPointsButton.Q<Label>().text = "Increase Starting Pixel\nPoints +1\n(Current: " + gameManager.startingPixelPoints + ")\nCost: " + pm.prestigeIncreasePixelPointsCost.ToString();
    }

    private void UpdateWorkerUpgradeButtonTooltipText(Worker worker)
    {
        WorkerUpgrade wupg = worker.workerUpgrade;
        Button workerButton;
        WorkerUpgrade.UpgradeStatus buttonStatus;
        int upgradeCost;
        string tooltipName;

        Button[] buttons = new Button[4]
        {
            wupg.productionMultiplierButton,
            wupg.automationButton,
            wupg.autoTickSpeedMuiltiplierButton,
            wupg.recycleButton
        };

        Dictionary<string, int> costs = new Dictionary<string, int>();
        costs.Add(workerUpgradeButtonTooltipTexts[0, 0], wupg.ProductionMultiplierCost());
        costs.Add(workerUpgradeButtonTooltipTexts[1, 0], wupg.automationCost);
        costs.Add(workerUpgradeButtonTooltipTexts[2, 0], wupg.AutoTickSpeedMultiplierCost());
        costs.Add(workerUpgradeButtonTooltipTexts[3, 0], wupg.recycleMultiplierCost);

        for (int i = 0; i < buttons.Length; i++)
        {
            tooltipName = workerUpgradeButtonTooltipTexts[i, 0];
            workerButton = buttons[i];
            buttonStatus = wupg.buttonStatuses[workerButton];
            upgradeCost = costs[tooltipName];

            if (buttonStatus == WorkerUpgrade.UpgradeStatus.Locked)
            {
                workerButton.SetEnabled(false);
                gameManager.uiManager.UpdateTooltipText(tooltipName, workerButton, workerUpgradeButtonTooltipTexts[i, 1]);
            }
            else if (buttonStatus == WorkerUpgrade.UpgradeStatus.Unlocked)
            {
                bool canAfford = gameManager.currencyManager.pixelPoints >= upgradeCost;
                if (canAfford)
                {
                    workerButton.SetEnabled(true);
                    gameManager.uiManager.UpdateTooltipText(tooltipName, workerButton, workerUpgradeButtonTooltipTexts[i, 2]);
                }
                else
                {
                    workerButton.SetEnabled(false);
                    gameManager.uiManager.UpdateTooltipText(tooltipName, workerButton, workerUpgradeButtonTooltipTexts[i, 3]);
                }
            }
            else if (buttonStatus == WorkerUpgrade.UpgradeStatus.Purchased)
            {
                bool canAfford = gameManager.currencyManager.pixelPoints >= upgradeCost;
                if (canAfford)
                {
                    workerButton.SetEnabled(true);
                    gameManager.uiManager.UpdateTooltipText(tooltipName, workerButton, workerUpgradeButtonTooltipTexts[i, 4]);
                }
                else
                {
                    workerButton.SetEnabled(false);
                    gameManager.uiManager.UpdateTooltipText(tooltipName, workerButton, workerUpgradeButtonTooltipTexts[i, 5]);
                }
            }
        }
    }

    public void BindPrestigeButtons()
    {
        gameManager.prestigeManager.prestigeAutomationButton = rootVisualElement.Q<Button>("pButton_UnlockAutomation");
        gameManager.prestigeManager.prestigeRecycleButton = rootVisualElement.Q<Button>("pButton_UnlockRecycle");
        gameManager.prestigeManager.prestigeCustomStartAndEndButton = rootVisualElement.Q<Button>("pButton_UnlockCustomStartAndEndColors");
        gameManager.prestigeManager.prestigeIncreasePixelPointsButton = rootVisualElement.Q<Button>("pButton_IncreaseStartingPixelPoints");
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

    private void InstantiateFoldouts()
    {
        gameFoldout = rootVisualElement.Q<Foldout>("FoldoutGame");
        prestigeStoreFoldout = rootVisualElement.Q<Foldout>("FoldoutStore");
        progressFoldout = rootVisualElement.Q<Foldout>("FoldoutProgress");
        //settingsFoldout = rootVisualElement.Q<Foldout>("FoldoutSettings");

        foldouts.Add(gameFoldout);
        foldouts.Add(prestigeStoreFoldout);
        foldouts.Add(progressFoldout);
        //foldouts.Add(settingsFoldout);


        gameFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(gameFoldout));
        prestigeStoreFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(prestigeStoreFoldout));
        progressFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(progressFoldout));
        //settingsFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(settingsFoldout));
    }

    public void CloseOtherFoldouts(Foldout exceptThisOne)
    {
        foreach (Foldout foldout in foldouts)
        {
            if (foldout == exceptThisOne)
                continue;

            foldout.SetValueWithoutNotify(false);
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