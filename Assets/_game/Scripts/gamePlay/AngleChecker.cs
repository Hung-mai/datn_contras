using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleChecker : MonoBehaviour {

    // private PlayerController player;
    private float angle;
    private Vector2 A, B, C;
    private float minDis = 1000;
    private float dis;

    public PlayerController nearestPlayer;

    // private void Update()
    // {
    //     for (int i = 0; i < IngameManager.ins.players.Count; i++)
    //     {
    //         if(transform.position)
    //     }
    // }

    public float checkAngle()
    {
        minDis = 1000000;
        // tìm ra thằng player gần nhất
        for (int i = 0; i < IngameManager.ins.players.Count; i++)
        {
            if(IngameManager.ins.players[i] != null)
            {
                dis = (transform.position - IngameManager.ins.players[i].transform.position).sqrMagnitude;
                if( dis < minDis)
                {
                    minDis = dis;
                    nearestPlayer = IngameManager.ins.players[i];
                }
            }
        }

        A = new Vector2(transform.position.x, transform.position.y);
        B = new Vector2(nearestPlayer.transform.position.x, nearestPlayer.transform.position.y);
        C = B - A;

        angle = Mathf.Atan2(C.y, C.x) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 30) * 30;

        return angle;
    }

}
