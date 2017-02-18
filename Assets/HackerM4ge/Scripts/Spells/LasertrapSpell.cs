using System;
using UnityEngine;

class LasertrapSpell : MonoBehaviour, Spell
{
    private GameObject trapSource;
    private float trapSourceDistance = 0f;
    private GameObject trapTarget;
    private float trapTargetDistance = 0f;
    private GameObject laser;

    public LasertrapSpell()
    {
        trapSource = Instantiate(Resources.Load("LaserSource")) as GameObject;
    }

    string Spell.GetName()
    {
        return GetType().Name;
    }

    Material Spell.GetThumbnail()
    {
        return Resources.Load("LaserBeamHazard", typeof(Material)) as Material;
    }

    void Spell.UpdateSpell(TriggerState triggerState, Vector2 touchpadAxis, Vector3 wandPosition, Vector3 wandDirection)
    {
        if (triggerState.press && trapTarget == null)
        {
            UpdateTrapSource(ref trapSource, touchpadAxis, wandPosition, wandDirection);
        }
        else if (triggerState.press && trapTarget != null)
        {
            UpdateTrapSource(ref trapTarget, touchpadAxis, wandPosition, wandDirection);
        }

        if (triggerState.up && trapTarget == null)
        {
            trapTarget = Instantiate(Resources.Load("LaserSource")) as GameObject;
        }
        else if (triggerState.up && trapTarget != null)
        {
            laser = CreateLaser(trapSource.transform.position, trapTarget.transform.position);
        }
    }

    private void UpdateTrapSource(ref GameObject trapSource, Vector2 touchpadAxis, Vector3 wandPosition, Vector3 wandDirection)
    {
        float distance = touchpadAxis.y * Time.deltaTime;
        trapSourceDistance += distance;
        UpdateTrapSourcePosition(ref trapSource, trapSourceDistance, wandPosition, wandDirection);
    }

    private void UpdateTrapSourcePosition(ref GameObject trapSource, float trapSourceDistance, Vector3 wandPosition, Vector3 wandDirection)
    {
        trapSource.transform.position = wandPosition + wandDirection * trapSourceDistance;
    }

    private static GameObject CreateLaser(Vector3 from, Vector3 to)
    {
        GameObject line = new GameObject();
        line.AddComponent<LineRenderer>();
        line.GetComponent<LineRenderer>().numPositions = 1;
        line.GetComponent<LineRenderer>().SetPosition(0, from);
        line.GetComponent<LineRenderer>().SetPosition(1, to);
        line.GetComponent<LineRenderer>().startWidth = 0.1f;
        line.GetComponent<LineRenderer>().endWidth = 0.1f;
        line.GetComponent<LineRenderer>().startColor = Color.red;
        line.GetComponent<LineRenderer>().endColor = Color.red;
        line.GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));

        return line;
    }
}
