using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Top Level")]
    [SerializeField] private GameObject playButton;

    [Header("Second Level Buttons")]
    [SerializeField] private GameObject survivalButton;
    [SerializeField] private GameObject shootingButton;

    [Header("Panels")]
    [SerializeField] private GameObject survivalPanel;
    [SerializeField] private GameObject shootingPanel;

    [Header("Scene Names")]
    [SerializeField] private string desertSceneName = "DesertScene";
    [SerializeField] private string shootingSceneName = "ShootingRange";

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialPanel;

    [Header("Shop")]
    [SerializeField] private GameObject shopPanel;

    private bool playExpanded = false;
    private bool survivalPanelOpen = false;
    private bool shootingPanelOpen = false;
    private bool tutorialOpen = false;
    private bool shopOpen = false;

    void Awake()
    {
        survivalButton.SetActive(false);
        shootingButton.SetActive(false);

        survivalPanel.SetActive(false);
        shootingPanel.SetActive(false);
    }

    void Start()
    {
        playExpanded = false;
        survivalPanelOpen = false;
        shootingPanelOpen = false;
    }

    // =====================
    // Play
    // =====================
    public void OnPlayClicked()
    {
        playExpanded = !playExpanded;

        survivalButton.SetActive(playExpanded);
        shootingButton.SetActive(playExpanded);

        ClearUISelection();
        CloseAllPanels();

        tutorialOpen = false;
        tutorialPanel.SetActive(false);

        shopOpen = false;
        shopPanel.SetActive(false);
    }


    // =====================
    // Survival
    // =====================
    public void OnSurvivalClicked()
    {
        ClearUISelection();

        survivalPanelOpen = !survivalPanelOpen;

        survivalPanel.SetActive(survivalPanelOpen);

        if (survivalPanelOpen)
        {
            shootingPanelOpen = false;
            shootingPanel.SetActive(false);
        }

        ClearUISelection();
    }

    // =====================
    // Shooting
    // =====================
    public void OnShootingClicked()
    {
        ClearUISelection();

        shootingPanelOpen = !shootingPanelOpen;

        shootingPanel.SetActive(shootingPanelOpen);

        if (shootingPanelOpen)
        {
            survivalPanelOpen = false;
            survivalPanel.SetActive(false);
        }

        ClearUISelection();
    }

    // =====================
    // Start Buttons
    // =====================
    public void OnSurvivalStartClicked()
    {
        SceneManager.LoadScene(desertSceneName);
    }

    public void OnShootingStartClicked()
    {
        SceneManager.LoadScene(shootingSceneName);
    }

    // =====================
    // Helpers
    // =====================
    private void CloseAllPanels()
    {
        survivalPanelOpen = false;
        shootingPanelOpen = false;

        survivalPanel.SetActive(false);
        shootingPanel.SetActive(false);
    }

    private void ClearUISelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }


    // =====================
    // Tutorial Button
    // =====================
    public void OnTutorialClicked()
    {
        ClearUISelection();

        tutorialOpen = !tutorialOpen;

        if (tutorialOpen)
        {
            CollapsePlayMenu();

            CloseSecondaryPanelsExceptTutorial();
        }

        tutorialPanel.SetActive(tutorialOpen);

        ClearUISelection();
    }

    // =====================
    // Shop Button
    // =====================
    public void OnShopClicked()
    {
        ClearUISelection();

        shopOpen = !shopOpen;

        if (shopOpen)
        {
            CollapsePlayMenu();
            CloseSecondaryPanelsExceptShop();
        }


        shopPanel.SetActive(shopOpen);

        ClearUISelection();
    }

    // =====================
    // Exit Button
    // =====================
    public void OnExitClicked()
    {
        Application.Quit();
    }

    private void CloseSecondaryPanelsExceptShop()
    {
        // Tutorial
        tutorialOpen = false;
        tutorialPanel.SetActive(false);

        // Survival
        survivalPanelOpen = false;
        survivalPanel.SetActive(false);

        // Shooting
        shootingPanelOpen = false;
        shootingPanel.SetActive(false);
    }

    private void CloseSecondaryPanelsExceptTutorial()
    {
        // Shop
        shopOpen = false;
        shopPanel.SetActive(false);

        // Survival
        survivalPanelOpen = false;
        survivalPanel.SetActive(false);

        // Shooting
        shootingPanelOpen = false;
        shootingPanel.SetActive(false);
    }

    private void CollapsePlayMenu()
    {
        playExpanded = false;

        survivalButton.SetActive(false);
        shootingButton.SetActive(false);
    }
}
