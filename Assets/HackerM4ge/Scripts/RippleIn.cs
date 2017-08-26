using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleIn : MonoBehaviour {

    public float minDistance = 2;
    public bool showing;
    public float delay = 0.1f;

    protected Animator[] children;

	// Use this for initialization
	void Start () {
        children = GetComponentsInChildren<Animator>();
        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", showing);
        }
    }

    // Update is called once per frame
    void Update () {
        Vector3 delta = Camera.main.transform.position - transform.position;
        if (delta.magnitude < minDistance)
        {
            if (showing) return;
            StartCoroutine("ActivateRipple", true);
        }
        else
        {
            if (!showing) return;
            StartCoroutine("ActivateRipple", false);
        }
    }

    public IEnumerator ActivateRipple(bool state)
    {
        showing = state;
        yield return new WaitForSecondsRealtime(delay);

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", state);
            yield return new WaitForSecondsRealtime(delay);
        }
    }

}
