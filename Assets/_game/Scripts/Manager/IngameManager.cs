using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class IngameManager : MonoBehaviourPunCallbacks
{
    public static IngameManager ins;
    public Transform spawnPoint;
    public PlayerController playerPrefab;
    public List<PlayerController> players = new List<PlayerController>();
    public int numPlayer = 0;
    public SpawnEnemyPoint[] spawnEnemyPoints;
    public CameraController cameraController;
    public Joystick joystick;
    PlayerController player;
    public GameObject obj_panelWin;
    public bool win = false;
    

    private void Awake() {
        ins = this;
    }

    private void Start()
    {
        
        // Debug.LogError("PhotonNetwork.IsMasterClient: " + PhotonNetwork.IsMasterClient);
        if(PhotonNetwork.IsMasterClient)
        {
            player = PhotonNetwork.Instantiate("Game/Player2", spawnPoint.position, Quaternion.identity).GetComponent<PlayerController>();
        }
        else
        {
            player = PhotonNetwork.Instantiate("Game/Player2", spawnPoint.position, Quaternion.identity).GetComponent<PlayerController>();
        }

        player.txt_name.text = PhotonNetwork.NickName;
        player._joystick = joystick;

        cameraController.player = player.transform;
        // player.SpawnPoint = spawnPoint.gameObject;
        // player.StartSetup();
        // players.Add(player);
        // umPlayer++;

        // chỉ master client vs đc phéo chạy hàm spawn lính
        if(PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < spawnEnemyPoints.Length; i++)
            {
                string nameOfPoint = spawnEnemyPoints[i].name.Split(' ')[0];
                PhotonNetwork.Instantiate("Enemy/" + nameOfPoint, spawnEnemyPoints[i].transform.position, Quaternion.identity);
            }
        }
    }

    public void Btn_jump()
    {
        player.KeyJump = true;
        StartCoroutine(offJump());
    }

    private IEnumerator offJump()
    {
        yield return Cache.GetWFS(0.1f);
        player.KeyJump = false;
    }

    public void Btn_outGame()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(Constant.SCENE_LOBBY);
    }
}
