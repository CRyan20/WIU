    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetVolume(float sliderValue)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
}

//        public Slider volumeSlider;
//        public AudioMixer audioMixer; // Reference to the Audio Mixer
//    void Start()
//    {
//        if (volumeSlider != null)
//        {
//            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);

//            // Connect the OnVolumeChanged method to the slider's OnValueChanged event
//            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

//            OnVolumeChanged(volumeSlider.value); // Call the method to apply the initial volume
//        }
//    }

//    // Corrected signature for the OnVolumeChanged method
//    public void OnVolumeChanged(float value)
//    {
//        Debug.Log("Volume Slider Value: " + value);

//        float volume = value;

//        // Save the volume level to PlayerPrefs for future sessions
//        PlayerPrefs.SetFloat("Volume", volume);

//        // Set the exposed parameter on the Audio Mixer
//        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
//    }
//}
