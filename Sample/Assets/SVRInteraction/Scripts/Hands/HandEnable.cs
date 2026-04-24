using System;
using System.Collections;
using System.Collections.Generic;
using QCHT.Interactions.Core;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace Skyworth.XR.Interaction.Hands
{

public class HandEnable : MonoBehaviour
{
    [SerializeField]
    private GameObject handTrackingManager;

    private bool handEnable = false;

    private static readonly List<XRHandTrackingSubsystem> handTrackingSubsystems = new List<XRHandTrackingSubsystem>();
    IEnumerator Start()
    {
        
        HandTrackingFeature feature = OpenXRSettings.Instance.GetFeature<HandTrackingFeature>();
        bool initCallback = !feature.enabled;
        
        while (!initCallback)
        {
            SubsystemManager.GetInstances(handTrackingSubsystems);
            if (handTrackingSubsystems.Count > 0)
            {
                initCallback = true;
            }
            yield return 0;
        }
        
        if (handEnable)
        {
            LogTool.Log("HandEnable => HandTracking Subsystem Start");
            foreach (var sys in handTrackingSubsystems)
            {
                sys.Start();
            }
        }
    }

    public void SetHandEnable()
    {
        if (handTrackingManager != null)
            handTrackingManager.SetActive(true);

        if (!handEnable)
        {
            LogTool.Log("HandEnable => HandTracking Subsystem Start");
            foreach (var sys in handTrackingSubsystems)
            {
                sys.Start();
            }
        }
        handEnable = true;
    }

    public void SetHandDisable()
    {
        if (handTrackingManager != null)
        {
            handTrackingManager.SetActive(false);
        }

        if (handEnable)
        {
            LogTool.Log("HandEnable => HandTracking Subsystem Stop");
            foreach (var sys in handTrackingSubsystems)
            {
                sys.Stop();
            }
        }
        handEnable = false;
    }
}
}