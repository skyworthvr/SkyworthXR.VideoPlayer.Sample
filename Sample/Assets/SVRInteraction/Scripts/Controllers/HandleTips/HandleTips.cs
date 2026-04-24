using UnityEngine;
using UnityEngine.XR;

using Skyworth.XR.Interaction;

namespace Skyworth.XR.Interaction.Controllers
{

public class HandleTips : MonoBehaviour
{
    [SerializeField]
    private int direction = 1;
    [SerializeField]
    private float distance = 0.05f;
    [SerializeField]
    private Transform center;
    [SerializeField]
    private XRNode handleNode;
    
    private Vector3 eyeToHandle;
    private Vector3 planNormal;
    private Vector3 projection;
    private Vector3 localPosition;

    private bool visible = false;

    private float connectTime = -1;

    void Awake()
    {
        SVROrigin.Instance.HandleStateChanged += HandleStateChanged;
    }

    void HandleStateChanged(XRNode node, bool state, bool initComplete)
    {
        if (node == handleNode && state && initComplete)
        {
            connectTime = Time.time;
        }
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            connectTime = -1;
        }
    }

    void Update()
    {
        visible = false;
        if (SVROrigin.Instance.HandleTipsEnable)
        {
            if (connectTime > 0 && Time.time - connectTime < 30)
            {
                visible = true;
            }
            else
            {
                float distance = Vector3.Distance(center.position, Camera.main.transform.position);
                visible = distance < 0.3f;
                connectTime = -1;
            }
        }
        transform.GetChild(0).gameObject.SetActive(visible);
        if (!visible)
        {
            return;
        }
        
        transform.parent.localEulerAngles = Vector3.zero;
        eyeToHandle = (transform.parent.position - Camera.main.transform.position).normalized;
        planNormal = transform.parent.forward * -1;
        projection = eyeToHandle - planNormal * Vector3.Dot(planNormal, eyeToHandle);
        projection = projection.normalized * -1;
        transform.position = transform.parent.position + projection * distance;
        
        localPosition = transform.localPosition;
        localPosition.z = Mathf.Lerp(0, -0.04f, Vector3.Dot(planNormal, Vector3.up));
        transform.localPosition = localPosition;
        transform.parent.localEulerAngles = new Vector3(0, 0,   Mathf.Lerp(-45 * direction,45 * direction, Vector3.Dot(planNormal, Vector3.up)));
        
        transform.LookAt(Camera.main.transform);
    }
}
}
