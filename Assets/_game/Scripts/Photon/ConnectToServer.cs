using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public GameObject obj_nickName;
    public GameObject obj_loading;
    public GameObject obj_connecting;
    public GameObject obj_invalid;
    public TMP_InputField txt_nameInput;
    public TMP_Text txt_connecting;


    private IEnumerator Start()
    {
        
        Application.targetFrameRate = 240;
        PhotonNetwork.AutomaticallySyncScene = true;

        if(PlayerPrefs.HasKey(Constant.nickName) == false)
        {
            yield return Cache.GetWFS(1.5f);
            obj_loading.SetActive(false);
            obj_nickName.SetActive(true);
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString(Constant.nickName);
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void Btn_connect()
    {
        if(txt_nameInput.text.Trim() != string.Empty)
        {
            string nickName = txt_nameInput.text.Trim();
            PhotonNetwork.NickName = nickName;
            PlayerPrefs.SetString(Constant.nickName, nickName);

            obj_connecting.SetActive(true);
            obj_nickName.SetActive(false);
            StartCoroutine(ie_connecting());

            PhotonNetwork.ConnectUsingSettings();

        }
        else
        {
            obj_invalid.SetActive(true);
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(Constant.SCENE_LOBBY);
    }

    private IEnumerator ie_connecting()
    {
        for (int i = 0; i < 100; i++)
        {
            if(i % 4 == 0)
            {
                txt_connecting.text = "connecting";
            }
            else if(i % 4 == 1)
            {
                txt_connecting.text = "connecting.";
            }
            else if(i % 4 == 2)
            {
                txt_connecting.text = "connecting..";
            }
            else
            {
                txt_connecting.text = "connecting...";
            }
            yield return Cache.GetWFS(0.15f);
        }
    }
}
