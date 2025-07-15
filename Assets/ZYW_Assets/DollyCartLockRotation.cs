using UnityEngine;
using Cinemachine;


[DefaultExecutionOrder(100)]
[RequireComponent(typeof(CinemachineDollyCart))]
public class DollyCartLockRotation : MonoBehaviour
{
    Quaternion _initialRot;

    void Awake()
    {
    
        _initialRot = transform.rotation;
    }

    void LateUpdate()
    {
        
        transform.rotation = _initialRot;
    }
}
