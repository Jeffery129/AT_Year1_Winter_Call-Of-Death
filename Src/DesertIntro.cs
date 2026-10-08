using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DesertIntro : MonoBehaviour
{
    [Header("Canvas References")]
    public GameObject introCanvas;
    public GameObject mainCanvas;


    [Header("Player Control")]
    public MonoBehaviour[] movementScripts;
    public MonoBehaviour[] mouseLookScripts;

    [Header("Intro Settings")]
    public float introDuration = 3f;

    void Awake()
    {
        introCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(IntroSequence());
    }


    private IEnumerator IntroSequence()
    {
        introCanvas.SetActive(true);
        mainCanvas.SetActive(false);

        SetPlayerControl(false);

        yield return new WaitForSeconds(introDuration);

        introCanvas.SetActive(false);
        mainCanvas.SetActive(true);

        SetPlayerControl(true);
    }


    private void SetPlayerControl(bool enabled)
    {
        foreach (var script in movementScripts)
            script.enabled = enabled;


        foreach (var script in mouseLookScripts)
            script.enabled = enabled;
    }
}
