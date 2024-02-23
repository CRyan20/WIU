using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioMixer audioMixer; // Reference to the Audio Mixer

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
            OnVolumeChanged(); // Call the method to apply the initial volume
        }
    }

    // Called when the slider value changes
    public void OnVolumeChanged()
    {
        float volume = volumeSlider.value;

        // Save the volume level to PlayerPrefs for future sessions
        PlayerPrefs.SetFloat("Volume", volume);

        // Set the exposed parameter on the Audio Mixer
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
}
