using UnityEngine;
using Photon.Pun;

public class Fire : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;
    ParticleSystem muzzleFlash;
    PhotonView pv;
    
    // ���� ���콺 ��ư Ŭ�� �̺�Ʈ ����
    bool isMouseClick => Input.GetMouseButtonDown(0); // Landa Function
    
    void Start()
    {
        // ����� ������Ʈ ����
        pv = GetComponent<PhotonView>();
        // FirePos ������ �ִ� �ѱ� ȭ�� ȿ�� ����
        muzzleFlash = firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }
    void Update()
    {
        // ���� �������ο� ���콺 ���� ��ư�� Ŭ������ �� �Ѿ��� �߻�
        if (pv.IsMine && isMouseClick)
        {
            FireBullet();
            //RPC�� �������� �ִ� �Լ��� ȣ��
            pv.RPC("FireBullet", RpcTarget.Others, null);
        }
    }

    [PunRPC]
    void FireBullet()
    {
        // �ѱ�ȭ�� ȿ���� ���� ���� �ƴ� ��쿡 �ѱ� ȭ��ȿ�� ����
        if (!muzzleFlash.isPlaying) muzzleFlash.Play(true);
        GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
    }
}
