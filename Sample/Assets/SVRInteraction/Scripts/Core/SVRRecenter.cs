using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Skyworth.XR.Interaction
{

public class SVRRecenter : MonoBehaviour
{
    private Transform cameraTransform;  
    private Vector3 initialCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        if (cameraTransform != null)
        {
            initialCameraPosition = cameraTransform.position;
        }
    }

    public void Recenter()
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        if (cameraTransform != null)
        {
            Quaternion rotationInverse = Quaternion.Inverse(cameraTransform.rotation);
            // 将旋转差分解为X、Y、Z轴的旋转
            Vector3 eulerDelta = rotationInverse.eulerAngles;
            // 创建一个只有Y轴旋转的四元数
            Quaternion yzRotation = Quaternion.Euler(0f, eulerDelta.y, 0f);
            transform.rotation = yzRotation;

            Vector3 positionDelta = cameraTransform.position - initialCameraPosition;
            positionDelta.y = 0f;
            transform.position -= positionDelta;
        }
    }
}
}
