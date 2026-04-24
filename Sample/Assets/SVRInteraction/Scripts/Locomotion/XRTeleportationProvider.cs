using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Skyworth.XR.Interaction.Locomotion
{

public class XRTeleportationProvider : TeleportationProvider
{
    protected override void Update()
    {
        base.Update();
    }
    
    public override bool QueueTeleportRequest(TeleportRequest teleportRequest)
    {
        var xrOrigin = system.xrOrigin;
        teleportRequest.destinationPosition = teleportRequest.destinationPosition + xrOrigin.transform.parent.localPosition;
        LogTool.Log($"[XRTeleportationProvider] destination offset: {xrOrigin.transform.parent.localPosition}");
        return base.QueueTeleportRequest(teleportRequest);
    }

}
}
