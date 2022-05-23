using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CustomCursor
{
    Default,
    OnEnemy
}

[Flags]
public enum UISyncOperation
{
    Health = 1,
    Resource = 2,
    Experience = 4,
    Stats = 8,
    Level = 16,
    HRE = (Health | Resource | Experience),
    ALL = (Health | Resource | Experience | Stats | Level)
}

public class UIManager : MonoBehaviour
{
    [Header("Mouse Cursor")]
    public Texture2D defaultCursor;
    public Texture2D onEnemyCursor;

    [Header("Item UI References")]
    [SerializeField]
    private UIItemDescriptionPopup itemDescriptionPanel;

    [Header("Player UI References")]
    [SerializeField] 
    private Text healthText;
    [SerializeField]
    private Text resourceText;
    [SerializeField]
    private Material healthMaterial;
    [SerializeField]
    private Material resourceMaterial;
    [SerializeField]
    private Material experienceMaterial;
    [SerializeField]
    private Text playerDescription;
    [SerializeField]
    private Text strengthText;
    [SerializeField]
    private Text constitutionText;
    [SerializeField]
    private Text agilityText;
    [SerializeField] 
    private Text intelligenceText;
    [SerializeField]
    private Text availablePointsText;
    [SerializeField]
    private Button strengthAddBtn;
    [SerializeField]
    private Button strengthSubBtn;
    [SerializeField]
    private Button constitutionAddBtn;
    [SerializeField]
    private Button constitutionSubBtn;
    [SerializeField]
    private Button agilityAddBtn;
    [SerializeField]
    private Button agilitySubBtn;
    [SerializeField]
    private Button intelligenceAddBtn;
    [SerializeField]
    private Button intelligenceSubBtn;
    [SerializeField]
    private Button applyStatChangesBtn;
    [SerializeField]
    private Button resetStatChangesBtn;

    public static UIManager instance;

    private CustomCursor currentCursor;
    private PlayerData playerData;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        currentCursor = CustomCursor.Default;
        Cursor.SetCursor(defaultCursor, new Vector2(20, 4), CursorMode.Auto);
    }

    public void SetCursorDefault()
    {
        if (currentCursor == CustomCursor.Default)
        {
            return;
        }

        Cursor.SetCursor(defaultCursor, new Vector2(20, 4), CursorMode.Auto);
        currentCursor = CustomCursor.Default;
    }

    public void SetCursorOnEnemy()
    {
        if (currentCursor == CustomCursor.OnEnemy)
        {
            return;
        }

        Cursor.SetCursor(onEnemyCursor, new Vector2(14, 2), CursorMode.Auto);
        currentCursor = CustomCursor.OnEnemy;
    }

    public void PlayerDataToggleButton(GameObject playerDataUI)
    {
        playerDataUI.SetActive(!playerDataUI.activeSelf);
    }

    public void SyncPlayerDataUI(UISyncOperation operation)
    {
        if ((operation & UISyncOperation.Health) == UISyncOperation.Health)
        {
            SyncHealthUI();
        }
        if ((operation & UISyncOperation.Resource) == UISyncOperation.Resource)
        {
            SyncResourceUI();
        }
        if ((operation & UISyncOperation.Experience) == UISyncOperation.Experience)
        {
            SyncExperienceUI();
        }
        if ((operation & UISyncOperation.Stats) == UISyncOperation.Stats)
        {
            SyncStatsUI();
        }
        if ((operation & UISyncOperation.Level) == UISyncOperation.Level)
        {
            SyncLevelUI();
        }
    }

    private void SyncHealthUI()
    {
        healthText.text = $"{playerData.GetHealthCurrent()}/{playerData.GetHealthMax()}";
        healthMaterial.SetFloat(Shader.PropertyToID("_FillAmount"), playerData.GetHealthRatio01());
    }

    private void SyncResourceUI()
    {
        resourceText.text = $"{playerData.GetResourceCurrent()}/{playerData.GetResourceMax()}";
        resourceMaterial.SetFloat(Shader.PropertyToID("_FillAmount"), playerData.GetResourceRatio01());
    }

    private void SyncExperienceUI()
    {
        experienceMaterial.SetFloat(Shader.PropertyToID("_FillAmount"), playerData.GetExperienceRatio01());
    }

    private void SyncStatsUI()
    {
        strengthText.text = playerData.GetStrength().ToString();
        constitutionText.text = playerData.GetConstitution().ToString();
        agilityText.text = playerData.GetAgility().ToString();
        intelligenceText.text = playerData.GetIntelligence().ToString();

        availablePointsText.text = "Remaining Points: " + playerData.GetAvailablePoints().ToString();
        

        SyncStatModifyButtonsState();
        SyncStatChangesButtonsState();
    }

    private void SyncLevelUI()
    {
        playerDescription.text = "Level " + playerData.GetLevel() + " " + playerData.PlayerClass.ToString();
    }

    private void InitStatButtons()
    {
        strengthAddBtn.onClick.AddListener(() => playerData.AddPointsStrength(1));
        strengthSubBtn.onClick.AddListener(() => playerData.AddPointsStrength(-1));
        constitutionAddBtn.onClick.AddListener(() => playerData.AddPointsConstitution(1));
        constitutionSubBtn.onClick.AddListener(() => playerData.AddPointsConstitution(-1));
        agilityAddBtn.onClick.AddListener(() => playerData.AddPointsAgility(1));
        agilitySubBtn.onClick.AddListener(() => playerData.AddPointsAgility(-1));
        intelligenceAddBtn.onClick.AddListener(() => playerData.AddPointsIntelligence(1));
        intelligenceSubBtn.onClick.AddListener(() => playerData.AddPointsIntelligence(-1));

        applyStatChangesBtn.onClick.AddListener(() => playerData.ApplyStatChanges());
        resetStatChangesBtn.onClick.AddListener(() => playerData.ResetStatChanges());
    }

    private void SyncStatModifyButtonsState()
    {
        strengthAddBtn.interactable = playerData.GetAvailablePoints() > 0;
        strengthSubBtn.interactable = playerData.GetCachedPointsStrength() > 0;
        constitutionAddBtn.interactable = playerData.GetAvailablePoints() > 0;
        constitutionSubBtn.interactable = playerData.GetCachedPointsConstitution() > 0;
        agilityAddBtn.interactable = playerData.GetAvailablePoints() > 0;
        agilitySubBtn.interactable = playerData.GetCachedPointsAgility() > 0;
        intelligenceAddBtn.interactable = playerData.GetAvailablePoints() > 0;
        intelligenceSubBtn.interactable = playerData.GetCachedPointsIntelligence() > 0;
    }

    private void SyncStatChangesButtonsState()
    {
        applyStatChangesBtn.interactable = playerData.HasPendingStatChanges();
        resetStatChangesBtn.interactable = playerData.HasPendingStatChanges();
    }

    public void SetCurrentPlayerData(PlayerData data)
    {
        playerData = data;
        InitStatButtons();
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public void SettingsButton()
    {
        
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void ActivateItemPopup(Vector3 itemWorldPosition, string itemTitle, string itemDescription)
    {
        itemDescriptionPanel.ActivateDescriptionPopup(itemWorldPosition, itemTitle, itemDescription);
    }

    public void DeactivateItemPopup()
    {
        itemDescriptionPanel.DeactivateDesctiptionPopup();
    }
}
