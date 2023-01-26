using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer masterAudioMixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    Resolution[] resolutions;
    List<string> resOptions = new List<string>();
    private string option;
    private int currentResolutionIndex = 0;

    [SerializeField] TMP_Dropdown resolutionDropdown;

    private int qualityLevel;
    [SerializeField] TMP_Dropdown qualityDropdown;

    [SerializeField] Toggle fullscreenToggle;

    void Start()
    {
        qualityLevel = QualitySettings.GetQualityLevel();
        qualityDropdown.value = qualityLevel;
        qualityDropdown.RefreshShownValue();

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        for(int i = 0; i < resolutions.Length; i++)
        {
            option = resolutions[i].width + "x" + resolutions[i].height;
            resOptions.Add(option);

            if(resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void SetQuality(int qualityIndex) //should save automatically
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt("QualityIndex", QualitySettings.GetQualityLevel());
        PlayerPrefs.Save();
    }

    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, Screen.fullScreen);

        PlayerPrefs.SetInt("ScreenWidth", resolutions[resolutionIndex].width);
        PlayerPrefs.SetInt("ScreenHeight", resolutions[resolutionIndex].height);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("isFullscreen", (isFullscreen ? 1 : 0));
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume)
    {
        masterAudioMixer.SetFloat("MasterVolume", volume);

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        masterAudioMixer.SetFloat("MusicVolume", volume);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        masterAudioMixer.SetFloat("SFXVolume", volume);

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    public void LoadPrefs()
    {
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityIndex"));

        Screen.SetResolution(PlayerPrefs.GetInt("ScreenWidth"), PlayerPrefs.GetInt("ScreenHeight"), (PlayerPrefs.GetInt("isFullscreen") != 0));

        masterAudioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MusicVolume"));
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");

        masterAudioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");

        masterAudioMixer.SetFloat("SFXVolume", PlayerPrefs.GetFloat("SFXVolume"));
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        //Debug.Log(PlayerPrefs.GetInt("QualityIndex"));
        //Debug.Log(PlayerPrefs.GetInt("ScreenWidth").ToString() + PlayerPrefs.GetInt("ScreenHeight").ToString() + (PlayerPrefs.GetInt("isFullscreen") != 0));
        //Debug.Log(PlayerPrefs.GetFloat("MasterVolume"));
        //Debug.Log(PlayerPrefs.GetFloat("MusicVolume"));
        //Debug.Log(PlayerPrefs.GetFloat("SFXVolume"));
    }

    float GetMasterLevel()
    {
        float value;
        bool result = masterAudioMixer.GetFloat("MasterVolume", out value);
        if(result)
            return value;
        else
            return 0f;
    }

    float GetMusicLevel()
    {
        float value;
        bool result = masterAudioMixer.GetFloat("MusicVolume", out value);
        if(result)
            return value;
        else
            return 0f;
    }

    float GetSFXLevel()
    {
        float value;
        bool result = masterAudioMixer.GetFloat("SFXVolume", out value);
        if(result)
            return value;
        else
            return 0f;
    }
}
