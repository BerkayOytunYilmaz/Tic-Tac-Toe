using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CreateRoomManager : Singleton<CreateRoomManager>
{
    public GameObject CreateRoom;
    public Button CreateRoomButton;
    public Button CreateRoomCancelled;
    public static event Action CreateButtonClicked;
    public TMP_InputField roomName;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => CreateRoom.SetActive(true));
        CreateRoomButton.onClick.AddListener(() => CreateButtonClicked?.Invoke());
        CreateRoomCancelled.onClick.AddListener(() => CreateRoom.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {

    }


}
