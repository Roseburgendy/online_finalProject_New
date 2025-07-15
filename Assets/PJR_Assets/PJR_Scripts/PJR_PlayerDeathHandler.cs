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

        // ��ʾ�������
        if (diePanel != null)
            diePanel.SetActive(true);

        // ���ÿ��ƽű�
        if (movementScript != null)
            movementScript.enabled = false;

        StartCoroutine(RespawnAfterDelay(DieTime));
    }

    IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // �����������
        if (diePanel != null)
            diePanel.SetActive(false);

        // �������ÿ��ƽű�����һ�������壬��Ϊ�˶����������٣�����ֹ������
        if (movementScript != null)
            movementScript.enabled = true;

        // �������ٵ�ǰ��ң���������Ҳ�ܿ����㱻�Ƴ���
        PhotonNetwork.Destroy(gameObject);

        // ������������ҵ��߼�
        LXGameSceneManager.Instance.RespawnPlayer();

    }
}
