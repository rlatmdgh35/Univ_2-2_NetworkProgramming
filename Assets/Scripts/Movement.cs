using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Cinemachine;
using UnityEngine;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    //PhotonView 컴포넌트 캐시를 위한 변수 선언
    private PhotonView pv;

    //시네머신 가상 카메라를 저장할 변수
    private CinemachineCamera virtualCamera;

    // 컴포넌트 캐시 처리를 위한 속성 선언
    // Declare properties for component cache processing
    CharacterController controller;
    new Transform transform;
    Animator animator;
    new Camera camera;

    // 가상의 Plane에 레이캐스팅하기 위한 속성 선언
    // Declare properties for laycasting on a virtual plane
    Plane plane;
    Ray ray;
    Vector3 hitPoint;

    // 이동 속도 속성
    // Move Speed Properties
    public float moveSpeed = 10.0f;

    // 수신된 위치와 회전값을 저장할 변수
    Vector3 receiveLoc;
    Quaternion receiveRot;

    // 수신된 좌표로 이동 및 회전 속도의 민감도
    public float damping = 10f;

    // 사용자 이동 키보드 입력
    // Input Player_move keyboard
    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    // Start is called before the first frame update
    void Start()
    {
        // 컴포넌트 초기화
        // Initialization Component
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;

        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindFirstObjectByType<CinemachineCamera>();

        //PhotonView가 자신의 것일 경우 시네머신 가상카메라를 연결
        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

        // 가상의 바닥을 플레이어의 위치로 생성
        // Create Virtual Plane by Player Location
        plane = new Plane(transform.up, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        // PhotonView에 의해 네트워크의 모든 유저 객체의 위치, 회전값 및 애니메이션이 동기화 됨
        // 따라서 자신이 생성한 네트워크 객체(player)만 컨트롤 함
        if (pv.IsMine)
        {
            //이동 구현
            Move();
            //회전 구현
            Turn();
        }
        else
        {
            // 수신된 좌표로 보간한 이동처리
            transform.position = Vector3.Lerp(transform.position, receiveLoc, Time.deltaTime * damping);
            // 수신된 회전값으로 보간한 회전처리
            transform.rotation = Quaternion.Slerp(transform.rotation, receiveRot, Time.deltaTime * damping);
        }
    }

    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        // 이동할 방향 벡터 계산
        // Calculate the direction vector to move
        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, 0.0f, moveDir.z);

        // 플레이어 이동
        // Player move
        controller.SimpleMove(moveDir * moveSpeed);

        // 플레이어 애니메이션 지정
        // Specify Player Animation
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    void Turn()
    {
        // 마우스의 2차원 좌표를 이용해 3차원 레이 생성
        // Create a 3D ray using the mouse's 2D coordinates
        ray = camera.ScreenPointToRay(Input.mousePosition);

        // 충돌 지점과의 거리
        // Distance from impact point
        float enter = 0.0f;

        // 가상의 바닥에 레이를 발사해 충돌한 지점의 거리를 enter에 할당
        // Fire a ray on the virtual floor to assign the distance of the impact point to the enter
        plane.Raycast(ray, out enter);

        // 가상의 바닥에 레이가 충돌한 좌표 추출
        // Extract coordinates where the ray hit the virtual floor
        hitPoint = ray.GetPoint(enter);

        // 회전해야 할 방향의 벡터를 계산
        // Calculate the vector in the direction you need to rotate
        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;

        // 플레이어의 회전값 지정
        // Specify the rotation value of the player
        transform.localRotation = Quaternion.LookRotation(lookDir);

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 자신의 로컬 캐릭터인 경우 자신의 데이터를 다른 네트워크 유저에게 송신
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            receiveLoc = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
