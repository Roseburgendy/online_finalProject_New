using UnityEngine;
using Photon.Pun;

public class FollowTransform : MonoBehaviourPun, IPunObservable
{
    private Transform targetTransform;
    private int targetPhotonViewId = -1; // 用于网络同步的目标ID

    public void SetTargetTransform(Transform targetTransform)
    {
        this.targetTransform = targetTransform;
        
        // 获取目标的PhotonView ID（如果有）
        if (targetTransform != null)
        {
            var targetView = targetTransform.GetComponent<PhotonView>();
            targetPhotonViewId = targetView != null ? targetView.ViewID : -1;
            
            // 同步给其他客户端
            if (photonView.IsMine)
            {
                photonView.RPC("RPC_SetTarget", RpcTarget.Others, targetPhotonViewId);
            }
        }
        else
        {
            targetPhotonViewId = -1;
        }
    }

    [PunRPC]
    private void RPC_SetTarget(int viewId)
    {
        targetPhotonViewId = viewId;
        if (viewId == -1)
        {
            targetTransform = null;
            return;
        }
        
        // 通过PhotonView查找目标
        var targetView = PhotonView.Find(viewId);
        if (targetView != null)
        {
            targetTransform = targetView.transform;
        }
    }

    private void LateUpdate()
    {
        if (targetTransform == null)
        {
            // 尝试重新查找目标（网络同步可能延迟）
            if (targetPhotonViewId != -1)
            {
                var targetView = PhotonView.Find(targetPhotonViewId);
                if (targetView != null)
                {
                    targetTransform = targetView.transform;
                }
            }
            return;
        }

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 发送当前目标ID
            stream.SendNext(targetPhotonViewId);
        }
        else
        {
            // 接收目标ID
            targetPhotonViewId = (int)stream.ReceiveNext();
            if (targetPhotonViewId != -1)
            {
                var targetView = PhotonView.Find(targetPhotonViewId);
                targetTransform = targetView != null ? targetView.transform : null;
            }
            else
            {
                targetTransform = null;
            }
        }
    }
}