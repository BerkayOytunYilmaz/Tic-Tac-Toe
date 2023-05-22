using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConnectMaster : MonoBehaviourPunCallbacks
{
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public Transform content;
    public GameObject roomInfoPrefab;

    public List<GameObject> InfoList;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        ButtonClick.joinButtonClicked += JoinSelectedRoom;
        CreateRoomManager.CreateButtonClicked += CreateRoomFunc;
    }
    public override void OnDisable()
    {
        ButtonClick.joinButtonClicked -= JoinSelectedRoom;
        CreateRoomManager.CreateButtonClicked -= CreateRoomFunc;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void OnConnectedToMaster()
    {
        print("Connected");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        print("joinedLobby");
        cachedRoomList.Clear();
        //PhotonNetwork.JoinOrCreateRoom("Room 1", new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true }, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        ClearList();
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0, null);
        GameManager.instance.RestartGame();
    }
    public override void OnCreatedRoom()
    {
        print("oda kuruldu");
        GameManager.instance.waitingScreen.SetActive(true);
        //PhotonNetwork.Instantiate("GameManager", Vector3.zero, Quaternion.identity, 0, null);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            print(roomList[0].PlayerCount);
            MakeRoomList(info);
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    void MakeRoomList(RoomInfo currentRoom)
    {
        int counter = InfoList.Count;
        for (int i = 0; i < counter; i++)
        {
            Destroy(InfoList[i]);
        }
        InfoList.Clear();
        if (currentRoom.PlayerCount == 0) return;
        GameObject room = Instantiate(roomInfoPrefab, content);
        InfoList.Add(room);
        room.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentRoom.Name;
        room.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentRoom.PlayerCount + "/" + currentRoom.MaxPlayers;
        if (currentRoom.PlayerCount == 2) room.transform.GetChild(2).GetComponent<Button>().interactable = false;
    }

    void JoinSelectedRoom()
    {
        string roomName = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        for (int i = 0; i < InfoList.Count; i++)
        {
            InfoList[i].GetComponentInChildren<Button>().interactable = false;
        }
        PhotonNetwork.JoinRoom(roomName);
        gameObject.SetActive(false);
    }

    public void ClearList()
    {
        for (int i = 0; i < InfoList.Count; i++)
        {
            Destroy(InfoList[0].gameObject);
            InfoList.RemoveAt(0);
        }
        cachedRoomList.Clear();
    }

    public void CreateRoomFunc()
    {
        string roomName;
        if (!cachedRoomList.ContainsKey(CreateRoomManager.instance.roomName.text))
            roomName = CreateRoomManager.instance.roomName.text;
        else
        {
            int i = 1;
            while (cachedRoomList.ContainsKey("Room " + i))
            {
                i += 1;
            }
            roomName = "Room " + i;
        }

        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2, IsOpen = true, IsVisible = true }, TypedLobby.Default);
        gameObject.SetActive(false);
    }
}
