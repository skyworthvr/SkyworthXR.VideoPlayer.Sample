using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace Skyworth.XR.Interaction.Locomotion
{

public class GazeScroll : MonoBehaviour
{
    private Vector3 worldPosition = Vector3.zero;
    private Vector2 screenPosition = Vector3.zero;
    
    private InputDevice leftControllerDevice;
    private InputDevice rightControllerDevice;
    
    private bool m_GazeScrollEnabled = false;

    [SerializeField]
    private UIInputModule inputModule;
    private UIInputModule UIInputModule
    {
        get
        {
            if (inputModule == null)
            {
                inputModule = EventSystem.current.GetComponent<UIInputModule>();
            }
            return inputModule;
        }
    }

    void Update()
    {
        leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool shouldEnable = !leftControllerDevice.isValid && !rightControllerDevice.isValid;
        if (shouldEnable != m_GazeScrollEnabled)
        {
            m_GazeScrollEnabled = shouldEnable;
            if (m_GazeScrollEnabled)
            {
                if (UIInputModule != null)
                {
                    UIInputModule.finalizeRaycastResults += FinalizeRaycastResults;
                    UIInputModule.initializePotentialDrag += InitializePotentialDrag;
                    UIInputModule.drag += Drag;
                    UIInputModule.beginDrag += BeginDrag;
                }
            }
            else
            {
                if (UIInputModule != null)
                {
                    UIInputModule.finalizeRaycastResults -= FinalizeRaycastResults;
                    UIInputModule.initializePotentialDrag -= InitializePotentialDrag;
                    UIInputModule.drag -= Drag;
                    UIInputModule.beginDrag -= BeginDrag;
                }
            }
        }
    }

    private void FinalizeRaycastResults(PointerEventData eventData, List<RaycastResult> list)
    {
        eventData.pressPosition = Camera.main.WorldToScreenPoint(worldPosition);
    }

    private void InitializePotentialDrag(GameObject go, PointerEventData eventData)
    {
        worldPosition = eventData.pointerCurrentRaycast.worldPosition;
    }

    private void Drag(GameObject go, PointerEventData eventData)
    {
        eventData.position = screenPosition;
    }

    private void BeginDrag(GameObject go, PointerEventData eventData)
    {
        screenPosition = eventData.position;
    }

}
}
