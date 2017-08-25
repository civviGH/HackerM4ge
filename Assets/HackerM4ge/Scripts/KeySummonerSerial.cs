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
            StartCoroutine("animateIn");
        }
        else
        {
            if (!showing) return;
            StartCoroutine("animateOut");
        }
    }

    private IEnumerator animateIn()
    {
        showing = true;
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", true);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator animateOut()
    {
        showing = false;
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", false);
            yield return new WaitForSeconds(delay);
        }
    }
}
