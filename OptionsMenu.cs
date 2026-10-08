using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Mouse")]
    [SerializeField] private MouseMovement mouseMovement;
    [SerializeField] private Slider sensitivitySlider;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    [Header("UI Text")]
    [SerializeField] private TMP_Text sensitivityValueText;
    [SerializeField] private TMP_Text volumeValueText;

    private const string SENSITIVITY_KEY = "MouseSensitivity";
    private const string VOLUME_KEY = "MasterVolume";

    private void Start()
    {
        // Load sensitivity
        float savedSensitivity = PlayerPrefs.GetFloat(SENSITIVITY_KEY, mouseMovement.mouseSensitivity);
        mouseMovement.SetSensitivity(savedSensitivity);
        sensitivitySlider.value = savedSensitivity;
        UpdateSensitivityText(savedSensitivity);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        // Load volume
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        volumeSlider.value = savedVolume;
        ApplyVolume(savedVolume);
        UpdateVolumeText(savedVolume);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void OnSensitivityChanged(float value)
    {
        mouseMovement.SetSensitivity(value);
        UpdateSensitivityText(value);
        PlayerPrefs.SetFloat(SENSITIVITY_KEY, value);
    }

    private void OnVolumeChanged(float value)
    {
        ApplyVolume(value);
        UpdateVolumeText(value);
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
    }

    private void UpdateSensitivityText(float value)
    {
        sensitivityValueText.text = Mathf.RoundToInt(value).ToString();
    }

    private void UpdateVolumeText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        volumeValueText.text = $"{percent}";
    }

    private void ApplyVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
    }
}
