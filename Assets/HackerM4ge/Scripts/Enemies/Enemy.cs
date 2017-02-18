using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Enemy : MonoBehaviour {

    public enum DamageType { Fire, Water, Earth, Wind, Physical, Elecrity, Love};

    abstract public void knockback(float strength);
    abstract public void slowDown(float strength, float duration);
    abstract public void damage(float strength, DamageType type);
   
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
