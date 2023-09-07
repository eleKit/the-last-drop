using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTrees : MonoBehaviour
{

	[Header ("Wood part of the tree")]
	public SpriteRenderer[] wood;

	[Header ("Leafs part of the tree")]
	public SpriteRenderer[] leaf;
	
	[Header ("Water")]
	public SpriteRenderer[] water;

		

	public void ColorTree ()
	{
		
		//color the leafs
		for (int i = 0; i < leaf.Length; i++) {
			leaf [i].color = new Color32 (202, 244, 82, 255);
			Debug.Log ("Color the leaf" + leaf [i].color.ToString ());
		}

		// color the woody parts of the tree
		for (int i = 0; i < wood.Length; i++) {
			wood [i].color = new Color32 (238, 207, 168, 255);  
		}
		
		for (int i = 0; i < water.Length; i++)
		{
			water[i].color = Color.white;
		}
	}
}
