using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRightScale : MonoBehaviour
{
    public Transform parent;
    public Transform _transform;

    private void Update() {
        if(parent.transform.localScale.x == 1)
        {
            _transform.transform.localScale = Vector3.one;
        }
        else
        {
            _transform.transform.localScale = new Vector3(-1,1,1);
        }
    }
}
