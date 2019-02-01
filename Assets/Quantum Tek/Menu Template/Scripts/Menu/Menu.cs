using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace QuantumTek.MenuSystem
{
    /// <summary> The type of a menu. </summary>
    [System.Serializable]
    public enum MenuType
    {
        /// <summary> The menu has many windows that can be open at the same time, and open/closed at any time. </summary>
        Solitary,
        /// <summary> The menu has many windows, but only one can be open at a time, although they can be open/closed by a tab at any time. </summary>
        Tab
    }

    /// <summary> The way to align the tabs at the top of the menu on runtime. </summary>
    [System.Serializable]
    public enum TabAlign
    {
        /// <summary> The tabs will be aligned to the center of the menu. </summary>
        Center,
        /// <summary> The tabs will be aligned to the left of the menu. </summary>
        Left,
        /// <summary> The tabs will be aligned to the right of the menu. </summary>
        Right
    }

    /// <summary> The type of the load animation. </summary>
    [System.Serializable]
    public enum LoadAnimationType
    {
        /// <summary> Changes the image fill amount based on how much the scene is loaded. </summary>
        FillAmount,
        /// <summary> Changes the image width based on how much the scene is loaded. </summary>
        Width,
    }

    /// <summary> Stores a name and a UI element to save the value of. </summary>
    [System.Serializable]
    public struct UISetting
    {
        public string Name;
        public Slider UISlider;
        public Toggle UIToggle;
        public Text UIText;
        public TextMeshProUGUI UITextMeshProUGUI;
        public Dropdown UIDropdown;
        public TMP_Dropdown UITMP_Dropdown;
        public OptionsList UIOptionsList;
    }

    /// <summary> An instance of a Menu. Handles everything needed in a menu. </summary>
    [AddComponentMenu("Menu System/Menu")]
    [DisallowMultipleComponent]
    public class Menu : MonoBehaviour
    {
        #region Object References
        protected List<Window> windows = new List<Window>();
        protected List<Tab> tabs = new List<Tab>();
        protected List<Window> activeWindows = new List<Window>();
        protected Tab activeTab = null;
        [Header("Object References")]
        [Tooltip("The background image in this menu.")]
        [SerializeField] protected GameObject backgroundImage = null;
        [Tooltip("The loading window in this menu.")]
        [SerializeField] protected Window loadingWindow = null;
        [Tooltip("The RectTransform loading image's mask in this menu.")]
        [SerializeField] protected RectTransform loadingImageMask = null;
        [Tooltip("The loading image in this menu.")]
        [SerializeField] protected Image loadingImage = null;
        [Tooltip("The audio source in this menu.")]
        [SerializeField] protected AudioSource audioSource = null;
        [Tooltip("Whether or not to use audio upon button action in this menu.")]
        [SerializeField] protected bool useAudio = true;
        [Tooltip("The AudioClip played when a button in this menu is clicked.")]
        [SerializeField] protected AudioClip buttonClip = null;
        #endregion

        #region Variables
        [Header("Variables")]
        [Tooltip("The type of this menu.")]
        [SerializeField] protected MenuType type = MenuType.Solitary;
        [Tooltip("The tab alignment of this menu, if there are tabs.")]
        [SerializeField] protected TabAlign tabAlign = TabAlign.Center;
        [Tooltip("The tab offset on the y axis.")]
        [SerializeField] protected float tabOffset = -6f;
        [Tooltip("The load animation type of this menu.")]
        [SerializeField] protected LoadAnimationType loadAnimationType = LoadAnimationType.FillAmount;
        [Tooltip("The list of settings to store.")]
        [SerializeField] protected List<UISetting> uiSettings = new List<UISetting>();
        #endregion

        protected void Awake()
        {
            // Finds the window componenents in children and adds them to the list of windows.
            Window[] childWindows = GetComponentsInChildren<Window>(true); // Patched in 1.0.1
            int childWindowCount = childWindows.Length;
            for (int i = 0; i < childWindowCount; ++i)
            { windows.Add(childWindows[i]); }

            // Sets up all the windows and tabs based on their active state.
            int windowCount = windows.Count;
            Window window = null;
            for (int i = 0; i < windowCount; ++i)
            {
                window = windows[i];
                window.Setup();
                if (window.Active && type == MenuType.Solitary) activeWindows.Add(window);
                else if (window.Active && type == MenuType.Tab && activeWindows.Count == 0) { activeWindows.Add(window); }
                if (window.Active && !activeTab) activeTab = window.Tab;
                if (window.Tab) { tabs.Add(window.Tab); window.Tab.Setup(window.Active); }
            }

            // Load settings.
            int settingsCount = uiSettings.Count;
            UISetting setting = new UISetting();
            for (int i = 0; i < settingsCount; ++i)
            {
                setting = uiSettings[i];
                LoadSetting(setting.Name); uiSettings[i] = setting;
            }

            // Align the tabs.
            if (type != MenuType.Tab) return;
            float tabsWidth = 0;
            int tabCount = tabs.Count;
            RectTransform tabTransform = null;
            // Find the total width.
            for (int i = 0; i < tabCount; ++i)
            { tabTransform = tabs[i].GetComponent<RectTransform>(); if (!tabTransform) continue; tabsWidth += tabTransform.rect.width; }
            // Align the tabs using the total width.
            float currentTabWidth = 0;
            for (int i = 0; i < tabCount; ++i)
            {
                tabTransform = tabs[i].GetComponent<RectTransform>();
                if (!tabTransform) continue;
                float tabWidth = tabTransform.rect.width;
                float tabHeight = tabTransform.rect.height;

                if (tabAlign == TabAlign.Center)
                {
                    tabTransform.anchorMin = new Vector2(0.5f, 1);
                    tabTransform.anchorMax = new Vector2(0.5f, 1);
                    tabTransform.pivot = new Vector2(0.5f, 1);
                    tabTransform.anchoredPosition = new Vector2(-tabsWidth / 2 / tabs.Count + currentTabWidth, tabHeight + tabOffset);
                    currentTabWidth += tabWidth;
                }
                else if (tabAlign == TabAlign.Left)
                {
                    tabTransform.anchorMin = new Vector2(0, 1);
                    tabTransform.anchorMax = new Vector2(0, 1);
                    tabTransform.pivot = new Vector2(0, 1);
                    tabTransform.anchoredPosition = new Vector2(currentTabWidth, tabHeight + tabOffset);
                    currentTabWidth += tabWidth;
                }
                else if (tabAlign == TabAlign.Right)
                {
                    currentTabWidth += tabWidth;
                    tabTransform.anchorMin = new Vector2(1, 1);
                    tabTransform.anchorMax = new Vector2(1, 1);
                    tabTransform.pivot = new Vector2(1, 1);
                    tabTransform.anchoredPosition = new Vector2(-tabsWidth + currentTabWidth, tabHeight + tabOffset);
                }
            }
        }

        /// <summary> Triggers the button click sound to be played. </summary>
        public void PlayAudio()
        { if (useAudio && audioSource && buttonClip) audioSource.PlayOneShot(buttonClip); }

        protected int FindSettingIndex(string pName)
        {
            if (pName.Length == 0) return - 1;
            int settingsCount = uiSettings.Count;
            for (int i = 0; i < settingsCount; ++i)
            { if (uiSettings[i].Name == pName) return i; }
            return -1;
        }

        /// <summary> Saves the given setting from certain UI elements. </summary>
        /// <param name="settingName">The name to save under.</param>
        public void SaveSetting(string settingName)
        {
            int settingIndex = FindSettingIndex(settingName);
            if (settingIndex < 0) return;
            UISetting setting = uiSettings[settingIndex];
            if (setting.UISlider) PlayerPrefs.SetFloat(settingName, setting.UISlider.value);
            if (setting.UIToggle) PlayerPrefs.SetInt(settingName, setting.UIToggle.isOn ? 1 : 0);
            if (setting.UIText) PlayerPrefs.SetString(settingName, setting.UIText.text);
            if (setting.UITextMeshProUGUI) PlayerPrefs.SetString(settingName, setting.UITextMeshProUGUI.text);
            if (setting.UIDropdown) PlayerPrefs.SetInt(settingName, setting.UIDropdown.value);
            if (setting.UITMP_Dropdown) PlayerPrefs.SetInt(settingName, setting.UITMP_Dropdown.value);
            if (setting.UIOptionsList) PlayerPrefs.SetInt(settingName, setting.UIOptionsList.CurrentOption);
            PlayerPrefs.Save();
        }

        protected void LoadSetting(string settingName)
        {
            int settingIndex = FindSettingIndex(settingName);
            if (settingIndex < 0) return;
            UISetting setting = uiSettings[settingIndex];
            if (setting.UISlider) setting.UISlider.value = PlayerPrefs.GetFloat(settingName);
            if (setting.UIToggle) setting.UIToggle.isOn = PlayerPrefs.GetInt(settingName) == 1 ? true : false;
            if (setting.UIText) setting.UIText.text = PlayerPrefs.GetString(settingName);
            if (setting.UITextMeshProUGUI) setting.UITextMeshProUGUI.text = PlayerPrefs.GetString(settingName);
            if (setting.UIDropdown) setting.UIDropdown.value = PlayerPrefs.GetInt(settingName);
            if (setting.UITMP_Dropdown) setting.UITMP_Dropdown.value = PlayerPrefs.GetInt(settingName);
            if (setting.UIOptionsList) setting.UIOptionsList.CurrentOption = PlayerPrefs.GetInt(settingName);
        }
        
        /// <summary> Returns the value of a setting. </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <returns></returns>
        public float GetFloatSetting(string settingName)
        {
            int settingIndex = FindSettingIndex(settingName);
            if (settingIndex < 0) return 0f;
            UISetting setting = uiSettings[settingIndex];
            if (setting.UISlider) return PlayerPrefs.GetFloat(settingName);
            return 0f;
        }
        /// <summary> Returns the value of a setting. </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <returns></returns>
        public int GetIntSetting(string settingName)
        {
            int settingIndex = FindSettingIndex(settingName);
            if (settingIndex < 0) return 0;
            UISetting setting = uiSettings[settingIndex];
            if (setting.UIToggle) return PlayerPrefs.GetInt(settingName);
            if (setting.UIDropdown) return PlayerPrefs.GetInt(settingName);
            if (setting.UITMP_Dropdown) return PlayerPrefs.GetInt(settingName);
            if (setting.UIOptionsList) return PlayerPrefs.GetInt(settingName);
            return 0;
        }
        /// <summary> Returns the value of a setting. </summary>
        /// <param name="settingName">The name of the setting.</param>
        /// <returns></returns>
        public string GetStringSetting(string settingName)
        {
            int settingIndex = FindSettingIndex(settingName);
            if (settingIndex < 0) return "";
            UISetting setting = uiSettings[settingIndex];
            if (setting.UIText) return PlayerPrefs.GetString(settingName);
            if (setting.UITextMeshProUGUI) return PlayerPrefs.GetString(settingName);
            return "";
        }

        /// <summary> Opens the menu and its currently active window(s). </summary>
        public void OpenMenu()
        {
            if (backgroundImage) backgroundImage.SetActive(true);

            if (type == MenuType.Solitary)
            {
                int windowCount = activeWindows.Count;
                for (int i = 0; i < windowCount; ++i)
                { activeWindows[i].gameObject.SetActive(true); }
            }
            else
            {
                int windowCount = windows.Count;
                for (int i = 0; i < windowCount; ++i)
                { windows[i].gameObject.SetActive(true); }
            }
        }
        /// <summary> Closes the menu and its currently active window(s). </summary>
        public void CloseMenu()
        {
            if (backgroundImage) backgroundImage.SetActive(false);
            if (type == MenuType.Solitary)
            {
                int windowCount = activeWindows.Count;
                for (int i = 0; i < windowCount; ++i)
                { activeWindows[i].gameObject.SetActive(false); }
            }
            else
            {
                int windowCount = windows.Count;
                for (int i = 0; i < windowCount; ++i)
                { windows[i].gameObject.SetActive(false); }
            }
        }
        /// <summary> Hides the menu and its currently active window(s). </summary>
        public void HideMenu()
        {
            if (type == MenuType.Solitary)
            {
                int windowCount = activeWindows.Count;
                for (int i = 0; i < windowCount; ++i)
                { activeWindows[i].gameObject.SetActive(false); }
            }
            else
            {
                int windowCount = windows.Count;
                for (int i = 0; i < windowCount; ++i)
                { if (windows[i].Tab) windows[i].gameObject.SetActive(false); }
            }
        }

        /// <summary> Opens a window if it is in this menu. </summary>
        /// <param name="pWindow">A reference to the Window object.</param>
        public void OpenWindow(Window pWindow)
        {
            // Return if there is no given window, the window is not part of the menu, or the window is already open.
            if (!pWindow || (type == MenuType.Solitary && activeWindows.Contains(pWindow)) || (type == MenuType.Tab && (activeWindows.Count == 0 || activeWindows[0] == pWindow) && pWindow.Tab) || pWindow.Active) return;
            // Open the window and add it to the list of active windows.
            pWindow.Open();
            if (type == MenuType.Tab) {
                if (pWindow.Tab)
                {
                    activeTab = pWindow.Tab;
                    if (activeWindows[0]) activeWindows[0].Close();
                    activeWindows.Clear();
                    if (pWindow.Tab) pWindow.Tab.Active = true;
                    activeWindows.Add(pWindow);
                }
            }
            else activeWindows.Add(pWindow);
            if (useAudio && audioSource && buttonClip) audioSource.PlayOneShot(buttonClip);
        }
        /// <summary> Opens a window if it is in this menu. </summary>
        /// <param name="pName">The name of the window.</param>
        public void OpenWindow(string pName)
        {
            // Search for the window in the list, if it isn't there, return.
            Window window = null;
            int windowCount = windows.Count;
            for (int i = 0; i < windowCount; ++i)
            { if (windows[i].Name == pName) { window = windows[i]; break; } }
            if (!window || (type == MenuType.Solitary && activeWindows.Contains(window)) || (type == MenuType.Tab && (activeWindows.Count == 0 || activeWindows[0] == window) && window.Tab) || window.Active) return;
            // Open the window and add it to the list of active windows.
            window.Open();
            if (type == MenuType.Tab)
            {
                if (window.Tab)
                {
                    activeTab = window.Tab;
                    if (activeWindows[0]) activeWindows[0].Close();
                    activeWindows.Clear();
                    if (window.Tab) window.Tab.Active = true;
                    activeWindows.Add(window);
                }
            }
            else activeWindows.Add(window);
            if (useAudio && audioSource && buttonClip) audioSource.PlayOneShot(buttonClip);
        }
        /// <summary> Closes a window if it is in this menu. </summary>
        /// <param name="pWindow">A reference to the Window object.</param>
        public void CloseWindow(Window pWindow)
        {
            // Return if there is no given window, the window is not part of the menu, or the window is already closed.
            if (!pWindow || !windows.Contains(pWindow) || !pWindow.Active) return;
            // Close the window and remove it from the list of active windows.
            pWindow.Close();
            if (type == MenuType.Solitary)
            {
                activeWindows.Remove(pWindow);
                if (useAudio && audioSource && buttonClip) audioSource.PlayOneShot(buttonClip);
            }
        }
        /// <summary> Closes a window if it is in this menu. </summary>
        /// <param name="pName">The name of the window.</param>
        public void CloseWindow(string pName)
        {
            // Search for the window in the list, if it isn't there, return.
            Window window = null;
            int windowCount = windows.Count;
            for (int i = 0; i < windowCount; ++i)
            { if (windows[i].Name == pName) { window = windows[i]; break; } }
            if (!window || !window.Active) return;
            // Close the window and remove it from the list of active windows.
            window.Close();
            if (type == MenuType.Solitary)
            {
                activeWindows.Remove(window);
                if (useAudio && audioSource && buttonClip) audioSource.PlayOneShot(buttonClip);
            }
        }

        /// <summary> Loads a scene while showing a loading screen. </summary>
        /// <param name="pBuildIndex">The build index of the scene.</param>
        public void LoadScene(int pBuildIndex)
        {
            if (pBuildIndex < 0 || pBuildIndex >= SceneManager.sceneCountInBuildSettings) { Debug.LogWarning("Tried to load the scene with build index " + pBuildIndex + ", but the build index was out of range."); return; }
            StartCoroutine(LoadSceneAsync(pBuildIndex));
            if (useAudio && audioSource && buttonClip) audioSource.PlayOneShot(buttonClip);
        }
        /// <summary> Loads a scene while showing a loading screen. </summary>
        /// <param name="pSceneName">The name of the scene.</param>
        public void LoadScene(string pSceneName)
        {
            if (pSceneName.Length == 0) { Debug.LogWarning("Tried to load a scene with no name."); return; }
            if (SceneManager.GetSceneByName(pSceneName) == null) { Debug.LogWarning("Tried to load the scene called " + pSceneName + ", but it doesn't exist in the build settings."); }
            StartCoroutine(LoadSceneAsync(pSceneName));
            if (useAudio && audioSource && buttonClip) audioSource.PlayOneShot(buttonClip);
        }

        protected IEnumerator LoadSceneAsync(int pBuildIndex)
        {
            // Start loading the scene.
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(pBuildIndex);

            // Show loading graphic.
            if (loadingWindow && !loadingWindow.Active) { loadingWindow.gameObject.SetActive(true); loadingWindow.Open(); }

            // Update loading graphic.
            while (!loadOperation.isDone)
            {
                // Get load progress, dividing by 0.9 because that is where it stops.
                float loadProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);

                if (loadingImage)
                {
                    if (loadAnimationType == LoadAnimationType.FillAmount) loadingImage.fillAmount = loadProgress;
                    else if (loadAnimationType == LoadAnimationType.Width && loadingImageMask) loadingImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, loadingImageMask.rect.width * loadProgress);
                }

                yield return null;
            }
        }
        protected IEnumerator LoadSceneAsync(string pSceneName)
        {
            // Start loading the scene.
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(pSceneName);

            // Show loading graphic.
            if (loadingWindow && !loadingWindow.Active) { loadingWindow.gameObject.SetActive(true); loadingWindow.Open(); }

            // Update loading graphic.
            while (!loadOperation.isDone)
            {
                // Get load progress, dividing by 0.9 because that is where it stops.
                float loadProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);

                if (loadingImage)
                {
                    if (loadAnimationType == LoadAnimationType.FillAmount) loadingImage.fillAmount = loadProgress;
                    else if (loadAnimationType == LoadAnimationType.Width && loadingImageMask) loadingImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, loadingImageMask.rect.width * loadProgress);
                }

                yield return null;
            }
        }
    }
}