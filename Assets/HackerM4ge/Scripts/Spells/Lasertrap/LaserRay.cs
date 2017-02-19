using UnityEngine;

public class LaserRay : MonoBehaviour
{
    const float dps = 40f;
    public Vector3 from { get; set; }
    public Vector3 to { get; set; }
    private LayerMask damagableLayer;

    void Start()
    {
        damagableLayer = LayerMask.GetMask("Damagable");
    }

    void Update()
    {
        if(from == null || to == null)
        {
            return;
        }
        RaycastHit hitObject;
        Ray ray = new Ray(from, to-from);
        float maxDistance = (to - from).magnitude;
        if (Physics.Raycast(ray, out hitObject, maxDistance, damagableLayer))
        {
            Enemy enemy = hitObject.collider.transform.gameObject.GetComponent<Enemy>();
            enemy.damage(Time.deltaTime * dps, Enemy.DamageType.Light);
        }
    }
}
