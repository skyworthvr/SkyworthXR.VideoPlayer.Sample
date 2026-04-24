using UnityEngine;

using Skyworth.XR.Interaction;

namespace Skyworth.XR.Interaction.Controllers
{

public class ControllerModelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject modelV1; // 920/910 手柄模型根节点
    [SerializeField] private GameObject modelV2; // 930 手柄模型根节点

    void Awake()
    {
        bool useV2 = DeviceDetector.CurrentDevice == SVRDeviceType.V0930;
        modelV1.SetActive(!useV2);
        modelV2.SetActive(useV2);
    }
}
}
