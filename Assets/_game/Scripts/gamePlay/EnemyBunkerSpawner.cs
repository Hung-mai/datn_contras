using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyBunkerSpawner : MonoBehaviour {

    private Animator anim;
    bool active;
    bool done;
    public GameObject bunker;

	// Use this for initialization
	private void Start ()
    {
        active = false;
        done = false;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	private void Update ()
    {
        anim.SetBool(Constant.anim_Active, active);
	}

    private void OnBecameVisible()
    {
        active = true;
    }

    public void SpawnBunker()
    {
        done = true;
        if (done) PhotonNetwork.Instantiate("Enemy/" + bunker.name, transform.position, transform.rotation);
        PhotonNetwork.Destroy(gameObject);
        
    }
}
