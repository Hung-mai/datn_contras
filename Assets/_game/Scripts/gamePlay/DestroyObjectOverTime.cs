using UnityEngine;
using System.Collections;
using Photon.Pun;

public class DestroyObjectOverTime : MonoBehaviour {

    public float timeToDie;
    
	// void Update () {
    //     if (timeToDie <= 0)
    //     {
    //         Destroy(gameObject);
    //     }
    //     else timeToDie -= Time.deltaTime;
	// }

    private void Start()
    {
        StartCoroutine(waitToDestroy());
    }

    private IEnumerator waitToDestroy()
    {
        yield return Cache.GetWFS(timeToDie);
        PhotonNetwork.Destroy(gameObject);
    }
}
