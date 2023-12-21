﻿using UnityEngine;
using System.Collections;

public class EnemyDeathEffect : MonoBehaviour {

    public float height;
    private int scale;

	// Use this for initialization
	private void Start ()
    {
        scale = Mathf.Clamp((int)(IngameManager.ins.players[0].transform.position.x*10000 - transform.position.x * 10000), -1, 1);
        transform.localScale = new Vector3(scale, 1, 1);
        if(GetComponent<Rigidbody2D>() != null)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(-scale*3, height);
        }

        SoundManager.PlayEfxSound(SoundManager.ins.enemyHit);
	}
	
	// // Update is called once per frame
	// void Update () {
	
	// }
}
