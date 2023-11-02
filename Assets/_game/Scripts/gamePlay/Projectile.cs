using UnityEngine;
using System.Collections;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float movespeed;
    public float spinningSpeed;

	// Use this for initialization
	private void Start ()
    {
        // myRigidbody = GetComponent<Rigidbody2D>();
        myRigidbody.AddRelativeForce(Vector2.up * (movespeed + PlayerController.rapidsPicked*PlayerController.projectileSpeedKoeff), ForceMode2D.Impulse);
        myRigidbody.angularVelocity = spinningSpeed;

        
        StartCoroutine(waitToDestroy());
	}

    private IEnumerator waitToDestroy()
    {
        yield return Cache.GetWFS(2);
        PhotonNetwork.Destroy(gameObject);
    }


    // void Update()
    // {
    //     Renderer r = GetComponent<SpriteRenderer>();
    //     if(!r.isVisible) Destroy(gameObject);
    // }

	
	//void OnBecameInvisible()
 //   {
 //       Destroy(gameObject);
 //       if (transform.parent != null) Destroy(transform.parent.gameObject);
 //   }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == Constant.TAG_ENEMY)
        {
            if(other.GetComponent<EnemyManager>() != null)
            {
                other.GetComponent<EnemyManager>().TakeDamage();
                PhotonNetwork.Destroy(gameObject);
                if (transform.parent != null) PhotonNetwork.Destroy(transform.parent.gameObject);
            }
        }
    }
}
