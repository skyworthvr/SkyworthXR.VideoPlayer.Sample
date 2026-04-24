using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Skyworth.XR.Interaction
{

public class SVRRayInteractor : XRRayInteractor
{
    internal const float kPixelPerLine = 0.1f;
    
#if !ENABLE_INPUT_SYSTEM
        [HideInInspector]
#endif
    [SerializeField]
    [Tooltip("Scroll wheel input action reference, typically the scroll wheel on a mouse.")]
    InputActionReference m_ScrollWheelAction;
    
    [SerializeField]
    public XRNode m_node;
    /// <summary>
    /// The Input System action to use to move the pointer on the currently active UI. Must be a <see cref="Vector2Control"/> Control.
    /// </summary>
    public InputActionReference scrollWheelAction
    {
        get => m_ScrollWheelAction;
        set => SetInputAction(ref m_ScrollWheelAction, value);
    }

    public XRNode node
    {
        get => m_node;
        set => m_node = value;
    }

    public override void UpdateUIModel(ref TrackedDeviceModel model)
    {
        base.UpdateUIModel(ref model);
        if (!isActiveAndEnabled)
            return;
        model.scrollDelta = m_ScrollWheelAction.action.ReadValue<Vector2>() * (1 / kPixelPerLine);
    }

    void SetInputAction(ref InputActionReference inputAction, InputActionReference value)
    {
        if (Application.isPlaying && inputAction != null)
            inputAction.action?.Disable();

        inputAction = value;

        if (Application.isPlaying && isActiveAndEnabled && inputAction != null)
            inputAction.action?.Enable();
    }
}
}
