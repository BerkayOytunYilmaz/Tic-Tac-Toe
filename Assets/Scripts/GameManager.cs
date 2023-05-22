using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    public enum Keys
    {
        None,
        Player1,
        Player2
    }
    public Keys turn;
    public bool canPlay;
    public List<int> MoveData;
    public GameObject player1;
    public GameObject player2;
    public GameObject places;
    public GameObject waitingScreen;
    public GameObject yourTurn;
    public GameObject opponentsTurn;
    public GameObject winnerParticle;
    public GameObject loserParticle;
    public GameObject drawParticle;
    public TextMeshPro player1Text;
    public TextMeshPro player2Text;
    public int player1Score;
    public int player2Score;
    // Start is called before the first frame update
    void Start()
    {
        turn = Keys.Player1;
        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        if (canPlay && (!player1 || !player2))
        {
            CanPlay(false);
            if (player1) player1.GetComponent<PlayerController>().SetExitKeys();
            RestartGame();
        }
    }
    public void SetText()
    {
        player1Text.text = player1Score.ToString();
        player2Text.text = player2Score.ToString();
    }
    public void CheckWinner()
    {
        int winner = CheckData();
        if (winner == 1)
        {
            turn = Keys.None;
            Invoke("ClearGame", 2);
            player1Score += 1;
            SetText();
            if ((int)player1.GetComponent<PlayerController>().playerKey == 1) { winnerParticle.SetActive(false); winnerParticle.SetActive(true); }
            else { loserParticle.transform.localScale = Vector3.zero; loserParticle.SetActive(true); loserParticle.transform.DOScale(Vector3.one, 1); }
        }
        else if (winner == 2)
        {
            turn = Keys.None;
            Invoke("ClearGame", 2);
            player2Score += 1;
            SetText();
            if ((int)player1.GetComponent<PlayerController>().playerKey == 2) { winnerParticle.SetActive(false); winnerParticle.SetActive(true); }
            else { loserParticle.transform.localScale = Vector3.zero; loserParticle.SetActive(true); loserParticle.transform.DOScale(Vector3.one, 1); }
        }
        else if (winner == 0 && CheckDraw())
        {
            turn = Keys.None;
            Invoke("ClearGame", 2);
            drawParticle.SetActive(true);
            drawParticle.transform.localScale = Vector3.zero;
            drawParticle.transform.DOScale(Vector3.one, 1);
        }
    }

    public int CheckData()
    {
        for (int i = 0; i < 9; i += 3)
        {
            if (MoveData[i] != 0 && ((MoveData[i] == MoveData[i + 1]) && (MoveData[i + 1] == MoveData[i + 2]))) return MoveData[i];
        }
        for (int i = 0; i < 3; i++)
        {
            if (MoveData[i] != 0 && ((MoveData[i] == MoveData[i + 3]) && (MoveData[i + 3] == MoveData[i + 6]))) return MoveData[i];
        }

        if (MoveData[0] != 0 && ((MoveData[0] == MoveData[4]) && (MoveData[4] == MoveData[8]))) return MoveData[0];
        if (MoveData[2] != 0 && ((MoveData[2] == MoveData[4]) && (MoveData[4] == MoveData[6]))) return MoveData[2];

        return 0;
    }
    public bool CheckDraw()
    {
        int draw = 0;
        for (int i = 0; i < MoveData.Count; i++)
        {
            if (MoveData[i] != 0) draw += 1;
        }
        if (draw == MoveData.Count) return true;
        else return false;
    }
    public void SendData(int hit, Keys playerKey)
    {
        if (player1) player1.GetComponent<PhotonView>().RPC("Move", RpcTarget.All, hit, playerKey);
        if (player2) player2.GetComponent<PhotonView>().RPC("Move", RpcTarget.All, hit, playerKey);
    }
    public void CanPlay(bool situation)
    {
        if (player1) player1.GetComponent<PhotonView>().RPC("PlayActive", RpcTarget.All, situation);
        if (player2) player2.GetComponent<PhotonView>().RPC("PlayActive", RpcTarget.All, situation);
    }
    public void ClearGame()
    {
        drawParticle.SetActive(false);
        loserParticle.SetActive(false);
        winnerParticle.SetActive(false);
        for (int i = 0; i < places.transform.childCount; i++)
        {
            places.transform.GetChild(i).GetComponent<Collider>().enabled = true;
            places.transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(false);
            places.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(false);
            MoveData[i] = 0;
        }
        turn = Keys.Player1;
    }
    public void RestartGame()
    {
        for (int i = 0; i < GameManager.instance.places.transform.childCount; i++)
        {
            places.transform.GetChild(i).GetComponent<Collider>().enabled = true;
            places.transform.GetChild(i).GetChild(0).GetChild(0).gameObject.SetActive(false);
            places.transform.GetChild(i).GetChild(0).GetChild(1).gameObject.SetActive(false);
            MoveData[i] = 0;
        }
        player1Score = 0;
        player2Score = 0;
        SetText();
    }
}
