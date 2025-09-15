using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PhotonManager : MonoBehaviourPunCallbacks //PUN의 다양한 콜백 함수를 오버라이드해서 작성
{
    //게임 버전
    private readonly string version = "1.0";
    //유저 닉네임
    private string userId = "KSH35";

    void Awake()
    {
        //마스터 클라이언트(룸을 생성한 유저)의 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;

        //게임 버전 설정 (동일 버전의 유저끼리 컨넥팅)
        PhotonNetwork.GameVersion = version;

        //게임 유저의 닉네임 설정
        PhotonNetwork.NickName = userId;

        //포톤 서버와의 데이터의 초당 전송 횟수 (기본 30회)
        Debug.Log("PhotonNetwork.SendRate : " + PhotonNetwork.SendRate);

        //포톤 서버 접속
        Debug.Log("1) 포톤 서버 접속");
        PhotonNetwork.ConnectUsingSettings();
    }

    //포턴 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("2) 포턴 서버 접속 후 호출되는 콜백 함수");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}"); //자동 입장이 아니므로 false
        Debug.Log("3) 로비 입장 명령 OnJoinedLobby 호출");
        PhotonNetwork.JoinLobby(); //로비 입장 명령 OnJoinedLobby 호출
    }

    //로비에 접속 후 호출되는 콜백 함수
    //public override void OnJoinedLobby()
    //{
    //    Debug.Log("4) 로비 입장");
    //    Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");

    //    Debug.Log("5) 무작위로 선택한 룸에 입장");
    //    PhotonNetwork.JoinRandomRoom();
    //}

    public override void OnJoinedLobby()
    {
        Debug.Log("4) 로비 입장");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        Debug.Log("5) 무작위로 선택한 룸에 입장");
        PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤한 룸 입장이 실패했을 경우 호출되는 콜백 함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("룸에 입장 실패"); 
        Debug.Log($"JoinRandom Failed = {returnCode}:{message}");

        // 룸의 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;     //룸에 입장할 수 있는 최대 접속자 수
        ro.IsOpen = true;       //룸의 오픈 여부
        ro.IsVisible = true;    //로비에서 룸 목록에 노출시킬지 여부

        //룸 생성
        Debug.Log("6) 룸 생성");
        PhotonNetwork.CreateRoom("YS Room", ro);
    }

    //룸 생성 완료 후 호출되는 콜백 함수
    public override void OnCreatedRoom()
    {
        Debug.Log("룸 생성 완료 후 호출되는 콜백 함수");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    //룸에 입장한 후 호출되는 콜백 함수
    public override void OnJoinedRoom()
    {
        Debug.Log("룸에 입장한 후 호출되는 콜백 함수");
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }

        //출현 위치 정보를 배열에 저장
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        //네트워크상에 캐릭터 생성
        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0);

    }


}
