using TMPro;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    // =====================
    // UI Coin
    // =====================
    [Header("Coin UI")]
    [SerializeField] private TMP_Text coinText;

    // =====================
    // UI  Level Text
    // =====================
    [Header("Level Text (TMP)")]
    [SerializeField] private TMP_Text attackLevelText;
    [SerializeField] private TMP_Text healthLevelText;
    [SerializeField] private TMP_Text speedLevelText;

    [Header("Cost Text")]
    [SerializeField] private TMP_Text attackCostText;
    [SerializeField] private TMP_Text healthCostText;
    [SerializeField] private TMP_Text speedCostText;

    // =====================
    // Cost Inspector
    // =====================
    [Header("Base Cost")]
    public int attackBaseCost = 10;
    public int healthBaseCost = 10;
    public int speedBaseCost = 10;

    [Header("Cost Increase Per Level")]
    public int attackCostIncrease = 5;
    public int healthCostIncrease = 5;
    public int speedCostIncrease = 5;

    // =====================
    // Unity Lifecycle
    // =====================

    private void OnEnable()
    {
        UpdateAllUI();
    }

    // =====================
    // Buy Methods
    // =====================

    public void BuyAttack()
    {
        int cost = GetAttackCost();

        if (!PlayerData.SpendCoin(cost))
            return;

        PlayerData.attackLevel++;
        UpdateAllUI();
    }

    public void BuyHealth()
    {
        int cost = GetHealthCost();

        if (!PlayerData.SpendCoin(cost))
            return;

        PlayerData.healthLevel++;
        UpdateAllUI();
    }

    public void BuySpeed()
    {
        int cost = GetSpeedCost();

        if (!PlayerData.SpendCoin(cost))
            return;

        PlayerData.speedLevel++;
        UpdateAllUI();
    }

    // =====================
    // Cost Calculation
    // =====================
    private int GetAttackCost()
    {
        return attackBaseCost + PlayerData.attackLevel * attackCostIncrease;
    }

    private int GetHealthCost()
    {
        return healthBaseCost + PlayerData.healthLevel * healthCostIncrease;
    }

    private int GetSpeedCost()
    {
        return speedBaseCost + PlayerData.speedLevel * speedCostIncrease;
    }

    // =====================
    // UI Update
    // =====================

    private void UpdateAllUI()
    {
        coinText.text = PlayerData.Coin.ToString();

        attackLevelText.text = PlayerData.attackLevel.ToString();
        healthLevelText.text = PlayerData.healthLevel.ToString();
        speedLevelText.text = PlayerData.speedLevel.ToString();

        // Cost
        attackCostText.text = GetAttackCost().ToString();
        healthCostText.text = GetHealthCost().ToString();
        speedCostText.text = GetSpeedCost().ToString();
    }
}
