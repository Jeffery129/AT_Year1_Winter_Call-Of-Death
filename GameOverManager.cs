using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject timeUpPanel;
    public GameObject deathPanel;

    [Header("TimeUp Panel UI")]
    public TMP_Text timeUpKillsText;
    public TMP_Text timeUpGoldText;

    [Header("Death Panel UI")]
    public TMP_Text deathKillsText;
    public TMP_Text deathGoldText;
    public TMP_Text survivalTimeText;

    [Header("Other UI to hide")]
    public GameObject[] otherCanvases;

    private float survivalTime = 0f;

    private AudioClip victoryClip;
    private AudioClip failureClip;
    public AudioSource BGMChannel;
    public AudioSource GameOverChannel;

    public bool gameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        victoryClip = Resources.Load<AudioClip>("VictoryClip");
        failureClip = Resources.Load<AudioClip>("FailureClip");
    }

    private void Update()
    {
        if (!gameOver)
        {
            survivalTime += Time.deltaTime;
        }
    }

    public void TriggerTimeUp()
    {
        if (gameOver) return;
        gameOver = true;

        Time.timeScale = 0f;

        BGMChannel.Stop();
        GameOverChannel.PlayOneShot(victoryClip);

        HideOtherCanvases();

        if (MainCanvasUI.Instance != null)
        {
            timeUpKillsText.text = $"{MainCanvasUI.Instance.kills}";
            timeUpGoldText.text = $"{MainCanvasUI.Instance.gold}";
        }

        PlayerData.AddCoin(MainCanvasUI.Instance.gold);

        timeUpPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.SetAllWeaponsShooting(false);
        }

    }

    public void TriggerDeath()
    {
        if (gameOver) return;
        gameOver = true;

        Time.timeScale = 0f;

        BGMChannel.Stop();
        GameOverChannel.PlayOneShot(failureClip);

        HideOtherCanvases();

        if (MainCanvasUI.Instance != null)
        {
            deathKillsText.text = $"{MainCanvasUI.Instance.kills}";
            deathGoldText.text = $"{MainCanvasUI.Instance.gold}";
        }

        int minutes = Mathf.FloorToInt(survivalTime / 60f);
        int seconds = Mathf.FloorToInt(survivalTime % 60f);
        survivalTimeText.text = $"{minutes}:{seconds}";

        PlayerData.AddCoin(MainCanvasUI.Instance.gold);

        deathPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.SetAllWeaponsShooting(false);
        }
    }

    private void HideOtherCanvases()
    {
        foreach (GameObject canva in otherCanvases)
        {
            canva.SetActive(false);
        }
    }

    public void OnGameOverExitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.SetAllWeaponsShooting(true);
        }
    }
}
