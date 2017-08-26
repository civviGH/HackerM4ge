using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHealthAndMana : MonoBehaviour {
    protected Animator[] children;

	// Use this for initialization
	void Start ()
    {
        children = GetComponentsInChildren<Animator>();
        highlighted = new bool[children.Length];
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", false);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
