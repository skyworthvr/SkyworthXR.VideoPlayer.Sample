using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Skyworth.XR.Interaction.Locomotion
{

public class InputActionExtensions : MonoBehaviour
{
    private bool inputEnable = true;
    
    private InputDevice HMDDevice;
    private InputActionManager inputActionManager;

    void Start()
    {
        inputActionManager = GetComponent<InputActionManager>();
        HMDDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
    }
    
    void Update()
    {
        if (!HMDDevice.TryGetFeatureValue(new InputFeatureUsage<bool>("Focused"), out bool focused))
        {
            focused = true;
        }
        
        if (inputEnable != focused)
        {
            inputEnable = focused;
            if (inputEnable)
            {
                inputActionManager.EnableInput();
                Debug.Log("InputActionExtensions EnableInput");
            }
            else
            {
                inputActionManager.DisableInput();
                Debug.Log("InputActionExtensions DisableInput");
            }
        }
    }

}
}