using System.Collections;
using UnityEngine;

public class KeySummonerSerial : MonoBehaviour {
    public float delay = 0.05f;
    public float minDistance = 2;
    public bool showing = false;

    private Animator[] children;

	// Use this for initialization
	void Start () {
        children = GetComponentsInChildren<Animator>();
    }

    // Update is called once per frame
    void Update() {
        //Debug.Log ("showing: " + showing);
        Vector3 delta = Camera.main.transform.position - transform.position;
        //Debug.Log ("Delta.m: " + delta.magnitude);
        if (delta.magnitude < minDistance)
        {
            if (showing) return;
            showing = true;
        }
        else
        {
            if (!showing) return;
            showing = false;
        }
        StartCoroutine(animateKeys(showing));
    }

    private IEnumerator animateKeys(bool state)
    {
        for(int i = 0; i < children.Length; i++)
        {
            Debug.LogFormat ("Time: " + Time.fixedTime + " | i:  " + i + " | " + state);
            children[i].SetBool("Shown", state);
            yield return new WaitForSeconds(delay);
        }
    }
}
