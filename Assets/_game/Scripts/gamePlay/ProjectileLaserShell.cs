using UnityEngine;
using System.Collections;
using Photon.Pun;

public class ProjectileLaserShell : MonoBehaviour {

    public GameObject projectile;
    public float delay;
    private float delayCounter;
    public int projectilesCount;

	// Use this for initialization
	private void Start () {
        PhotonNetwork.Instantiate("Game/" + projectile.name, transform.position, transform.rotation);
        projectilesCount--;
        delayCounter = delay;
	}
	
	// Update is called once per frame
	private void Update () {
	    if(projectilesCount > 0 && delayCounter <=0)
        {
            PhotonNetwork.Instantiate("Game/" + projectile.name, transform.position, transform.rotation);
            projectilesCount--;
            delayCounter = delay;
        } else
        {
            delayCounter -= Time.deltaTime;
        }

        if (projectilesCount <= 0)PhotonNetwork. Destroy(gameObject);
	}
}
