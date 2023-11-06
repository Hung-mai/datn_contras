using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FlyingBarrelTrigger : MonoBehaviour {

    public GameObject barrel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Constant.TAG_PLAYER && barrel != null)
        {
            barrel.GetComponent<FlyingBarrel>().y = transform.position.y;
            barrel.GetComponent<FlyingBarrel>().Activate();
        }
    }

    // Use this for initialization
    // void Start ()
    // {
		
	// }
	
	// Update is called once per frame
	private void Update ()
    {
        if (barrel == null) PhotonNetwork.Destroy(gameObject);
	}
}
