using Photon.Pun;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PJR_PlayerDeathHandler : MonoBehaviourPun
{
    private bool isDead = false;

    private PJR_PlayerMovement movementScript;


    [SerializeField] public GameObject diePanel;
    [SerializeField] public float DieTime;
    [SerializeField] private Animator animator;
    
    [SerializeField] private Text countdownText; 
    private void Start()
    {
        Debug.Log($"[{PhotonNetwork.NickName}] photonView.IsMine = {photonView.IsMine}");

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

        // 播放死亡动画
        if (animator != null)
            animator.SetTrigger("Die");

        // ��ʾ�������
        if (diePanel != null)
            diePanel.SetActive(true);

        // ���ÿ��ƽű�
        if (movementScript != null)
            movementScript.enabled = false;

        StartCoroutine(CountdownAndRespawn(DieTime));
    }


    IEnumerator CountdownAndRespawn(float delay)
    {
        float timeLeft = delay;

        while (timeLeft > 0)
        {
            if (countdownText != null)
                countdownText.text = $"Respawning in: {Mathf.CeilToInt(timeLeft)}s";
            yield return new WaitForSeconds(1f);
            timeLeft -= 1f;
        }

        if (countdownText != null)
            countdownText.text = "";

        if (diePanel != null)
            diePanel.SetActive(false);

        if (movementScript != null)
            movementScript.enabled = true;

        PhotonNetwork.Destroy(gameObject);
        LXGameSceneManager.Instance.RespawnPlayer();
    }

}
