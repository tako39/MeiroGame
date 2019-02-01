using UnityEngine;
using UnityEngine.UI;

namespace QuantumTek.MenuSystem
{
    /// <summary> The animation type of a tab. </summary>
    [System.Serializable]
    public enum TabAnimationType
    {
        /// <summary> Nothing happens when the tab's active state is changed. </summary>
        None,
        /// <summary> The tab's color changes when the tab's active state is changed. </summary>
        Color,
        /// <summary> The tab's sprite changes when the tab's active state is changed. </summary>
        Sprite,
        /// <summary> An animation is triggered by a trigger animator parameter when the tab's active state is changed. </summary>
        Trigger,
        /// <summary> An animation is triggered by a boolean animator parameter when the tab's active state is changed. </summary>
        Boolean
    }

    /// <summary> An instance of a Tab. Handles everything needed in a tab. </summary>
    [AddComponentMenu("Menu System/Tab")]
    [DisallowMultipleComponent]
    public class Tab : MonoBehaviour
    {
        #region Object References
        [Header("Object References")]
        [Tooltip("The Image attached to this tab.")]
        [SerializeField] protected Image image = null;
        [Tooltip("The Animator attached to this tab.")]
        [SerializeField] protected Animator animator = null;
        #endregion

        #region Variables
        protected bool active = false;
        public bool Active { get { return active; } set { Activate(value); } }
        protected bool setup = false;
        [Header("Variables")]
        [Tooltip("The animation type of this tab.")]
        [SerializeField] protected TabAnimationType animationType = TabAnimationType.Trigger;
        public TabAnimationType AnimationType { get { return animationType; } }
        [Tooltip("The color of the tab when active. This is not needed unless a color animation is used.")]
        [SerializeField] protected Color activeColor = new Color(1, 1, 1, 1);
        [Tooltip("The color of the tab when inactive. This is not needed unless a color animation is used.")]
        [SerializeField] protected Color inactiveColor = new Color(1, 1, 1, 0.5f);
        [Tooltip("The sprite of the tab when active. This is not needed unless a sprite animation is used.")]
        [SerializeField] protected Sprite activeSprite = null;
        [Tooltip("The sprite of the tab when inactive. This is not needed unless a sprite animation is used.")]
        [SerializeField] protected Sprite inactiveSprite = null;
        [Tooltip("The name of the trigger animator parameter that activates the tab. This is not needed unless an animator with trigger parameters is used.")]
        [SerializeField] protected string activeTrigger = "Activate Scale";
        [Tooltip("The name of the trigger animator parameter that deactivates the tab. This is not needed unless an animator with trigger parameters is used.")]
        [SerializeField] protected string inactiveTrigger = "Deactivate Scale";
        [Tooltip("The name of the boolean animator parameter that controls the active state of the tab. This is not needed unless an animator with a boolean parameter is used.")]
        [SerializeField] protected string activeBool = "";
        [Tooltip("The name of the activation animation. This is not needed unless an animator is used.")]
        [SerializeField] protected string activeAnimation = "Activate Scale";
        [Tooltip("The name of the deactivation animation. This is not needed unless an animator is used.")]
        [SerializeField] protected string inactiveAnimation = "Deactivate Scale";
        #endregion

        protected void Activate(bool pActive)
        {
            // Return if already the given state, otherwise set the current state to the given.
            if (!setup || (active && pActive) || (!active && !pActive)) return;
            active = pActive;

            // Return if an Image is required and there isn't one assigned.
            if (!image && (animationType == TabAnimationType.Color || animationType == TabAnimationType.Sprite)) return;
            // Return if an Animator is required and there isn't one assigned.
            if (!animator && (animationType == TabAnimationType.Trigger || animationType == TabAnimationType.Boolean)) return;

            // Animate the tab based on the animation type.
            if (animationType == TabAnimationType.None) return;
            else if (animationType == TabAnimationType.Color) image.color = active ? activeColor : inactiveColor;
            else if (animationType == TabAnimationType.Sprite) image.sprite = active ? activeSprite : inactiveSprite;
            else if (animationType == TabAnimationType.Trigger)
            {
                if (active) animator.SetTrigger(activeTrigger);
                else animator.SetTrigger(inactiveTrigger);
            }
            else if (animationType == TabAnimationType.Boolean) animator.SetBool(activeBool, active);
        }

        /// <summary> Sets up the tab. Only for use by the Menu script. </summary>
        /// <param name="pActive">Whether or not the tab is active.</param>
        public void Setup(bool pActive)
        {
            active = pActive;
            // Set the animation to the end result.
            if (active)
            {
                if (animationType == TabAnimationType.None) return;
                if (animationType == TabAnimationType.Color) image.color = active ? activeColor : inactiveColor;
                if (animationType == TabAnimationType.Sprite) image.sprite = active ? activeSprite : inactiveSprite;
                if (animationType == TabAnimationType.Trigger) if (activeAnimation.Length > 0) animator.PlayInFixedTime(activeAnimation, 0, 1);
                if (animationType == TabAnimationType.Boolean)
                {
                    animator.SetBool(activeBool, active);
                    if (activeAnimation.Length > 0) animator.PlayInFixedTime(activeAnimation, 0, 1);
                }
            }
            else
            {
                if (animationType == TabAnimationType.None) return;
                if (animationType == TabAnimationType.Color) image.color = active ? activeColor : inactiveColor;
                if (animationType == TabAnimationType.Sprite) image.sprite = active ? activeSprite : inactiveSprite;
                if (animationType == TabAnimationType.Trigger) if (inactiveAnimation.Length > 0) animator.PlayInFixedTime(inactiveAnimation, 0, 1);
                if (animationType == TabAnimationType.Boolean)
                {
                    animator.SetBool(activeBool, active);
                    if (inactiveAnimation.Length > 0) animator.PlayInFixedTime(inactiveAnimation, 0, 1);
                }
            }

            setup = true;
        }
    }
}