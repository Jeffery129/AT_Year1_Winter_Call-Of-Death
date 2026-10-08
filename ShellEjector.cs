using UnityEngine;

public class ShellEjector : MonoBehaviour
{
    [Header("References")]
    public Transform ejectionPort;
    public GameObject shellPrefab;

    [Header("Force")]
    public float ejectForce = 2.5f;
    public float upwardForce = 0.8f;
    public float randomSpread = 0.15f;

    [Header("Rotation")]
    public float randomTorque = 3f;

    public void Eject()
    {
        if (shellPrefab == null || ejectionPort == null) return;

        GameObject shell = Instantiate(
            shellPrefab,
            ejectionPort.position,
            Random.rotation
        );

        Rigidbody rb = shell.GetComponent<Rigidbody>();

        Vector3 dir =
            ejectionPort.forward +
            ejectionPort.right * Random.Range(-randomSpread, randomSpread) +
            ejectionPort.up * upwardForce;

        rb.AddForce(dir.normalized * ejectForce, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * randomTorque, ForceMode.Impulse);

        Destroy(shell, Random.Range(2f, 3f));
    }
}
