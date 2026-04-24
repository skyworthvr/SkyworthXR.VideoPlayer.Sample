using UnityEngine;
using UnityEngine.InputSystem;

namespace Skyworth.XR.Interaction.Hands
{

public class HandInteractorManager : MonoBehaviour
{
    [SerializeField] private GameObject leftController;
    [SerializeField] private GameObject rightController;
        
    [SerializeField] private InputAction leftIsTracked;
    [SerializeField] private InputAction rightIsTracked;
        
    private void OnEnable()
    {
        leftIsTracked.Enable();
        rightIsTracked.Enable();
    }

    private void OnDisable()
    {
        leftIsTracked.Disable();
        rightIsTracked.Disable();
    }

    void Update()
    {
        leftController.SetActive(leftIsTracked.IsInProgress());
        rightController.SetActive(rightIsTracked.IsInProgress());
    }

}
}
