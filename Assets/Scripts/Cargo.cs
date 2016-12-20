using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour, IConsoleIteration {

	void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.tag == "LaserSource")
		{
			// Disable whole parentlaser if Cargo collides with LaserSource
			// other.gameObject = LaserSource GO
			// other.gameObject.transform.parent.gameObject = LaserParent GO
			other.gameObject.transform.parent.gameObject.SetActive(false);
		}
	}

	public void Activate_Once()
	{
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
    }
}
