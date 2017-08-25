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
        Debug.Log ("showing: " + showing);
        Vector3 delta = Camera.main.transform.position - transform.position;
        Debug.Log("Delta.m: " + delta.magnitude);
        Debug.Log("minDistance: " + minDistance);
        if (delta.magnitude < minDistance)
        {
            if (showing) return;
            StartCoroutine("animate", true);
        }
        else
        {
            if (!showing) return;
            StartCoroutine("animate", false);
        }
    }

    private IEnumerator animate(bool to)
    {
        showing = to;
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", to);
            yield return new WaitForSeconds(delay);
        }
    }
}
