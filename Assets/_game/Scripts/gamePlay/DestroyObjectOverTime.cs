using UnityEngine;
using System.Collections;
using Photon.Pun;


public class DestroyObjectOverTime : MonoBehaviour {

    public float timeToDie;
    public PhotonView photonView;
    
	// void Update () {
    //     if (timeToDie <= 0)
    //     {
    //         Destroy(gameObject);
    //     }
    //     else timeToDie -= Time.deltaTime;
	// }

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        StartCoroutine(waitToDestroy());
    }

    private IEnumerator waitToDestroy()
    {
        yield return Cache.GetWFS(timeToDie);
        if(photonView != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
