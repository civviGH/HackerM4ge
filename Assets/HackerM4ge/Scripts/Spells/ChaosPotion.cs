using UnityEngine;

public class ChaosPotion : MonoBehaviour {
    LayerMask layerMask;
    public GameObject breakingSound;
    public GameObject breakingParticles;
    public GameObject poisonFogParticles;

    void Start()
    {
        layerMask = LayerMask.GetMask("Surfaces");
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log ("ontrigger enter");
        if (collider.gameObject.layer.ToString() == "Surfaces")
        {
            Debug.Log ("layermask hit");
            Collider[] damagedEnemies = Physics.OverlapSphere(
                gameObject.transform.position,
                5f,
                LayerMask.GetMask("Damagable")
            );

            foreach (Collider damagedEnemy in damagedEnemies)
            {
                // TODO 
                // damagedEnemy.transform.gameObject.GetComponent<Enemy>().damage(100f, Enemy.DamageType.Fire);
            }
            breakingSound.transform.parent = null;
            breakingSound.GetComponent<AudioSource>().Play();
          
            poisonFogParticles.transform.parent = null;
            poisonFogParticles.GetComponent<ParticleSystem>().Play();

            breakingParticles.transform.parent = null;
            breakingParticles.GetComponent<ParticleSystem>().Play();

            Destroy(this);
        }
    }
}
