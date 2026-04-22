using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
#if SVR_V920
using UnityEngine.XR.OpenXR;
using Qualcomm.Snapdragon.Spaces;
#endif

public class SVROrigin : XROrigin
{
	public static SVROrigin Instance;
	
	public Action RecenterOriginCallback;
	
	public bool HandleTipsEnable = false;
	
	public bool ControllerVisible = true;
	
	private bool mTransitionOrTurnEnable = true;

    private int m_NewDragThreshold = 30;

#if SVR_V920
    private BaseRuntimeFeature _baseRuntimeFeature = null;
#endif
    public bool TransitionOrTurnEnable
	{
		get
		{
			return mTransitionOrTurnEnable;
		}
		set
		{
			mTransitionOrTurnEnable = value;
			if (LHand)
			{
				LHand.TransitionOrTurnEnable = mTransitionOrTurnEnable;
			}

			if (RHand)
			{
				RHand.TransitionOrTurnEnable = mTransitionOrTurnEnable;
			}
		}
	}

	public Action<XRNode, bool, bool> HandleStateChanged;
	
	[SerializeField]
	private ActionBasedControllerManager LHand;
	[SerializeField]
	private ActionBasedControllerManager RHand;

	[SerializeField] 
	private GameObject HeadController;

	private InputDevice leftControllerDevice;
	private InputDevice rightControllerDevice;
	private InputTrackingState inputTrackingState;

	private static readonly List<XRInputSubsystem> inputSubsystems = new List<XRInputSubsystem>();


	public bool LHandConnect = false;
    public bool RHandConnect = false;

	private bool controllerStateComplete = false;

	void Awake()
	{
		base.Awake();
		Instance = this;
	}

	private IEnumerator Start()
	{
		base.Start();
		
		yield return 0;
		yield return 0;
#if UNITY_ANDROID && !UNITY_EDITOR
        bool initCallback = false;
        while (!initCallback)
        {
            SubsystemManager.GetInstances(inputSubsystems);
            if (inputSubsystems.Count > 0)
            {
                foreach (var inputSubsystem in inputSubsystems)
                {
                    inputSubsystem.trackingOriginUpdated -= OnInputSubsystemTrackingOriginUpdated;
                    inputSubsystem.trackingOriginUpdated += OnInputSubsystemTrackingOriginUpdated;
                }
                initCallback = true;
            }
        }
        SetEventSystemClickThreshold();
#endif
    }

    public void SystemRecenter()
    {
        SubsystemManager.GetInstances(inputSubsystems);
        for (int i = 0; i < inputSubsystems.Count; i++)
        {
            inputSubsystems[i].TryRecenter();
        }
    }

	private void OnInputSubsystemTrackingOriginUpdated(XRInputSubsystem inputSubsystem)
	{
		transform.rotation = Quaternion.identity;
		transform.position = Vector3.zero;
		
		RecenterOriginCallback?.Invoke();
	}

	void Update()
	{
		UpdateControllerState();

		if (LHand)
		{
			LHand.gameObject.SetActive(LHandConnect && ControllerVisible);
		}

		if (RHand)
		{
			RHand.gameObject.SetActive(RHandConnect && ControllerVisible);
		}

		if (!LHandConnect && !RHandConnect)
		{

			SetHeadControllerEnable(true);
		}
		else
		{
            SetHeadControllerEnable(false);
        }

    }

	public void SetHeadControllerEnable(bool enable)
	{
		if (HeadController.activeSelf != enable)
			Debug.Log($"HandleStateChanged		SetHeadControllerEnable:{enable}");
		HeadController.SetActive(enable);
	}

	private void UpdateControllerState()
	{
		leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		
		//left
		bool oldState = LHandConnect;
		if (leftControllerDevice.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState lTrackingState))
		{
			LHandConnect = (lTrackingState & (InputTrackingState.Position | InputTrackingState.Rotation)) != 0;
		}
		else
		{
			LHandConnect = false;
		}
		if (oldState != LHandConnect)
		{
			HandleStateChanged?.Invoke(XRNode.LeftHand, LHandConnect, controllerStateComplete);
            Debug.Log("HandleStateChanged LeftHand:" + LHandConnect + "|" + RHandConnect + "|" + lTrackingState);
		}
		
		//right
		oldState = RHandConnect;
		if (rightControllerDevice.TryGetFeatureValue(CommonUsages.trackingState, out InputTrackingState rTrackingState))
		{
			RHandConnect = (rTrackingState & (InputTrackingState.Position | InputTrackingState.Rotation)) != 0;
		}
		else
		{
			RHandConnect = false;
		}
		if (oldState != RHandConnect)
		{
			HandleStateChanged?.Invoke(XRNode.RightHand, RHandConnect, controllerStateComplete);
            Debug.Log("HandleStateChanged RightHand:" + LHandConnect + "|" + RHandConnect + "|" + rTrackingState);
		}

        if (!controllerStateComplete)
        {
            Debug.Log("Init Controller State:" + LHandConnect + "|" + RHandConnect + "|" + lTrackingState + "|" + rTrackingState);
        }
		controllerStateComplete = true;
	}

    public void SetPassthrough(bool flag)
    {
#if SVR_V920
        _baseRuntimeFeature = OpenXRSettings.Instance.GetFeature<BaseRuntimeFeature>();
        if (!_baseRuntimeFeature)
        {
            Debug.LogWarning("Base Runtime Feature isn't available.");
            return;
        }
        if(_baseRuntimeFeature.IsPassthroughSupported())
        {
            if (!_baseRuntimeFeature)
            {
                Debug.LogWarning("Base Runtime Feature isn't available.");
                return;
            }
            _baseRuntimeFeature.SetPassthroughEnabled(flag);
        }
        else
        {
            Debug.LogWarning("Base Runtime Feature passthrough is not support!");
        }
#endif
    }

    private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			OnInputSubsystemTrackingOriginUpdated(null);
			controllerStateComplete = false;
		}
	}

    private void SetEventSystemClickThreshold()
    {
        EventSystem.current.pixelDragThreshold = m_NewDragThreshold;
    }

}
