using UnityEngine;
using Cinemachine;


[DefaultExecutionOrder(100)]
[RequireComponent(typeof(CinemachineDollyCart))]
public class ZYW_DollyCartLockRotation : MonoBehaviour
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
