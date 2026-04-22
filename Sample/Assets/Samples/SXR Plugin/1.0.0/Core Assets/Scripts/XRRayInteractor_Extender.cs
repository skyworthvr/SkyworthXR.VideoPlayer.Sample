using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;

public class XRRayInteractor_Extender : MonoBehaviour
{
    [SerializeField]
    private XRRayInteractor mXRRayInteractor;
    private RaycastResult mRaycastResult;

    void Update()
    {
#if SVR_V920
        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            UpdateGazeCasterPointDown();
        }
        if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2"))
        {
            UpdateGazeCasterPointUp();
        }
#endif
    }

    private void UpdateGazeCasterPointDown()
    {
        if (mXRRayInteractor.TryGetCurrentUIRaycastResult(out mRaycastResult))
        {
            PointerEventData mPointData = new PointerEventData(EventSystem.current);
            mPointData.pointerEnter = mRaycastResult.gameObject;
            IPointerDownHandler pointerDownHandler = mRaycastResult.gameObject.GetComponent<IPointerDownHandler>();
            if (pointerDownHandler != null)
            {
                pointerDownHandler.OnPointerDown(mPointData);
            }
        }
    }

    private void UpdateGazeCasterPointUp()
    {
        if (mXRRayInteractor.TryGetCurrentUIRaycastResult(out mRaycastResult))
        {
            Debug.Log($"mRaycastResult.name =>{mRaycastResult.gameObject.name}");
            PointerEventData mPointData = new PointerEventData(EventSystem.current);
            mPointData.pointerEnter = mRaycastResult.gameObject;
            IPointerClickHandler mPointerClickHandler = mRaycastResult.gameObject.GetComponent<IPointerClickHandler>();
            ISelectHandler mPointerSelectHandler = mRaycastResult.gameObject.GetComponent<ISelectHandler>();
            if (mPointerClickHandler == null)
                mPointerClickHandler = mRaycastResult.gameObject.GetComponentInParent<IPointerClickHandler>();
            if (mPointerClickHandler != null)
            {
                
                mPointerClickHandler.OnPointerClick(mPointData);
            }
            else if (mPointerSelectHandler != null)
            {
                IPointerUpHandler pointerUpHandler = mRaycastResult.gameObject.GetComponent<IPointerUpHandler>();
                mPointerSelectHandler.OnSelect(mPointData);
                if (pointerUpHandler != null)
                {
                    pointerUpHandler.OnPointerUp(mPointData);
                }
            }
        }
    }
}
