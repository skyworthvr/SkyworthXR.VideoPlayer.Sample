using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
#if SVR_V920
using UnityEngine.XR.OpenXR;
#endif

#if SNPDRAGON_SPACE
using Qualcomm.Snapdragon.Spaces;
using QCHT.Interactions.Core;
#endif

using Skyworth.XR.Interaction.Hands;
using Skyworth.XR.Interaction.Controllers;

namespace Skyworth.XR.Interaction
{

public enum EyeTextureResolutionLevel
{
    Low,//1504
    Normal,//1600
    Middle,//1920
    High,//2160
    VeryHigh,//2400
}

public class SVROrigin : XROrigin
{
	public static SVROrigin Instance;
	
	public Action RecenterOriginCallback;
	
	public bool HandleTipsEnable = false;

	private bool controllerVisible = true;

    private bool controllerModeVisible = true;

	private bool mTransitionOrTurnEnable = true;

#if SNPDRAGON_SPACE
    private BaseRuntimeFeature _baseRuntimeFeature = null;
    private HandTrackingFeature _handTrackingFeature = null;
#endif
#if SVR_V920
	private SvrOpenxrFeature _svrOpenxrFeature = null;
#endif
    private ulong _svrOpenxrFeatureRecentCount = 0;

    private int m_NewDragThreshold = 30;

    public HandEnable handEnable;

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

    public bool ControllerVisible
    {
        set
        {
            controllerVisible = value;
        }
        get
        {
            return controllerVisible;
        }
    }

    public bool ControllerModeVisible
    {
        set
        {
            controllerModeVisible = value;
        }
        get
        {
            return controllerModeVisible;
        }
    }


    public Action<XRNode, bool, bool> HandleStateChanged;
    public Action<XRNode, float> HandlePowerChanged;
    
    InputTrackingState lTrackingState = InputTrackingState.None;
    InputTrackingState rTrackingState = InputTrackingState.None;
	
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

	[SerializeField]
	private float handCheckInterval = 1.2f;
	private float m_HandCheckTimer;

	public bool ResetOffsetOnRecenter = true;

	void Awake()
	{
		base.Awake();
		Instance = this;
	}

	private IEnumerator Start()
	{
		base.Start();
        Application.onBeforeRender += UpdateRecenterCounter;
		
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
		        }
		        initCallback = true;
		    }
		}
        StartCoroutine(DelayToSetHeadController());
#if SVR_V920
        _svrOpenxrFeature = OpenXRSettings.Instance.GetFeature<SvrOpenxrFeature>();
        _svrOpenxrFeature.OnSpaceChangeEvent += SvrOpenxrFeature_OnSpaceChangeEvent;
#endif
        SetEventSystemClickThreshold();
#if SNPDRAGON_SPACE
        _handTrackingFeature = OpenXRSettings.Instance.GetFeature<HandTrackingFeature>();
