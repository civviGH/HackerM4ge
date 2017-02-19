using System;
using UnityEngine;

class LasertrapSpell : MonoBehaviour, Spell
{
    const float minimalDistance = 0.2f;
    const float maxSpeed = 15f;
    const float lifetime = 20f;

    private UnityEngine.Object laserSourcePrefab;
    private Material laserBeamHazardMaterial;
    private LayerMask surfaceLayer;

    private GameObject trapSource;
    private GameObject trapTarget;
    private float trapSourceDistance;

    public LasertrapSpell()
    {
        laserSourcePrefab = Resources.Load("LaserTrapSpellPrefabs/LaserSource");
        laserBeamHazardMaterial = Resources.Load("LaserTrapSpellPrefabs/LaserBeamHazard", typeof(Material)) as Material;
        surfaceLayer = LayerMask.GetMask("Surfaces");

        trapSourceDistance = minimalDistance;
    }

    private void Init()
    {
        trapSource = Instantiate(laserSourcePrefab) as GameObject;
        trapTarget = null;
    }

    string Spell.GetName()
    {
        return GetType().Name;
    }

    Material Spell.GetThumbnail()
    {
        return laserBeamHazardMaterial;
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
            trapTarget = Instantiate(Resources.Load("LaserTrapSpellPrefabs/LaserSource")) as GameObject;
        }
        else if (triggerState.up && trapTarget != null)
        {
            GameObject laser = CreateLaser(trapSource.transform.position, trapTarget.transform.position);
            Destroy(trapSource, lifetime);
            Destroy(trapTarget, lifetime);
            Destroy(laser, lifetime);

            Init();
        }
    }

    private void UpdateTrapSource(ref GameObject trapSource, Vector2 touchpadAxis, Vector3 wandPosition, Vector3 wandDirection)
    {
        float distance = Math.Sign(touchpadAxis.y) * touchpadAxis.y * touchpadAxis.y * Time.deltaTime * maxSpeed;
        trapSourceDistance += distance;
        if(trapSourceDistance < minimalDistance)
        {
            trapSourceDistance = minimalDistance;
        }
        UpdateTrapSourcePosition(ref trapSource, trapSourceDistance, wandPosition, wandDirection);
    }

    private void UpdateTrapSourcePosition(ref GameObject trapSource, float trapSourceDistance, Vector3 wandPosition, Vector3 wandDirection)
    {
        RaycastHit hitObject;
        Ray ray = new Ray(wandPosition, wandDirection);

        float currentDistance = trapSourceDistance;
        if (Physics.Raycast(ray, out hitObject, trapSourceDistance, surfaceLayer))
        {
            currentDistance = Math.Max(hitObject.distance - 0.5f, minimalDistance);
        }

        trapSource.transform.position = wandPosition + wandDirection * currentDistance;
    }

    private static GameObject CreateLaser(Vector3 from, Vector3 to)
    {
        GameObject laser = new GameObject();
        laser.AddComponent<LineRenderer>();
        laser.GetComponent<LineRenderer>().numPositions = 2;
        laser.GetComponent<LineRenderer>().SetPosition(0, from);
        laser.GetComponent<LineRenderer>().SetPosition(1, to);
        laser.GetComponent<LineRenderer>().startWidth = 0.01f;
        laser.GetComponent<LineRenderer>().endWidth = 0.01f;
        laser.GetComponent<LineRenderer>().startColor = Color.red;
        laser.GetComponent<LineRenderer>().endColor = Color.red;
        laser.GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));

        laser.AddComponent<LaserRay>();
        laser.GetComponent<LaserRay>().from = from;
        laser.GetComponent<LaserRay>().to = to;

        return laser;
    }

    void Spell.Select()
    {
        Init();
    }

    void Spell.Deselect()
    {
        Destroy(trapSource);
        Destroy(trapTarget);
        trapSource = null;
        trapTarget = null;
    }
}
