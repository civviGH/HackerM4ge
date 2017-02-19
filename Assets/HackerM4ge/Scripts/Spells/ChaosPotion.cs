using UnityEngine;

public class ChaosPotion : MonoBehaviour {
    LayerMask layerMask;
    public GameObject breakingSound;
    public GameObject breakingParticles;
    public GameObject vaporParticles;
    public GameObject shrapnelParticles;

    void Start()
    {
        layerMask = LayerMask.NameToLayer("Surfaces");
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == layerMask)
        {
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

            shrapnelParticles.transform.parent = null;
            shrapnelParticles.GetComponent<ParticleSystem>().Play();

            vaporParticles.transform.parent = null;
            vaporParticles.GetComponent<ParticleSystem>().Play();

            breakingParticles.transform.parent = null;
            breakingParticles.GetComponent<ParticleSystem>().Play();

            Destroy(this);
        }
    }
}
