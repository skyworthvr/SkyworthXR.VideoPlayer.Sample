using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRControllerVisual : MonoBehaviour
{
    public XRNode m_node;
    public const string m_TriggerTargetProperty = "m_TriggerTarget";
    [SerializeField]
    private Transform m_TriggerTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice ControllerDevice = InputDevices.GetDeviceAtXRNode(m_node);

        
        if (ControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggervalue))
        {
            m_TriggerTarget.localRotation = Quaternion.Euler(triggervalue * 10, 0, 0);
        }
        else
        {
            m_TriggerTarget.localRotation = Quaternion.identity;
        }
    }
}
