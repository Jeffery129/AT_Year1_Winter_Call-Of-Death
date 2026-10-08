using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject menuCanvas;

    [Header("Mouse Look")]
    [SerializeField] private MouseMovement mouseMovement;

    [Header("Pause Panels")]
    [SerializeField] private GameObject pausePanel;    // Continue / Options / Exit

    [Header("Option Canvas")]
    [SerializeField] private GameObject optionCanvas;

    public static PauseManager Instance { get; private set; }
    public bool IsPaused => isPaused;
    private bool isPaused = false;

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
        ResumeGame();
        pausePanel.SetActive(true);
        optionCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;

        Time.timeScale = 0f;

        // Canvas
        mainCanvas.SetActive(false);
        menuCanvas.SetActive(true);

        pausePanel.SetActive(true);
        optionCanvas.SetActive(false);

        // Mouse look off
        if (mouseMovement != null)
            mouseMovement.allowLook = false;

        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.SetAllWeaponsShooting(false);
        }

        // Cursor unlock
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ResumeGame()
    {
        isPaused = false;

        Time.timeScale = 1f;

        // Canvas
        menuCanvas.SetActive(false);
        mainCanvas.SetActive(true);

        optionCanvas.SetActive(false);
        pausePanel.SetActive(true);

        // Mouse look on
        if (mouseMovement != null)
            mouseMovement.allowLook = true;

        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.SetAllWeaponsShooting(true);
        }

        // Cursor lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnContinueClicked()
    {
        ResumeGame();
    }

    public void OnOptionsClicked()
    {
        pausePanel.SetActive(false);
        optionCanvas.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnOptionsBackClicked()
    {
        optionCanvas.SetActive(false);
        menuCanvas.SetActive(true);

        pausePanel.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnExitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
 