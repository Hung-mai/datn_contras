using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyProjectile : MonoBehaviour {

    public Rigidbody2D myRigidbody;
    public float movespeed;

    // Use this for initialization
    private void Start()
    {
        // myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.AddRelativeForce(Vector2.right * (movespeed + PlayerController.rapidsPicked * PlayerController.projectileSpeedKoeff), ForceMode2D.Impulse);

        StartCoroutine(waitToDestroy());
    }

    // void OnBecameInvisible()
    // {
    //     Destroy(gameObject);
    //     if (transform.parent != null) Destroy(transform.parent.gameObject);
    // }

    private IEnumerator waitToDestroy()
    {
        yield return Cache.GetWFS(5);
        PhotonNetwork.Destroy(gameObject);
    }
}
