using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.UIElements.Experimental;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get => instance; }
    private static UIManager instance;
    public GameManager gameManager;

    [SerializeField] public UnityEngine.UIElements.UIDocument uiDocment { get; private set; }
    public VisualElement rootVisualElement;
    public VisualElement gradientArea { get; private set; }
    public VisualElement[,] activeGradientVisualElements { get; private set; }
    private int prevSize = 0;
    private Label levelProgressionLabel;

    [Header("Tooltips")]
    public Color toolTipBGColor = Color.black;
    public Color toolTipTextColor = Color.white;

    [Header("Worker Upgrade References")]
    public WorkerUpgrade wur;
    public WorkerUpgrade wug;
    public WorkerUpgrade wub;

    [Header("Restart Button")]
    public Button restartButton;
    public VisualElement restartVisualElement;

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
            "Automation already purchased", // 3 Purchased
        },
        { // 1 Recycle Texts
            "prestigeRecycleTooltip",  // 0 Tooltip Name
            "Buy to enable Recycle", // 1 Can Afford
            "Buy to enable Recycle\nYou need more Prestige Points to buy this", // 2 Can NOT Afford
            "Recycle already purchased", // 3 Purchased
        },
        { // 2 Custom Start and End Texts
            "prestigeCustomStartAndEndTooltip",  // 0 Tooltip Name
            "Buy to enable custom start and end colours on game restart", // 1 Can Afford
            "Buy to enable custom start and end colours on game restart\nYou need more Prestige Points to buy this", // 2 Can NOT Afford
            "Custom Colors already purchased", // 3 Purchased
        },
        { // 3 Increase Pixel Points Texts
            "prestigeIncreasePixelPointsTooltip",  // 0 Tooltip Name
            "Buy to increase the amount of Pixel Points you restart with", // 1 Can Afford
            "Buy to increase the amount of Pixel Points you restart with\nYou need more Prestige Points to buy this", // 2 Can NOT Afford
            "PURCAHSED IS AN INVALID STATE FOR INCREASE PIXEL POINTS", // 3 Purchased
        }
    };

    // Worker Upgarde Button Tooltip Texts
    private string[,] workerUpgradeButtonTooltipTexts = new string[4, 8]
    {
        {   // 0 Production Texts
            "productionMultiplierTooltip", // 0 Tooltip Name
            "LOCKED IS INVALID STATUS FOR PROD MULTI", // 1 Locked
            "UNLOCKED IS INVALID STATUS FOR PROD MULTI", // 2 Unlocked CAN afford
            "UNLOCKED IS INVALID STATUS FOR PROD MULTI", // 3 Unlocked CAN NOT afford
            "Increase Production\n(The tick amount for manual clicking)", // 4 Purchased CAN afford
            "Increase Production\nYou need more Pixel Points to buy this", // 5 Purchased CAN NOT afford
            "INVALID", // Enabled
            "INVALID" // Diabled
        },
        {   // 1 Automation Texts
            "automationTooltip", // 0 Tooltip Name
            "Unlock Automation\nin the Prestige Store", // 1 Locked
            "Enable Automation", // 2 Unlocked CAN afford
            "Automation\nYou need more Pixel Points to buy this", // 3 Unlocked CAN NOT afford
            "Automation Enabled", // 4 Purchased CAN afford
            "Automation Enabled", // 5 Purchased CAN NOT afford
            "Click to DISABLE Automation", // Enabled
            "Click to ENABLE Automation" // Diabled
        },
        {   // 2 TickSpeed Texts
            "autoTickSpeedTootip", // 0 Tooltip Name
            "Unlock Automation\nin the Prestige Store", // 1 Locked
            "Increase Speed", // 2 Unlocked CAN afford
            "Increase Speed\nYou need more Pixel Points to buy this", // 3 Unlocked CAN NOT afford
            "Increase Speed\n(The speed of automated filling)", // 4 Purchased CAN afford
            "Increase Speed\nYou need more Pixel Points to buy this", // 5 Purchased CAN NOT afford
            "INVALID", // Enabled
            "INVALID" // Diabled
        },
        {   // 3 Recycle Texts
            "recycleTooltip", // 0 Tooltip Name
            "Unlock Recycle\nin the Prestige Store", // 1 Locked
            "Enable Recycle", // 2 Unlocked CAN afford
            "Enable Recycle\nYou need more Pixel Points to buy this", // 3 Unlocked CAN NOT afford
            "Increase Recycle Amount", // 4 Purchased CAN afford
            "Increase Recycle Amount\nYou need more Pixel Points to buy this", // 5 Purchased CAN NOT afford
            "Click to DISABLE Recycle", // Enabled
            "Click to ENABLE Recycle" // Diabled
        }
    };

    // Custom Start and End Colors
    private bool isRestarting = false;
    private VisualElement cseContainer; // Toggle based on if unlocked
    private VisualElement startColorImage;
    private VisualElement endColorImage;
    private Button randomStartColorButton;
    private Button randomEndColorButton;
    private SliderInt[] sliderInts = new SliderInt[6];
    private SliderInt startSliderRed;
    private SliderInt startSliderGreen;
    private SliderInt startSliderBlue;
    private SliderInt endSliderRed;
    private SliderInt endSliderGreen;
    private SliderInt endSliderBlue;
    public Color32 startColor = Color.black;
    public Color32 endColor = Color.black;

    // Save, Load, and Reset
    private Button saveButton;
    private Button loadButton;
    private Button resetButton;

    // Progress
    private VisualElement progressPopup;
    private VisualElement progressVE1;
    private VisualElement progressVE2;
    private VisualElement progressVE3;
    private VisualElement progressVE4;
    private VisualElement progressVE5;
    private VisualElement progressVE6;

    // Help/Welcome
    private VisualElement welcomeScreen;
    private Button welcomeScreenAutoHide;
    private Button helpButton;
    private bool helpToggle = true;

    // Popup
    private VisualElement popupVE;
    private Button popupCloseButton;
    private Label popupTitle;
    private Label popupLabel;

    // Scrollbar Fix
    private float scrollVelocity;
    private float scrollDamping = 0.1f;
    private List<Scroller> verticalScrollers = new List<Scroller>();

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
        restartVisualElement = rootVisualElement.Q<VisualElement>("RestartVisualElement");
        levelProgressionLabel = rootVisualElement.Q<Label>("LevelProgressLabel");
    }

    private void Update()
    {
        if (isRestarting)
        {
            startColor = new Color32((byte)startSliderRed.value, (byte)startSliderGreen.value, (byte)startSliderBlue.value, 255);
            endColor = new Color32((byte)endSliderRed.value, (byte)endSliderGreen.value, (byte)endSliderBlue.value, 255);
            startColorImage.style.backgroundColor = (Color)startColor;
            endColorImage.style.backgroundColor = (Color)endColor;
        }

        if (verticalScrollers.Count > 0)
        {
            foreach(Scroller scroller in verticalScrollers)
            {
                scroller.value += scrollVelocity;
                scrollVelocity -= scrollVelocity * scrollDamping;
            }
        }
    }

    public IEnumerator PopUp(int msIn, int msWait, int msOut)
    {
        progressPopup.experimental.animation.Start(-25f, 20f, msIn, (ve, val) =>
        {
            ve.style.bottom = val;
        }).Ease(Easing.OutBounce);

        yield return new WaitForSeconds(msWait * 0.001f);

        progressPopup.experimental.animation.Start(20f, -25f, msOut * 2, (ve, val) =>
        {
            ve.style.bottom = val;
        });

        yield return null;
    }

    public void InitialUISetup()
    {
        AssignProgressUIComponents();
        AssignPopupComponents();

        BindAndSetSettingsButtons();
        BindWorkerUpgradeButtons();
        BindPrestigeButtons();
        BindCustomStartEndColorElements();
        BindExtraButtons();

        SetupToolTips(); // must be after bind but before setup

        SetupWorkerUpgradeButtons();
        SetupPrestigeButtons();
        SetupRandomColorButtons();

        UpdateLevelCompletionText();
        gameManager.gradientManager.CheckGradientStatus();
        InstantiateFoldouts();
        ScrollBarSpeedFix();

        SaveSystem.Load();

        gameManager.progressManager.CheckCurrentLevel();
        gameManager.progressManager.UpdateAllBlocks();
        UpdateProgressUI();
        gameManager.progressManager.UpdateText();
    }

    private void ScrollBarSpeedFix()
    {
        // Only affect Progress view
        var progressView = progressFoldout.Q<ScrollView>();
        var progressScroller = progressView.Q<Scroller>();

        progressView.RegisterCallback<WheelEvent>(e =>
        {
            scrollVelocity += e.delta.y * 100;
            // Stop the event here so the builtin scroll functionality of the list doesn't activate
            e.StopPropagation();
        });

        verticalScrollers.Add(progressScroller);

        // Stop scroller for main scene
        var gameView = gameFoldout.Q<ScrollView>();
        Scroller hScroller = rootVisualElement.Query<Scroller>().AtIndex(1);
        gameView.RegisterCallback<WheelEvent>(e =>
        {
            hScroller.value = 0;
        });
    }

    private void AssignPopupComponents()
    {
        popupVE = rootVisualElement.Q<VisualElement>("Popup");
        popupCloseButton = popupVE.Q<Button>("ClosePopup");
        popupTitle = popupVE.Q<Label>("Title");
        popupLabel = popupVE.Q<Label>("Body");

        popupCloseButton.clickable.clicked += ClosePopup;
    }

    public void ShowPopup(string bodyText, string titleText = "")
    {
        if (titleText != "")
            popupTitle.text = titleText;

        popupLabel.text = bodyText;

        popupVE.style.display = DisplayStyle.Flex;
    }

    private void ClosePopup()
    {
        popupVE.style.display = DisplayStyle.None;
    }

    private void AssignProgressUIComponents()
    {
        ProgressManager pm = gameManager.progressManager;
        pm.progressFoldout = rootVisualElement.Q<Foldout>("FoldoutProgress");

        progressPopup = rootVisualElement.Q<VisualElement>("ProgressMadeVE");

        progressVE1 = rootVisualElement.Q<VisualElement>("ProgressVisualElement1").Q<VisualElement>("HorizontalGridVisualElement");
        progressVE1.Q<Label>().text = "Level 1 - Monochrome (limited to using 0, 128, 255 values)";

        progressVE2 = rootVisualElement.Q<VisualElement>("ProgressVisualElement2").Q<VisualElement>("HorizontalGridVisualElement");
        progressVE2.Q<Label>().text = "Level 2 - Complete Level 1 Goals to Unlock";
        progressVE2.style.display = DisplayStyle.None;

        progressVE3 = rootVisualElement.Q<VisualElement>("ProgressVisualElement3").Q<VisualElement>("HorizontalGridVisualElement");
        progressVE3.Q<Label>().text = "Level 3 - Complete Level 2 Goals to Unlock";
        progressVE3.style.display = DisplayStyle.None;

        progressVE4 = rootVisualElement.Q<VisualElement>("ProgressVisualElement4").Q<VisualElement>("HorizontalGridVisualElement");
        progressVE4.Q<Label>().text = "Level 4 - Complete Level 3 Goals to Unlock";
        progressVE4.style.display = DisplayStyle.None;

        progressVE5 = rootVisualElement.Q<VisualElement>("ProgressVisualElement5").Q<VisualElement>("HorizontalGridVisualElement");
        progressVE5.Q<Label>().text = "Level 5 - Complete Level 4 Goals to Unlock";
        progressVE5.style.display = DisplayStyle.None;

        progressVE6 = rootVisualElement.Q<VisualElement>("ProgressVisualElement6").Q<VisualElement>("HorizontalGridVisualElement");
        progressVE6.Q<Label>().text = "Level 6 - Complete Level 5 Goals to Unlock";
        progressVE6.style.display = DisplayStyle.None;

        CreateLevelGoalBlockElements();
    }

    public void CreateLevelGoalBlockElements()
    {
        ProgressManager pm = gameManager.progressManager;

        for (int i = 0; i < pm.levelGoals.Count; i++)
        {
            Goal goal = pm.levelGoals[i];

            VisualElement progressBlock = goal.blockVisualElement;
            progressBlock.AddToClassList("color-block");
            progressBlock.style.backgroundColor = (Color)new Color32(255, 255, 255, 255);
            //progressBlock.style.backgroundColor = (Color)new Color32(goal.color32.r, goal.color32.g, goal.color32.b, 255);

            Label lbl = new Label();
            lbl.text = "(" + goal.color32.r + ", " + goal.color32.g + ", " + goal.color32.b + ")";
            progressBlock.Add(lbl);

            switch (goal.level)
            {
                case 1:
                    progressVE1.Add(progressBlock);
                    break;
                case 2:
                    progressVE2.Add(progressBlock);
                    break;
                case 3:
                    progressVE3.Add(progressBlock);
                    break;
                case 4:
                    progressVE4.Add(progressBlock);
                    break;
                case 5:
                    progressVE5.Add(progressBlock);
                    break;
                case 6:
                    progressVE6.Add(progressBlock);
                    break;
            }

        }
    }

    public void UpdateProgressUI()
    {
        ProgressManager pm = gameManager.progressManager;
        int currentLevel = pm.currentLevel;

        if(currentLevel >= 2)
        {
            progressVE2.style.display = DisplayStyle.Flex;
            progressVE2.Q<Label>().text = "Level 2 - The Basics<br>(limited to using 0, 128, 255 values)";
        }
        
        if(currentLevel >= 3)
        {
            progressVE3.style.display = DisplayStyle.Flex;
            progressVE3.Q<Label>().text = "Level 3 - Secondary Colors<br>(limited to using 0, 128, 255 values)";
        }

        if (currentLevel >= 4)
        {
            progressVE4.style.display = DisplayStyle.Flex;
            progressVE4.Q<Label>().text = "Level 4 - Tertiary Colors<br>(limited to using 0, 128, 255 values)";
        }

        if (currentLevel >= 5)
        {
            progressVE5.style.display = DisplayStyle.Flex;
            progressVE5.Q<Label>().text = "Level 5 - The Light Side of the Rainbow<br>(limited to using 0, 32, 64, 128, 192, 255 values)";
        }

        if (currentLevel >= 6)
        {
            progressVE6.style.display = DisplayStyle.Flex;
            progressVE6.Q<Label>().text = "Level 6 - The Dark Side of the Rainbow<br>(limited to using 0, 32, 64, 128, 192, 255 values)";
        }
    }

    public void UpdateLevelCompletionText()
    {
        levelProgressionLabel.text = gameManager.gradientManager.numberCompleted + " / " + (gameManager.size * gameManager.size);
    }

    private void BindAndSetSettingsButtons()
    {
        saveButton = rootVisualElement.Q<Button>("saveButton");
        loadButton = rootVisualElement.Q<Button>("loadButton");
        resetButton = rootVisualElement.Q<Button>("deleteButton");

        saveButton.clickable.clicked += SaveSystem.Save;
        loadButton.clickable.clicked += SaveSystem.Load;
        resetButton.clickable.clicked += SaveSystem.DeleteSaveFile;
    }

    public void BindExtraButtons()
    {
        // Help
        welcomeScreen = rootVisualElement.Q<VisualElement>("Welcome");
        welcomeScreenAutoHide = rootVisualElement.Q<Button>("WelcomeScreenToggle");
        welcomeScreenAutoHide.clickable.clicked += HideWelcomeMessage;
        helpButton = rootVisualElement.Q<Button>("HelpButton");
        helpButton.clickable.clicked += ToggleHelpMenu;

        // Restart
        restartButton.clickable.clicked += RestartCheck;
        EnableRestartVisualElement(false);
    }

    private void RestartCheck()
    {
        if (startSliderRed.value == endSliderRed.value &&
            startSliderGreen.value == endSliderGreen.value &&
            startSliderBlue.value == endSliderBlue.value)
        {
            ShowPopup(
                "The start and end colors must be different for a gradient! Make sure they do not have the same red, green, and blue values.",
                "<b>Invalid Start/End Colors</b>"
                );
        }
        else
        {
            gameManager.RestartGame();
        }
    }

    private void HideWelcomeMessage()
    {
        helpToggle = false;
        ToggleHelpMenu();
    }

    private void ToggleHelpMenu()
    {
        if (helpToggle)
            welcomeScreen.style.display = DisplayStyle.Flex;
        else
            welcomeScreen.style.display = DisplayStyle.None;

        helpToggle = !helpToggle;
    }

    public void EnableRestartVisualElement(bool vis)
    {
        isRestarting = vis;
        restartVisualElement.style.display = vis ? DisplayStyle.Flex : DisplayStyle.None;
        cseContainer.style.display = vis && gameManager.customColorEnabled ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void UpdateRestartButtonText()
    {
        restartButton.text = "Restart the Gradient!\n" +
            "Receive +" + gameManager.prestigePointIncrement + " Prestige Points when you complete a full gradient.";
    }

    public void SetupToolTips()
    {
        // Worker Tooltips
        CreateTooltip("workerTooltip", wur.workerButton, "Click to add RED components to the gradient");
        CreateTooltip("workerTooltip", wug.workerButton, "Click to add GREEN components to the gradient");
        CreateTooltip("workerTooltip", wub.workerButton, "Click to add BLUE components to the gradient");

        // Worker Upgrade Tooltips
        foreach (Worker worker in gameManager.workerManager.workers)
        {
            CreateTooltip(workerUpgradeButtonTooltipTexts[0, 0], worker.workerUpgrade.productionMultiplierButton, "");
            CreateTooltip(workerUpgradeButtonTooltipTexts[1, 0], worker.workerUpgrade.automationButton, "");
            CreateTooltip(workerUpgradeButtonTooltipTexts[2, 0], worker.workerUpgrade.autoTickSpeedMuiltiplierButton, "");
            CreateTooltip(workerUpgradeButtonTooltipTexts[3, 0], worker.workerUpgrade.recycleButton, "");
        }

        // Prestige Tooltips
        CreateTooltip(prestigeButtonTooltipTexts[0, 0], gameManager.prestigeManager.prestigeAutomationButton, "");
        CreateTooltip(prestigeButtonTooltipTexts[1, 0], gameManager.prestigeManager.prestigeRecycleButton, "");
        CreateTooltip(prestigeButtonTooltipTexts[2, 0], gameManager.prestigeManager.prestigeCustomStartAndEndButton, "");
        CreateTooltip(prestigeButtonTooltipTexts[3, 0], gameManager.prestigeManager.prestigeIncreasePixelPointsButton, "");

        // Start and End Color Tooltips
        //CreateTooltip("startColorTooltip", startColorImage, "Restart with this color in the bottom left");
        //CreateTooltip("endColorTooltip", endColorImage, "Restart with this color in the top right");
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

        UpdateTooltipText(name, parentVE, tooltipText); ;

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
        wur.automationEnabledIcon = wur.automationButton.Q<VisualElement>("IconAutomationEnabled");
        wur.automationDisabledIcon = wur.automationButton.Q<VisualElement>("IconAutomationDisabled");
        wur.productionMultiplierButton = rootVisualElement.Q<Button>("IncreaseProductionButton_R");
        wur.autoTickSpeedMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_R");
        wur.recycleButton = rootVisualElement.Q<Button>("RecycleButton_R");

        // Bind Upgrade Buttons - Green
        wug = gameManager.workerManager.workers[1].GetComponent<WorkerUpgrade>();
        wug.workerButton = rootVisualElement.Q<Button>("Button_G");
        wug.automationButton = rootVisualElement.Q<Button>("AutomateButton_G");
        wug.automationEnabledIcon = wug.automationButton.Q<VisualElement>("IconAutomationEnabled");
        wug.automationDisabledIcon = wug.automationButton.Q<VisualElement>("IconAutomationDisabled");
        wug.productionMultiplierButton = rootVisualElement.Q<Button>("IncreaseProductionButton_G");
        wug.autoTickSpeedMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_G");
        wug.recycleButton = rootVisualElement.Q<Button>("RecycleButton_G");

        // Bind Upgrade Buttons - Blue
        wub = gameManager.workerManager.workers[2].GetComponent<WorkerUpgrade>();
        wub.workerButton = rootVisualElement.Q<Button>("Button_B");
        wub.automationButton = rootVisualElement.Q<Button>("AutomateButton_B");
        wub.automationEnabledIcon = wub.automationButton.Q<VisualElement>("IconAutomationEnabled");
        wub.automationDisabledIcon = wub.automationButton.Q<VisualElement>("IconAutomationDisabled");
        wub.productionMultiplierButton = rootVisualElement.Q<Button>("IncreaseProductionButton_B");
        wub.autoTickSpeedMuiltiplierButton = rootVisualElement.Q<Button>("IncreaseSpeedButton_B");
        wub.recycleButton = rootVisualElement.Q<Button>("RecycleButton_B");
    }

    public void SetupWorkerUpgradeButtons()
    {
        foreach (Worker worker in gameManager.workerManager.workers)
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
        else if (gameManager.currencyManager.prestigePoints >= pm.prestigeAutomationCost)
        {
            pm.prestigeAutomationButton.SetEnabled(true);
            UpdateTooltipText(prestigeButtonTooltipTexts[0, 0], pm.prestigeAutomationButton, prestigeButtonTooltipTexts[0, 1]);
        }
        else
        {
            pm.prestigeAutomationButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[0, 0], pm.prestigeAutomationButton, prestigeButtonTooltipTexts[0, 2]);
        }

        // Recycle Button
        if (gameManager.recycleEnabled)
        {
            pm.prestigeRecycleButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[1, 0], pm.prestigeRecycleButton, prestigeButtonTooltipTexts[1, 3]);
        }
        else if (gameManager.currencyManager.prestigePoints >= pm.prestigeRecycleCost)
        {
            pm.prestigeRecycleButton.SetEnabled(true);
            UpdateTooltipText(prestigeButtonTooltipTexts[1, 0], pm.prestigeRecycleButton, prestigeButtonTooltipTexts[1, 1]);
        }
        else
        {
            pm.prestigeRecycleButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[1, 0], pm.prestigeRecycleButton, prestigeButtonTooltipTexts[1, 2]);
        }

        // CustomStartAndEnd Button
        if (gameManager.customColorEnabled)
        {
            pm.prestigeCustomStartAndEndButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[2, 0], pm.prestigeCustomStartAndEndButton, prestigeButtonTooltipTexts[2, 3]);
        }
        else if (gameManager.currencyManager.prestigePoints >= pm.prestigeCustomStartAndEndCost)
        {
            pm.prestigeCustomStartAndEndButton.SetEnabled(true);
            UpdateTooltipText(prestigeButtonTooltipTexts[2, 0], pm.prestigeCustomStartAndEndButton, prestigeButtonTooltipTexts[2, 1]);
        }
        else
        {
            pm.prestigeCustomStartAndEndButton.SetEnabled(false);
            UpdateTooltipText(prestigeButtonTooltipTexts[2, 0], pm.prestigeCustomStartAndEndButton, prestigeButtonTooltipTexts[2, 2]);
        }

        // IncreasePixelPoints Button
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
            wupg.autoTickSpeedMuiltiplierButton.Q<Label>().text = "Level: " + wupg.autoTickSpeedLevel.ToString() + "\nCost: " + wupg.AutoTickSpeedMultiplierCost().ToString();
            wupg.recycleButton.Q<Label>().text = "Level: " + wupg.recycleLevel.ToString() + "\nCost: " + wupg.RecycleMultiplierCost().ToString();

            if (wupg.productionLevel < 10)
                wupg.productionMultiplierButton.Q<Label>().text = "Level: " + wupg.productionLevel.ToString() + "\nCost: " + wupg.ProductionMultiplierCost().ToString();
            else
                wupg.productionMultiplierButton.Q<Label>().text = "Level: MAX";
        }
    }

    public void UpdatePrestigeButtonText()
    {
        PrestigeManager pm = gameManager.prestigeManager;

        pm.prestigeAutomationButton.Q<Label>().text = "Unlock Automation\nCost: " + pm.prestigeAutomationCost.ToString() + "\nAutomation will fill the bar without clicking.";
        pm.prestigeRecycleButton.Q<Label>().text = "Unlock Recycle\nCost: " + pm.prestigeRecycleCost.ToString() + "\nRecycle adds an extra random red, green, or blue pixel to the grid every time the bar fills up.";
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
        costs.Add(workerUpgradeButtonTooltipTexts[3, 0], wupg.RecycleMultiplierCost());

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
            else if (buttonStatus == WorkerUpgrade.UpgradeStatus.Enabled)
            {
                workerButton.SetEnabled(true);
                gameManager.uiManager.UpdateTooltipText(tooltipName, workerButton, workerUpgradeButtonTooltipTexts[i, 6]);
            }
            else if (buttonStatus == WorkerUpgrade.UpgradeStatus.Disabled)
            {
                workerButton.SetEnabled(true);
                gameManager.uiManager.UpdateTooltipText(tooltipName, workerButton, workerUpgradeButtonTooltipTexts[i, 7]);
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

    private void BindCustomStartEndColorElements()
    {
        cseContainer = rootVisualElement.Q<VisualElement>("ColorPickerVisualElement");
        startColorImage = rootVisualElement.Q<VisualElement>("StartImage");
        endColorImage = rootVisualElement.Q<VisualElement>("EndImage");
        randomStartColorButton = rootVisualElement.Q<Button>("RandomStartColorButton");
        randomEndColorButton = rootVisualElement.Q<Button>("RandomEndColorButton");
        startSliderRed = rootVisualElement.Q<SliderInt>("StartRedSlider");
        startSliderGreen = rootVisualElement.Q<SliderInt>("StartGreenSlider");
        startSliderBlue = rootVisualElement.Q<SliderInt>("StartBlueSlider");
        endSliderRed = rootVisualElement.Q<SliderInt>("EndRedSlider");
        endSliderGreen = rootVisualElement.Q<SliderInt>("EndGreenSlider");
        endSliderBlue = rootVisualElement.Q<SliderInt>("EndBlueSlider");

        sliderInts[0] = startSliderRed;
        sliderInts[1] = startSliderGreen;
        sliderInts[2] = startSliderBlue;
        sliderInts[3] = endSliderRed;
        sliderInts[4] = endSliderGreen;
        sliderInts[5] = endSliderBlue;

        foreach(SliderInt slider in sliderInts)
            slider.RegisterValueChangedCallback(e => LimitSliderValues());
    }

    private void SetupRandomColorButtons()
    {
        randomStartColorButton.clickable.clicked += RandomCustomStartColor;
        randomEndColorButton.clickable.clicked += RandomCustomEndColor;
    }

    private void RandomCustomStartColor()
    {
        startSliderRed.value = UnityEngine.Random.Range(0, 255);
        startSliderGreen.value = UnityEngine.Random.Range(0, 255);
        startSliderBlue.value = UnityEngine.Random.Range(0, 255);
    }

    private void RandomCustomEndColor()
    {
        endSliderRed.value = UnityEngine.Random.Range(0, 255);
        endSliderGreen.value = UnityEngine.Random.Range(0, 255);
        endSliderBlue.value = UnityEngine.Random.Range(0, 255);
    }

    private void LimitSliderValues()
    {
        int level = gameManager.progressManager.currentLevel;
        
        if(level <= 4)
        {
            int[] validValues = new int[] { 0, 128, 255 };
            foreach (SliderInt slider in sliderInts)
                slider.value = RoundToValidValue(validValues, slider.value);

        }
        else if(level <= 6)
        {
            int[] validValues = new int[] { 0, 32, 64, 128, 192, 255 };
            foreach (SliderInt slider in sliderInts)
                slider.value = RoundToValidValue(validValues, slider.value);
        }        
    }

    private int RoundToValidValue(int[] validValues, int value)
    {
        // https://stackoverflow.com/questions/7865833/rounding-a-value-to-only-a-list-of-certain-values-in-c-sharp

        var diffList = from number in validValues
                       select new { 
                           number, 
                           difference = Mathf.Abs(number - value)
                       };
        var result = (from diffItem in diffList
                      orderby diffItem.difference
                      select diffItem).First().number;
        
        return result;
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
        activeGradientVisualElements[i, j].style.backgroundColor = (Color)gameManager.gradientManager.gradientGColors[i, j].ToColor32();
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
        settingsFoldout = rootVisualElement.Q<Foldout>("FoldoutSettings");

        foldouts.Add(gameFoldout);
        foldouts.Add(prestigeStoreFoldout);
        foldouts.Add(progressFoldout);
        foldouts.Add(settingsFoldout);

        gameFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(gameFoldout));
        prestigeStoreFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(prestigeStoreFoldout));
        progressFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(progressFoldout));
        settingsFoldout.RegisterValueChangedCallback(e => CloseOtherFoldouts(settingsFoldout));

        // uss alterations
        progressFoldout.Q<VisualElement>("unity-content").style.overflow = Overflow.Hidden;
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