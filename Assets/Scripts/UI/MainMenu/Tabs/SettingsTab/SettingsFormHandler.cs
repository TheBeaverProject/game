using System;
using System.Collections.Generic;
using Firebase;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.MainMenu.Tabs.SettingsTab
{
    public class SettingsFormHandler : MonoBehaviour
    {
        #region Serialized Fields
        
        public TMP_Dropdown ResDropdownMenu;
        public TMP_Dropdown FullScreenDropdownMenu;
        public Slider FOVSlider;
        public TextMeshProUGUI FOVText;
        public TMP_Dropdown QualityDropdown;

        public GameObject EscapeMenu;

        #endregion

        private void Start()
        {
            InitFSDropdown();
            InitResDropdown();
            
            selectedRes = Screen.currentResolution;
            selectedFSMode = Screen.fullScreenMode;
            
            FOVSlider.value = PlayerPrefs.HasKey(PlayerPrefKeys.FOV) ? PlayerPrefs.GetInt(PlayerPrefKeys.FOV) : 70;

            InitQualityDropdown();
        }

        #region Screen

        private Resolution selectedRes;
        private FullScreenMode selectedFSMode;

        #region FullScreen

        private readonly Dictionary<string, FullScreenMode> fsModesDic = new Dictionary<string, FullScreenMode>
        {
            {"FullScreen", FullScreenMode.ExclusiveFullScreen},
            {"BorderLess", FullScreenMode.FullScreenWindow},
            {"Maximized", FullScreenMode.MaximizedWindow},
            {"Windowed", FullScreenMode.Windowed}
        };
        
        private void InitFSDropdown()
        {
            List<string> resOptions = new List<string>();

            foreach (var fsMode in fsModesDic)
            {
                resOptions.Add(fsMode.Key);
            }
            
            FullScreenDropdownMenu.AddOptions(resOptions);
            
            int i = 0;
            foreach (var optionData in FullScreenDropdownMenu.options)
            {


                if (fsModesDic[optionData.text] == Screen.fullScreenMode)
                    FullScreenDropdownMenu.value = i;
                i++;
            }
        }

        public void HandleFSDropdown(int value)
        {
            var option = FullScreenDropdownMenu.options[FullScreenDropdownMenu.value];
            selectedFSMode = fsModesDic[option.text];
        }

        #endregion

        #region Resolution

        private Dictionary<string, Resolution> resDictionary = new Dictionary<string, Resolution>();

        private void InitResDropdown()
        {
            Resolution[] resolutions = Screen.resolutions;
            List<string> resOptions = new List<string>();
            
            foreach (var res in resolutions)
            {
                resOptions.Add($"{res.width}x{res.height}@{res.refreshRate}");
                resDictionary.Add($"{res.width}x{res.height}@{res.refreshRate}", res);
            }
            
            ResDropdownMenu.AddOptions(resOptions);
            var currRes = Screen.currentResolution;

            int i = 0;
            foreach (var optionData in ResDropdownMenu.options)
            {
                if (optionData.text == $"{currRes.width}x{currRes.height}@{currRes.refreshRate}")
                    ResDropdownMenu.value = i;
                i++;
            }
        }
        
        public void HandleResDropdown(int value)
        {
            var option = ResDropdownMenu.options[ResDropdownMenu.value];
            selectedRes = resDictionary[option.text];
        }

        #endregion

        public void HandleApplyScreenButton()
        {
            Debug.Log($"Selected Resolution: {selectedRes.width}x{selectedRes.height}@{selectedRes.refreshRate}\nFullscreen mode: {selectedFSMode.ToString()}");
            Screen.SetResolution(selectedRes.width, selectedRes.height, selectedFSMode, selectedRes.refreshRate);
        }

        #endregion
        

        #region Game

        #region FOV

        public void FOVSliderUpdate()
        {
            FOVText.text = Convert.ToString((int) FOVSlider.value);
        }

        #endregion

        #region Quality

        private void InitQualityDropdown()
        {
            string[] names = QualitySettings.names;
            for (int i = 0; i < names.Length; i++)
            {
                QualityDropdown.options.Add(new TMP_Dropdown.OptionData(names[i]));
            }

            QualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        #endregion

        public void GameSettingsSave()
        {
            PlayerPrefs.SetInt(PlayerPrefKeys.FOV, (int) FOVSlider.value);

            QualitySettings.SetQualityLevel(QualityDropdown.value, true);
            
            PlayerPrefs.Save();

            if (EscapeMenu != null)
            {
                EscapeMenu.GetComponent<Canvas>().worldCamera.fieldOfView = FOVSlider.value;
            }
        }

        #endregion
        

        #region Account

        public void OnLogoutClick()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LeaveRoom();
            }
            AuthHandler.LogOut();
            SceneManager.LoadScene("Authentication");
        }

        #endregion
    }
}