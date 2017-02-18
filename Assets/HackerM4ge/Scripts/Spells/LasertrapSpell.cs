using UnityEngine;

class LasertrapSpell : MonoBehaviour, Spell
{
    private UnityEngine.Object laserSourcePrefab;
    private Material laserBeamHazardMaterial;

    private GameObject trapSource;
    private float trapSourceDistance;
    private GameObject trapTarget;
    private float trapTargetDistance;

    public LasertrapSpell()
    {
        laserSourcePrefab = Resources.Load("LaserTrapSpellPrefabs/LaserSource");
        laserBeamHazardMaterial = Resources.Load("LaserTrapSpellPrefabs/LaserBeamHazard", typeof(Material)) as Material;
        Reset();
    }

    private void Reset()
    {
        trapSource = Instantiate(laserSourcePrefab) as GameObject;
        trapTarget = null;
        trapSourceDistance = 0.2f;
        trapTargetDistance = 0.2f;
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
            Destroy(trapSource, 5f);
            Destroy(trapTarget, 5f);
            Destroy(laser, 5f);

            Reset();
        }
    }

    private void UpdateTrapSource(ref GameObject trapSource, Vector2 touchpadAxis, Vector3 wandPosition, Vector3 wandDirection)
    {
        float distance = touchpadAxis.y * Time.deltaTime * 5;
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
        line.GetComponent<LineRenderer>().numPositions = 2;
        line.GetComponent<LineRenderer>().SetPosition(0, from);
        line.GetComponent<LineRenderer>().SetPosition(1, to);
        line.GetComponent<LineRenderer>().startWidth = 0.01f;
        line.GetComponent<LineRenderer>().endWidth = 0.01f;
        line.GetComponent<LineRenderer>().startColor = Color.red;
        line.GetComponent<LineRenderer>().endColor = Color.red;
        line.GetComponent<LineRenderer>().material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));

        return line;
    }
}
