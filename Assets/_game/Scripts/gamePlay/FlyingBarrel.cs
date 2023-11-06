using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBarrel : MonoBehaviour {

    public float movespeed;
    public float y;

    public float amplitude = 1;
    public float frequency = 1;
    private Rigidbody2D _rigidbody2D;

    // Use this for initialization
    private void Start ()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        y = transform.position.y;
    }
	
    public void Activate()
    {
        _rigidbody2D.AddRelativeForce(Vector2.right * movespeed, ForceMode2D.Impulse);
    }

	// Update is called once per frame
	private void Update ()
    {
        transform.position = new Vector3(transform.position.x, y + amplitude * Mathf.Sin(transform.position.x * frequency));
	}
}
