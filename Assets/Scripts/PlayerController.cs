using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviour
{
    public GameManager.Keys playerKey;

    // Start is called before the first frame update
    void Start()
    {
        SetKeys();
    }
    public void SetKeys()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            GameManager.instance.player1 = gameObject;
        }
        else
        {
            GameManager.instance.player2 = gameObject;

        }

        if (PhotonNetwork.IsMasterClient)
        {
            playerKey = GameManager.Keys.Player1;
        }
        else
        {
            playerKey = GameManager.Keys.Player2;
            GameManager.instance.CanPlay(true);
        }
    }
    public void SetExitKeys()
    {

        GameManager.instance.player1 = gameObject;
        playerKey = GameManager.Keys.Player1;

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GetComponent<PhotonView>().IsMine && GameManager.instance.canPlay && playerKey == GameManager.instance.turn)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * 300, Color.green);

            if (Physics.Raycast(ray, out hit, 300))
            {
                if (hit.transform.CompareTag("Place"))
                {
                    GameManager.instance.SendData(int.Parse(hit.transform.name), playerKey);
                }
            }
        }
        if (GetComponent<PhotonView>().IsMine && GameManager.instance.canPlay && playerKey == GameManager.instance.turn)
        {
            GameManager.instance.yourTurn.SetActive(true);
        }
        else if (GetComponent<PhotonView>().IsMine)
        {
            GameManager.instance.yourTurn.SetActive(false);
        }


        if ((GetComponent<PhotonView>().IsMine && playerKey == GameManager.instance.turn) || !GameManager.instance.canPlay)
        {
            GameManager.instance.opponentsTurn.SetActive(false);
        }
        else if (GetComponent<PhotonView>().IsMine && GameManager.instance.canPlay)
        {
            GameManager.instance.opponentsTurn.SetActive(true);
        }
    }

    [PunRPC]
    public void Move(int hit, GameManager.Keys dataKey)
    {
        if (!GetComponent<PhotonView>().IsMine) return;
        GameManager.instance.places.transform.GetChild(hit - 1).GetComponent<Collider>().enabled = false;

        if (dataKey == GameManager.Keys.Player1)
        {
            GameManager.instance.places.transform.GetChild(hit - 1).GetChild(0).GetChild(0).gameObject.SetActive(true);
            GameManager.instance.turn = GameManager.Keys.Player2;
        }
        else if (dataKey == GameManager.Keys.Player2)
        {
            GameManager.instance.places.transform.GetChild(hit - 1).GetChild(0).GetChild(1).gameObject.SetActive(true);
            GameManager.instance.turn = GameManager.Keys.Player1;
        }

        GameManager.instance.MoveData[hit - 1] = (int)dataKey;
        GameManager.instance.CheckWinner();
    }
    [PunRPC]
    public void PlayActive(bool situation)
    {
        if (!GetComponent<PhotonView>().IsMine) return;
        GameManager.instance.canPlay = situation;
        GameManager.instance.waitingScreen.SetActive(!situation);
        if (situation) GameManager.instance.turn = GameManager.Keys.Player1;
        else GameManager.instance.turn = GameManager.Keys.None;
    }


}
