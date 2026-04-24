using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using Skyworth.XR.Interaction;

namespace Skyworth.XR.Interaction.Controllers
{

public class XRControllerVisual : MonoBehaviour
{
    public XRNode m_node;
    private Animator s_ControllerAnimatior;
    private Material s_Material;
    private GameObject m_vroot;
    private Transform m_RayOrigin;
    private XRRayInteractor rayInteractor;
    public static List<GameObject> ContollerVisual = new List<GameObject>();
    public bool HiddenHandle = false;
    private bool m_initialized = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (m_initialized) return;
        m_initialized = true;

        s_ControllerAnimatior = GetComponentInChildren<Animator>();
        s_Material = GetComponentInChildren<Renderer>().material;
        m_vroot = transform.GetChild(0).gameObject;
        m_RayOrigin = transform.GetChild(1);
        rayInteractor = GetComponentInParent<XRRayInteractor>();
        if (rayInteractor) rayInteractor.rayOriginTransform = m_RayOrigin;
        ContollerVisual.Add(gameObject);
    }
    void Start()
    {
        SVROrigin.Instance.HandlePowerChanged += UpdateControllerPower;
    }

    private void UpdateControllerPower(XRNode xRNode, float power)
    {
        if (xRNode == m_node)
        {
            s_Material.SetFloat("_Battery", power);
        }
    }

    void SetAnimatorFloat(Animator animator, InputDevice device, InputFeatureUsage<bool> usage, string paramName)
    {
        float value = 0f;
        if (device.TryGetFeatureValue(usage, out var v) && v)
            value = 1f;
        animator.SetFloat(paramName, value);
    }

    void SetAnimatorFloat(Animator animator, InputDevice device, InputFeatureUsage<float> usage, string paramName, float defaultValue = 0f)
    {
        float value = defaultValue;
        if (device.TryGetFeatureValue(usage, out var v))
            value = v;
        animator.SetFloat(paramName, value);
    }

    // Update is called once per frame
    void Update()
    {
        InputDevice ControllerDevice = InputDevices.GetDeviceAtXRNode(m_node);

        if (!ControllerDevice.isValid)
        {
            if (m_vroot.activeInHierarchy)
                m_vroot.SetActive(false);
            return;
        }
        if (ControllerDevice.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState trackingState))
        {
            if ((trackingState & (InputTrackingState.Position | InputTrackingState.Rotation)) != 0)
            {
                if (!m_vroot.activeInHierarchy)
                {
                }
                else
                {
                    if (HiddenHandle)
                    {
                        m_vroot.SetActive(false);
                    }
                }
            }
        }

        SetAnimatorFloat(s_ControllerAnimatior, ControllerDevice, CommonUsages.primaryButton, "primaryButton");
        SetAnimatorFloat(s_ControllerAnimatior, ControllerDevice, CommonUsages.secondaryButton, "secondaryButton");
        SetAnimatorFloat(s_ControllerAnimatior, ControllerDevice, CommonUsages.menuButton, "menuButton");
        SetAnimatorFloat(s_ControllerAnimatior, ControllerDevice, CommonUsages.grip, "grip");
        SetAnimatorFloat(s_ControllerAnimatior, ControllerDevice, CommonUsages.trigger, "trigger");

        if (ControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisvalue))
        {
            s_ControllerAnimatior.SetFloat("primary2DAxisX", primary2DAxisvalue.x);
            s_ControllerAnimatior.SetFloat("primary2DAxisY", primary2DAxisvalue.y);
        }
        else
        {
            s_ControllerAnimatior.SetFloat("primary2DAxisX", 0);
            s_ControllerAnimatior.SetFloat("primary2DAxisY", 0);
        }
    }
}
}
