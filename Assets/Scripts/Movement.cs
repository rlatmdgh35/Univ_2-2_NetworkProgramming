using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Cinemachine;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //PhotonView ������Ʈ ĳ�ø� ���� ���� ����
    private PhotonView pv;

    //�ó׸ӽ� ���� ī�޶� ������ ����
    private CinemachineCamera virtualCamera;

    // ������Ʈ ĳ�� ó���� ���� �Ӽ� ����
    // Declare properties for component cache processing
    CharacterController controller;
    new Transform transform;
    Animator animator;
    new Camera camera;

    // ������ Plane�� ����ĳ�����ϱ� ���� �Ӽ� ����
    // Declare properties for laycasting on a virtual plane
    Plane plane;
    Ray ray;
    Vector3 hitPoint;

    // �̵� �ӵ� �Ӽ�
    // Move Speed Properties
    public float moveSpeed = 10.0f;

    // ����� �̵� Ű���� �Է�
    // Input Player_move keyboard
    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    // Start is called before the first frame update
    void Start()
    {
        // ������Ʈ �ʱ�ȭ
        // Initialization Component
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;

        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindFirstObjectByType<CinemachineCamera>();

        //PhotonView�� �ڽ��� ���� ��� �ó׸ӽ� ����ī�޶� ����
        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

        // ������ �ٴ��� �÷��̾��� ��ġ�� ����
        // Create Virtual Plane by Player Location
        plane = new Plane(transform.up, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // �̵� ����
        // Implementation of movement
        Move();

        // ȸ�� ����
        // Implementation of rotation
        Turn();
    }

    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        // �̵��� ���� ���� ���
        // Calculate the direction vector to move
        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, 0.0f, moveDir.z);

        // �÷��̾� �̵�
        // Player move
        controller.SimpleMove(moveDir * moveSpeed);

        // �÷��̾� �ִϸ��̼� ����
        // Specify Player Animation
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    void Turn()
    {
        // ���콺�� 2���� ��ǥ�� �̿��� 3���� ���� ����
        // Create a 3D ray using the mouse's 2D coordinates
        ray = camera.ScreenPointToRay(Input.mousePosition);

        // �浹 �������� �Ÿ�
        // Distance from impact point
        float enter = 0.0f;

        // ������ �ٴڿ� ���̸� �߻��� �浹�� ������ �Ÿ��� enter�� �Ҵ�
        // Fire a ray on the virtual floor to assign the distance of the impact point to the enter
        plane.Raycast(ray, out enter);

        // ������ �ٴڿ� ���̰� �浹�� ��ǥ ����
        // Extract coordinates where the ray hit the virtual floor
        hitPoint = ray.GetPoint(enter);

        // ȸ���ؾ� �� ������ ���͸� ���
        // Calculate the vector in the direction you need to rotate
        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;

        // �÷��̾��� ȸ���� ����
        // Specify the rotation value of the player
        transform.localRotation = Quaternion.LookRotation(lookDir);

    }
}
