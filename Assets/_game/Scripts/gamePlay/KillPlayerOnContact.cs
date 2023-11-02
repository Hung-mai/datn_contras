using UnityEngine;
using System.Collections;
using Photon.Pun;

public class KillPlayerOnContact : MonoBehaviour {

    public bool killSelf;
    public bool KillRegardless;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == Constant.TAG_PLAYER)
        {
            PlayerController pController = other.GetComponent<PlayerController>();
            if(KillRegardless) pController.invincCounter = -1;
            pController.Death();
            if (killSelf) PhotonNetwork.Destroy(gameObject);
        }

    }
}
