using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuantumTek.MenuSystem
{
    /// <summary> An instance of an OptionsList. Handles everything needed in a list of options to scroll through by use of buttons. </summary>
    [AddComponentMenu("Menu System/Menu")]
    [DisallowMultipleComponent]
    public class OptionsList : MonoBehaviour
    {
        #region Object References
        [Header("Object References")]
        [Tooltip("The Menu this list is a part of. Used for autosaving of the option.")]
        [SerializeField] protected Menu menu = null;
        [Tooltip("The Text object this list shows. Only required if using Text and not TextMeshPro for the list.")]
        [SerializeField] protected Text text = null;
        [Tooltip("The TextMeshPro object this list shows. Only required if using TextMeshPro and not Text for the list.")]
        [SerializeField] protected TextMeshProUGUI textMPro = null;
        #endregion

        #region Variables
        [Header("Variables")]
        [Tooltip("The name to save the current option as.")]
        [SerializeField] protected string settingName = "";
        [Tooltip("The current option.")]
        [SerializeField] protected int currentOption = 0;
        public int CurrentOption {
            get { return currentOption; }
            set {
                currentOption = value;
                if (currentOption < 0) currentOption = options.Count - 1;
                else if (currentOption > options.Count - 1) currentOption = 0;
                if (text) text.text = options[currentOption];
                if (textMPro) textMPro.text = options[currentOption];
            }
        }
        [Tooltip("The list of available options.")]
        [SerializeField] protected List<string> options = new List<string>();
        [Tooltip("Whether or not to save the current option.")]
        [SerializeField] protected bool saveOption = true;
        #endregion

        protected void Awake()
        {
            // Update the text.
            if (options.Count == 0) return;
            if (text) text.text = options[currentOption];
            if (textMPro) textMPro.text = options[currentOption];
        }

        /// <summary> Changes the currently displayed option. </summary>
        /// <param name="direction">Which direction to change the option, either 1 (right), or -1 (left).</param>
        public void ChangeOption(int direction)
        {
            // Play the click audio.
            if (menu) menu.PlayAudio();

            // Clamp the direction.
            if (direction == 0) return;
            if (direction > 0) direction = 1;
            else direction = -1;

            // Clamp the current option.
            currentOption += direction;
            if (currentOption < 0) currentOption = options.Count - 1;
            else if (currentOption > options.Count - 1) currentOption = 0;

            // Update the text.
            if (text) text.text = options[currentOption];
            if (textMPro) textMPro.text = options[currentOption];

            // Save the option.
            if (saveOption && menu) menu.SaveSetting(settingName);
        }

        /// <summary> Returns the selected option. </summary>
        /// <returns></returns>
        public string GetOption()
        {
            if (options.Count == 0) return "";
            return options[currentOption];
        }
    }
}