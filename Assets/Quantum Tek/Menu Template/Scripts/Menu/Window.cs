using UnityEngine;

namespace QuantumTek.MenuSystem
{
    /// <summary> The animation type of a window. </summary>
    [System.Serializable]
    public enum WindowAnimationType
    {
        /// <summary> The window is not animated. </summary>
        None,
        /// <summary> The window is animated by a trigger. </summary>
        Trigger,
        /// <summary> The window is animated by a boolean. </summary>
        Boolean
    }

    /// <summary> An instance of a Window. Handles everything needed in a window. </summary>
    [AddComponentMenu("Menu System/Window")]
    [DisallowMultipleComponent]
    public class Window : MonoBehaviour
    {
        #region Object References
        [Header("Object References")]
        [Tooltip("The tab on this window.")]
        [SerializeField] protected Tab tab = null;
        public Tab Tab { get { return tab; } }
        [Tooltip("The Animator attached to this window. This is not needed.")]
        [SerializeField] protected Animator animator = null;
        [Tooltip("The GameObject holding all of the graphics for this window.")]
        [SerializeField] protected GameObject graphics = null;
        #endregion

        #region Variables
        [Header("Variables")]
        [Tooltip("The name of this window.")]
        [SerializeField] protected new string name = null;
        public string Name { get { return name; } }
        protected bool active = false;
        public bool Active { get { return active; } }
        protected bool setup = false;
        [Tooltip("The animation type of this window.")]
        [SerializeField] protected WindowAnimationType animationType = WindowAnimationType.Trigger;
        public WindowAnimationType AnimationType { get { return animationType; } }
        [Tooltip("The name of the trigger animator parameter that opens the window. This is not needed unless an animator with trigger parameters is used.")]
        [SerializeField] protected string openTrigger = "Open Scale";
        [Tooltip("The name of the trigger animator parameter that closes the window. This is not needed unless an animator with trigger parameters is used.")]
        [SerializeField] protected string closeTrigger = "Close Scale";
        [Tooltip("The name of the boolean animator parameter that controls the open state of the window. This is not needed unless an animator with a boolean parameter is used.")]
        [SerializeField] protected string openBool = "";
        #endregion

        /// <summary> Opens the window and triggers an animation if there is one. </summary>
        public void Open()
        {
            if (!setup || active) return;
            // Animate the window based on the animation type.
            active = true;
            if (!animator && animationType != WindowAnimationType.None) return;
            if (animationType == WindowAnimationType.None && graphics) graphics.SetActive(active);
            else if (animationType == WindowAnimationType.Trigger) animator.SetTrigger(openTrigger);
            else if (animationType == WindowAnimationType.Boolean) animator.SetBool(openBool, active);

            if (Tab) Tab.Active = active;
        }
        /// <summary> Closes the window and triggers an animation if there is one. </summary>
        public void Close()
        {
            if (!setup || !active) return;
            // Animate the window based on the animation type.
            active = false;
            if (!animator && animationType != WindowAnimationType.None) return;
            if (animationType == WindowAnimationType.None && graphics) graphics.SetActive(active);
            else if (animationType == WindowAnimationType.Trigger) animator.SetTrigger(closeTrigger);
            else if (animationType == WindowAnimationType.Boolean) animator.SetBool(openBool, active);

            if (Tab) Tab.Active = active;
        }

        /// <summary> Sets up the window. Only for use by the Menu script. </summary>
        public void Setup()
        {
            // Set the active state of the window.
            if (!graphics) { active = false; setup = true; return; }
            active = graphics.activeSelf;
            setup = true;
        }
    }
}