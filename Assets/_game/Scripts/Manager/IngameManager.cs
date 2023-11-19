using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class IngameManager : MonoBehaviour
{
    public static IngameManager ins;
    public Transform spawnPoint;
    public PlayerController playerPrefab;
    public List<PlayerController> players = new List<PlayerController>();
    public int numPlayer = 0;
    public SpawnEnemyPoint[] spawnEnemyPoints;
    public CameraController cameraController;

    private void Awake() {
        ins = this;
    }

    private void Start()
    {
        
        // Debug.LogError("PhotonNetwork.IsMasterClient: " + PhotonNetwork.IsMasterClient);
        PlayerController player;
        if(PhotonNetwork.IsMasterClient)
        {
            player = PhotonNetwork.Instantiate("Game/Player", spawnPoint.position, Quaternion.identity).GetComponent<PlayerController>();
        }
        else
        {
            player = PhotonNetwork.Instantiate("Game/Player 2", spawnPoint.position, Quaternion.identity).GetComponent<PlayerController>();
        }

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
}
