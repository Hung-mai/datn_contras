using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySniperRegular : MonoBehaviour {

    public AngleChecker angler;
    public Animator anim;


    // Use this for initialization
    // void Start () {
    //     angler = GetComponent<AngleChecker>();
    //     anim = GetComponent<Animator>();
	// }
	
	// Update is called once per frame
	private void Update()
    {
        anim.SetInteger(Constant.anim_angle, (int) angler.checkAngle());
        if(angler.nearestPlayer.transform.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1,1,1);
        } else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
	}
}
