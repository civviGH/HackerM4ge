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
        Vector3 delta = Camera.main.transform.position - transform.position;
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
        animateKeys(showing);
    }

    private IEnumerable animateKeys(bool state)
    {
        for(int i = 0; i < children.Length; i++)
        {
            children[i].SetBool("Shown", state);
            yield return new WaitForSeconds(delay);
        }
    }
}
