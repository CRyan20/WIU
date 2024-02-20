using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionDropdown : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;

    // Start is called before the first frame update
    void Start()
    {
        // Populate the TMP dropdown with available resolutions
        PopulateResolutionDropdown();
    }

    // Function to populate the TMP dropdown with available resolutions
    void PopulateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        // Get the available screen resolutions
        Resolution[] resolutions = Screen.resolutions;

        // Create a list of string representations for each resolution
        List<string> resolutionOptions = new List<string>();
        foreach (Resolution resolution in resolutions)
        {
            resolutionOptions.Add(resolution.width + "x" + resolution.height);
        }

        // Add the resolution options to the TMP dropdown
        resolutionDropdown.AddOptions(resolutionOptions);

        // Set the current resolution as the default option
        resolutionDropdown.value = GetCurrentResolutionIndex();
        resolutionDropdown.RefreshShownValue();
    }

    // Function to get the index of the current screen resolution in the TMP dropdown
    int GetCurrentResolutionIndex()
    {
        Resolution currentResolution = Screen.currentResolution;

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == currentResolution.width &&
                Screen.resolutions[i].height == currentResolution.height)
            {
                return i;
            }
        }

        // If the current resolution is not found, return 0 as a default
        return 0;
    }

    // Function to handle resolution change when the TMP dropdown value changes
    public void OnResolutionChanged()
    {
        // Get the selected resolution from the TMP dropdown
        string selectedResolution = resolutionDropdown.options[resolutionDropdown.value].text;

        // Split the resolution string into width and height
        string[] resolutionParts = selectedResolution.Split('x');
        int width = int.Parse(resolutionParts[0]);
        int height = int.Parse(resolutionParts[1]);

        // Change the screen resolution
        Screen.SetResolution(width, height, Screen.fullScreen);

        Debug.Log("Resolution changed to: " + selectedResolution);
    }
}
