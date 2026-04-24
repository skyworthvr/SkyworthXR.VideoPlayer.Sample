
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.Scripting;

namespace Skyworth.XR.Interaction.UI
{

public struct HeadInputDeviceInputState : IInputStateTypeInfo
{
    public FourCC format => new FourCC('H', 'E', 'A', 'D');

    [Preserve,InputControl(name = "trigger", usage = "Trigger", layout = "Axis", aliases = new[] {"Primary", "select"})]
    public float trigger;
        
    [Preserve, InputControl(name = "triggerPressed", layout = "Button", aliases = new[] {"PrimaryButton", "selectPressed"})]
    public bool selectPressed;
}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[InputControlLayout(displayName = "HeadInputDevice", stateType = typeof(HeadInputDeviceInputState))]
public class HeadInputDevice : InputDevice
{
    public const string kDeviceName = "HeadInputDevice";
    
    public AxisControl select { get; private set; }
    public ButtonControl selectPressed { get; private set; }

    static HeadInputDevice() {
        InputSystem.RegisterLayout<HeadInputDevice>(
            matches:
            new InputDeviceMatcher()
                .WithProduct(kDeviceName));
    }

    protected override void FinishSetup() {
        base.FinishSetup();
        select = GetChildControl<AxisControl>("trigger");
        selectPressed = GetChildControl<ButtonControl>("triggerPressed");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeInPlayer() {
    }
}
}
