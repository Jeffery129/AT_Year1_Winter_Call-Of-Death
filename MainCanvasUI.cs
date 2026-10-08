using UnityEngine;
using TMPro;

public class MainCanvasUI : MonoBehaviour
{
    public static MainCanvasUI Instance { get; private set; }


    [Header("UI References")]
    public TMP_Text goldText;
    public TMP_Text timerText;
    public TMP_Text killCountText;

    [Header("Game Stats")]
    public int gold = 0;
    public int kills = 0;
    public float levelTime = 180f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        UpdateGoldText();
        UpdateKillText();
        UpdateTimerText();
    }

    private void Update()
    {
        if (levelTime > 0f)
        {
            levelTime -= Time.deltaTime;
            if (levelTime < 0f) levelTime = 0f;
            UpdateTimerText();
        }

        if (levelTime <= 0f)
        {
            levelTime = 0f;
            OnTimeUp();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    private void UpdateGoldText()
    {
        goldText.text = $"{gold}";
    }

    public void AddKill(int amount = 1)
    {
        kills += amount;
        UpdateKillText();
    }

    private void UpdateKillText()
    {
        killCountText.text = $"{kills}";
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(levelTime / 60f);
        int seconds = Mathf.FloorToInt(levelTime % 60f);
        timerText.text = $"{minutes}:{seconds}";
    }

    private void OnTimeUp()
    {
        GameOverManager.Instance.TriggerTimeUp();
    }
}