#endif
#endif

    }

    private void SvrOpenxrFeature_OnSpaceChangeEvent()
    {
        if (ResetOffsetOnRecenter)
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
        }

        LogTool.Log("SVROrigin=>SvrOpenxrFeature_OnSpaceChangeEvent RecenterOriginCallback");
        RecenterOriginCallback?.Invoke();
    }

    public IEnumerator DelayToSetHeadController()
    {
        yield return new WaitForSeconds(2);
        SetHeadControllerEnable(true);
    }


	void OnDestroy()
	{
		base.OnDestroy();
		Application.onBeforeRender -= UpdateRecenterCounter;
	}

    public void SystemRecenter()
    {
#if SNPDRAGON_SPACE
        _baseRuntimeFeature = OpenXRSettings.Instance.GetFeature<BaseRuntimeFeature>();
        if (!_baseRuntimeFeature)
        {
            LogTool.Log("SVROrigin => Base Runtime Feature isn't available.");
            return;
        }
        bool result = _baseRuntimeFeature.TryResetPose();
        LogTool.Log($"SVROrigin => Base Runtime Feature Try Reset Pose Result={result}");
#endif
    }

    /// <summary>
    /// Custom Recenter (for scene or UI Recenter)
    /// </summary>
    /// <param name="UIRootTrs">UI Root</param>
    /// <param name="SceneRootTrs">Scene Root</param>
    /// <param name="OtherRootTrs">Other nedd Recenter Transform params</param>
    public void RecenterOrigin(Transform UIRootTrs, Transform SceneRootTrs, params Transform[] OtherRootTrs)
    {
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        Vector3 tempRegionPosition = new Vector3(Camera.main.transform.position.x, 0f, Camera.main.transform.position.z);
        Vector3 tempRegionEuler = new Vector3(0f, Camera.main.transform.eulerAngles.y, 0f);
        if (UIRootTrs)
        {
            UIRootTrs.position = new Vector3(tempRegionPosition.x, UIRootTrs.position.y, tempRegionPosition.z);
            UIRootTrs.eulerAngles = tempRegionEuler;
        }
        if (SceneRootTrs)
        {
            SceneRootTrs.position = new Vector3(tempRegionPosition.x, SceneRootTrs.position.y, tempRegionPosition.z);
            SceneRootTrs.eulerAngles = tempRegionEuler;
        }

        foreach(var otherRootTrs in OtherRootTrs)
        {
            otherRootTrs.position = new Vector3(tempRegionPosition.x, otherRootTrs.position.y, tempRegionPosition.z);
            otherRootTrs.eulerAngles = tempRegionEuler;
        }
    }

    private void OnInputSubsystemTrackingOriginUpdated(XRInputSubsystem inputSubsystem)
	{
		if (ResetOffsetOnRecenter)
		{
			transform.localRotation = Quaternion.identity;
			transform.localPosition = Vector3.zero;
		}

        LogTool.Log("SVROrigin=>OnInputSubsystemTrackingOriginUpdated RecenterOriginCallback");
        RecenterOriginCallback?.Invoke();
	}
	
	void Update()
	{
		UpdateControllerState();
		
		if (LHand)
		{
			LHand.gameObject.SetActive(LHandConnect && controllerVisible);
		}

		if (RHand)
		{
			RHand.gameObject.SetActive(RHandConnect && controllerVisible);
		}

        if (LHand)
        {
            LHand.transform.Find("LeftBaseController/ModelPt").gameObject.SetActive(LHandConnect && controllerModeVisible);
        }

        if (RHand)
        {
            RHand.transform.Find("RightBaseController/ModelPt").gameObject.SetActive(RHandConnect && controllerModeVisible);
        }

        if(!LHandConnect && !RHandConnect)
        {
            m_HandCheckTimer += Time.deltaTime;
            if (m_HandCheckTimer >= handCheckInterval)
            {
                m_HandCheckTimer = 0f;
                if (handEnable != null)
                {
#if SNPDRAGON_SPACE
                    if (SystemProperties.get("persist.sxr.handtracking.enable", "0") == "1" && controllerVisible
                        && (_handTrackingFeature != null && _handTrackingFeature.enabled))
#else
                        if (SystemProperties.get("persist.sxr.handtracking.enable","0") == "1" && controllerVisible)
#endif
                    {
                        //全局菜单显示的情况下不显示其他应用手势
                        if (SystemProperties.get("sxr.globalmenu.open", "0") == "1")
                            handEnable.SetHandDisable();
                        else
                            handEnable.SetHandEnable();
                        SetHeadControllerEnable(false);
                    }
                    else
                    {
                        handEnable.SetHandDisable();
                        SetHeadControllerEnable(true);
                    }
                }
            }
        }
        else
        {
            if (handEnable != null)
            {
                handEnable.SetHandDisable();
            }
        }
    }

	[Obsolete("仅供SVROrigin调用，外部调用SetHeadGazeVisible")]
	public void SetHeadControllerEnable(bool enable)
	{
		HeadController.SetActive(enable);
	}

	public void SetHeadGazeVisible(bool visible)
	{
		HeadController.transform.GetChild(0).gameObject.SetActive(visible);
	}

	/// <summary>
    /// when svr api handle power changed update handle model power uv
    /// </summary>
    /// <param name="xRNode"></param>
    /// <param name="power"></param>
    public void UpdateControllerPower(XRNode xRNode, float power)
    {
        HandlePowerChanged?.Invoke(xRNode, power);
    }

	private void UpdateControllerState()
	{
		leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
		rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        //left
        bool oldState = LHandConnect;
        leftControllerDevice.TryGetFeatureValue(CommonUsages.isTracked, out LHandConnect);
		
		if (oldState != LHandConnect)
		{
			HandleStateChanged?.Invoke(XRNode.LeftHand, LHandConnect, controllerStateComplete);
			LogTool.Log($"SVROrigin => HandleStateChanged LeftHand:{LHandConnect}|{RHandConnect}|{lTrackingState}");
		}
		
		//right
		oldState = RHandConnect;
        rightControllerDevice.TryGetFeatureValue(CommonUsages.isTracked, out RHandConnect);
       
        if (oldState != RHandConnect)
		{
			HandleStateChanged?.Invoke(XRNode.RightHand, RHandConnect, controllerStateComplete);
			LogTool.Log($"SVROrigin => HandleStateChanged RightHand:{LHandConnect}|{RHandConnect}|{rTrackingState}");
		}

		if (!controllerStateComplete)
		{
			LogTool.Log($"SVROrigin => Init Controller State:{LHandConnect}|{RHandConnect}|{lTrackingState}|{rTrackingState}");
		}
		controllerStateComplete = true;
    }

    private void UpdateRecenterCounter()
    {
    }

    public void ResetHeight()
    {
	    var y = 1.68f - Camera.main.transform.localPosition.y - CameraFloorOffsetObject.transform.localPosition.y;
	    transform.parent.localPosition = new Vector3(0, y, 0);
	    string debugs = "ResetHeight " + "[Camera Local:" + Camera.main.transform.localPosition.y + "]";
	    debugs = debugs + "[YOffset:" + CameraFloorOffsetObject.transform.localPosition.y + "]";
	    debugs = debugs + "[Origin:" + transform.localPosition.y + "]";
	    debugs = debugs + "[Ground:" + y + "]" + "[Camera World:" + Camera.main.transform.position.y + "]";
	    LogTool.Log(debugs);
    }

    public void SetPassthrough(bool throughFlag, bool writeProperty=false)
    {
#if SNPDRAGON_SPACE
        _baseRuntimeFeature = OpenXRSettings.Instance.GetFeature<BaseRuntimeFeature>();
        if (!_baseRuntimeFeature)
        {
            LogTool.Log("SVROrigin => Base Runtime Feature isn't available.");
            return;
        }
        if(_baseRuntimeFeature.IsPassthroughSupported())
        {
            if (!_baseRuntimeFeature)
            {
                LogTool.Log("SVROrigin => Base Runtime Feature isn't available.");
                return;
            }
            Camera.main.clearFlags = throughFlag ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
            Camera.main.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _baseRuntimeFeature.SetPassthroughEnabled(throughFlag);
            controllerModeVisible = !throughFlag;
            ControllerVisible = !throughFlag;
        }
        else
        {
            LogTool.Log("SVROrigin => Base Runtime Feature passthrough is not support!");
        }
#elif SVR_V920

        Camera.main.clearFlags = throughFlag ? CameraClearFlags.SolidColor : CameraClearFlags.Skybox;
        Camera.main.backgroundColor = new Color(0f, 0f, 0f, 0f);
        Sxr.NativeLib.SetBlendMode(throughFlag ? 3 : 1);
        SVROrigin.Instance.ControllerVisible = !throughFlag;
#endif
    }

    private void SetEventSystemClickThreshold()
    {
        EventSystem.current.pixelDragThreshold = m_NewDragThreshold;
    }

    [Obsolete("use SetEyeTextureResolutionLevel")]
    /// <summary>
    ///  For app set eyes texture scale
    /// </summary>
    /// <param name="resolutionScale"></param>
    public void SetEyeTextureResolutionScale(float resolutionScale)
    {
        UnityEngine.XR.XRSettings.eyeTextureResolutionScale = resolutionScale;
        LogTool.Log($"XRSettings.eyeTextureResolutionScale = {XRSettings.eyeTextureResolutionScale} " +
            $" eyeTextureHeight={XRSettings.eyeTextureHeight} " +
            $"eyeTextureWidth={XRSettings.eyeTextureWidth}");
    }

    public void SetEyeTextureResolutionLevel(EyeTextureResolutionLevel level)
    {
        float eyeTextureResolutionScale = 1600f / 1600f;
        switch(level)
        {
            case EyeTextureResolutionLevel.Low:
                eyeTextureResolutionScale = 1504f / 1600f;
                break;
            case EyeTextureResolutionLevel.Normal:
                eyeTextureResolutionScale = 1600f / 1600f;
                break;
            case EyeTextureResolutionLevel.Middle:
                eyeTextureResolutionScale = 1920 / 1600f;
                break;
            case EyeTextureResolutionLevel.High:
                eyeTextureResolutionScale = 2160f / 1600f;
                break;
            case EyeTextureResolutionLevel.VeryHigh:
                eyeTextureResolutionScale = 2400f / 1600f;
                break;
        }
        SetEyeTextureResolutionScale(eyeTextureResolutionScale);
    }

    private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
            controllerStateComplete = false;
		}
	}

    private Coroutine trackingOriginCoroutne;
    private IEnumerator WaitForRuntimeTrackingOrigin(int frame)
    {
        int i = 0;
        while (i < frame)
        {
            yield return null;
            i++;
        }
        OnInputSubsystemTrackingOriginUpdated(null);
    }

}
}
