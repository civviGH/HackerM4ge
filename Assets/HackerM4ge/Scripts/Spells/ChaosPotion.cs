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
                var transform = damagedEnemy.transform;
                transform.position += new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
                enemy.movement.DetourTo(transform);
            }
            breakingSound.transform.parent = null;
            breakingSound.GetComponent<AudioSource>().Play();
          
            poisonFogParticles.transform.parent = null;
            poisonFogParticles.GetComponent<ParticleSystem>().Play();

            breakingParticles.transform.parent = null;
            breakingParticles.GetComponent<ParticleSystem>().Play();

            Destroy(gameObject);
        }
    }
}
