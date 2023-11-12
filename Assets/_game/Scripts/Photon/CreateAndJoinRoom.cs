using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_Text txt_nickName;
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public GameObject obj_online;
    public GameObject obj_selectMode;
    public GameObject obj_joinRoom;
    public GameObject obj_changeNickName;
    public TMP_InputField input_nickName;
    public GameObject obj_invalid;
    public GameObject obj_btn_editName;
    public GameObject obj_room;
    private bool isPlayingAlone = false;
    public TMP_InputField inputJoinRoom;


    private void Start()
    {
        txt_nickName.text = "nickname: " +  PhotonNetwork.NickName;
    }

    public void Btn_CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        isPlayingAlone = false;
        PhotonNetwork.CreateRoom(Random.Range(100000,999999).ToString(), roomOptions);
    }

    public void Btn_JoinRoom()
    {
        isPlayingAlone = false;
        PhotonNetwork.JoinRoom(inputJoinRoom.text);
    }

    public void Btn_startGame()
    {
        PhotonNetwork.LoadLevel("Level_1");
    }

    

    public override void OnJoinedRoom()
    {
        if(isPlayingAlone)
        {
            PhotonNetwork.LoadLevel("Level_1");
        }
        else
        {
            obj_room.SetActive(true);
            txt_roomId.text = "room: " + PhotonNetwork.CurrentRoom.Name;

            if(PhotonNetwork.IsMasterClient)
            {
                obj_slotPlayer1.SetActive(true);
                txt_namePlayer1.text = PhotonNetwork.NickName;
                obj_slotPlayer2.SetActive(false);
                obj_btnStart.SetActive(true);
            }
            else
            {
                obj_btnStart.SetActive(false);
                
                foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
                {
                    if(PhotonNetwork.CurrentRoom.MasterClientId == player.Value.ActorNumber)
                    {
                        obj_slotPlayer1.SetActive(true);
                        txt_namePlayer1.text = player.Value.NickName;
                    }
                    else
                    {
                        obj_slotPlayer2.SetActive(true);
                        txt_namePlayer2.text = player.Value.NickName;
                    }
                }
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        obj_slotPlayer2.SetActive(true);
        txt_namePlayer2.text = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        obj_slotPlayer2.SetActive(false);
        txt_namePlayer1.text = PhotonNetwork.NickName;
        obj_btnStart.SetActive(true);
    }

    // button, 
    public void Btn_playAlone()
    {
        isPlayingAlone = true;
        PhotonNetwork.CreateRoom(Random.Range(10000,99999).ToString());
        // SceneManager.LoadScene("Level_1");
    }

    public void Btn_PlayerOnline()
    {
        obj_selectMode.SetActive(false);
        obj_online.SetActive(true);
        obj_btn_editName.SetActive(false);
    }

    public void Btn_backToMainMenu()
    {
        obj_btn_editName.SetActive(true);
        obj_online.SetActive(false);
        obj_selectMode.SetActive(true);
    }
    public void Btn_OpenJoinRoomFrame()
    {
        obj_joinRoom.SetActive(true);
    }
    public void Btn_closeFrameJoinRoom()
    {
        obj_joinRoom.SetActive(false);
    }

    public void Btn_openChangeNickName()
    {
        txt_nickName.text = "";
        obj_invalid.SetActive(false);
        obj_changeNickName.SetActive(true);
    }

    public void Btn_confirmNickName()
    {
        if(input_nickName.text.Trim() != string.Empty)
        {
            string nickName = input_nickName.text.Trim();
            PhotonNetwork.NickName = nickName;
            PlayerPrefs.SetString(Constant.nickName, nickName);

            obj_changeNickName.SetActive(false);
            txt_nickName.text = "nickname: " +  PhotonNetwork.NickName;

        }
        else
        {
            obj_invalid.SetActive(true);
        }
    }

    public void Btn_closeChangeNicknam()
    {
        obj_changeNickName.SetActive(false);
    }

    [Header("----------- room --------------")]
    public TMP_Text txt_roomId;
    public TMP_Text txt_namePlayer1;
    public TMP_Text txt_namePlayer2;
    public GameObject obj_slotPlayer1;
    public GameObject obj_slotPlayer2;
    public GameObject obj_btnStart;

    public void Btn_closeRoom()
    {
        PhotonNetwork.LeaveRoom();
        obj_room.SetActive(false);
    }

}
