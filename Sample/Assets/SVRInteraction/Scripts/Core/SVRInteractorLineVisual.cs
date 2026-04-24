using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Skyworth.XR.Interaction
{

public class SVRInteractorLineVisual : XRInteractorLineVisual
{
    [SerializeField]
    private float maxLineLength = 1.2f;
    [SerializeField]
    private float defaultLineLength = 0.8f;
    
    protected new void OnDisable()
    {
        LogTool.Log($"SVRInteractorLineVisual => Disabling {gameObject.name} line visual");
        if (reticle != null)
        {
            reticle.SetActive(false);
        }
        base.OnDisable();
    }

    private void LateUpdate()
    {
        if (reticle != null && reticle.activeSelf)
        {
            float distance = Vector3.Distance(this.transform.position, reticle.transform.position);
            lineLength = Mathf.Min(distance * 0.66f, maxLineLength) ;
        }
        else
        {
            lineLength = defaultLineLength;
        }
    }
}
}