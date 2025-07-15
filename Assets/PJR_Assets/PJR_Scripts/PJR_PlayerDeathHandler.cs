using Photon.Pun;
using UnityEngine;
using System.Collections;

public class PJR_PlayerDeathHandler : MonoBehaviourPun
{
    private bool isDead = false;

    private PJR_PlayerMovement movementScript;


    [SerializeField] public GameObject diePanel;
    [SerializeField] public float DieTime;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            if (diePanel != null)
                diePanel.SetActive(false);
            return;
        }

        movementScript = GetComponent<PJR_PlayerMovement>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine || isDead) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            Debug.Log("Touch Lava via Collision!!!");
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // 显示死亡面板
        if (diePanel != null)
            diePanel.SetActive(true);

        // 禁用控制脚本
        if (movementScript != null)
            movementScript.enabled = false;

        StartCoroutine(RespawnAfterDelay(DieTime));
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 隐藏死亡面板
        if (diePanel != null)
            diePanel.SetActive(false);

        // 重新启用控制脚本（不一定有意义，因为此对象马上销毁，但防止残留）
        if (movementScript != null)
            movementScript.enabled = true;

        // 主动销毁当前玩家（让其他人也能看到你被移除）
        PhotonNetwork.Destroy(gameObject);

        // 调用生成新玩家的逻辑
        LXGameSceneManager.Instance.RespawnPlayer();

    }
}
