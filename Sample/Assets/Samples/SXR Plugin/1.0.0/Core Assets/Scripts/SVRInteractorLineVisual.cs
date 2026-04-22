using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SVRInteractorLineVisual : XRInteractorLineVisual
{
    [SerializeField]
    private float maxLineLength = 1.2f;
    [SerializeField]
    private float defaltLineLength = 0.8f;
    
    protected new void OnDisable()
    {
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
            lineLength = defaltLineLength;
        }
    }
}