using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleIn : MonoBehaviour {

    public float minDistance = 2f;
    public bool showing = false;
    public float delay = 0.1f;
    public int randomCount = 4;

    protected Animator[] children;
    protected bool[] highlighted;

    // Use this for initialization
    void Start () {
        children = GetComponentsInChildren<Animator>();
        highlighted = new bool[children.Length];
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
            RandomizeSelectedChildren();
            StartCoroutine("ActivateRipple", true);
        }
        else
        {
            if (!showing) return;
            StartCoroutine("ActivateRipple", false);
        }
    }

    private void RandomizeSelectedChildren() {
        for(int i = 0; i < children.Length; i++)
        {
            highlighted[i] = false;
        }
        int randomChildren = 0;
        while(randomChildren < randomCount)
        {
            highlighted[Random.Range(0, children.Length)] = true;
            randomChildren = 0;
            for (int i = 0; i < children.Length; i++)
            {
                if (highlighted[i]) randomChildren++;
            }
        }
    }

    public IEnumerator ActivateRipple(bool state)
    {
        showing = state;
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", state);
            if (!state || highlighted[i]) children[i].SetBool("Highlighted", state);
            yield return new WaitForSeconds(delay);
        }
    }

}
