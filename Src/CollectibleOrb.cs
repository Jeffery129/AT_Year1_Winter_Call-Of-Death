using UnityEngine;

public class CollectibleOrb : MonoBehaviour
{
    public enum OrbType { Health, Coin }
    public OrbType orbType;

    public float attractDistance = 5f;
    public float moveSpeed = 10f;

    [SerializeField] int plusHP;
    [SerializeField] int plusCoin;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        plusHP = Random.Range(5, 15);
        plusCoin = Random.Range(1, 10);
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attractDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (!collider.CompareTag("Player")) return;

        if (orbType == OrbType.Health)
        {
            collider.GetComponent<PlayerStatus>().Heal(plusHP);
        }
        else if (orbType == OrbType.Coin)
        {
            if (MainCanvasUI.Instance != null)
                MainCanvasUI.Instance.AddGold(plusCoin);
        }

        Destroy(gameObject);
    }
}