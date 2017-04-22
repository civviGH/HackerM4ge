using UnityEngine;

public class ChaosPotion : MonoBehaviour {
    LayerMask layerMask;
    public GameObject breakingSound;
    public GameObject breakingParticles;
    public GameObject poisonFogParticles;

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
                Enemy enemy = damagedEnemy.transform.gameObject.GetComponent<Enemy>();
                enemy.movement.DetourTo(damagedEnemy.transform.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            }
            breakingSound.transform.parent = null;
            breakingSound.GetComponent<AudioSource>().Play();
          
            poisonFogParticles.transform.parent = null;
            poisonFogParticles.GetComponent<ParticleSystem>().Play();

            breakingParticles.transform.parent = null;
            breakingParticles.GetComponent<ParticleSystem>().Play();

            Destroy(breakingSound, 0.5f);
            Destroy(breakingParticles, 2f);
            Destroy(poisonFogParticles, 2f);
            Destroy(gameObject);
        }
    }
}
