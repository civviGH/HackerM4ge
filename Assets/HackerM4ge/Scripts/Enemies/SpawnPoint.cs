using UnityEngine;

public class SpawnPoint : MonoBehaviour {
    public GameObject spiderPrefab;
    public GameObject virusPrefab;
    public int spiderCount;
    public int virusCount;

    public int GetTotalCount()
    {
        return spiderCount + virusCount;
    }

    public void SpawnRandom(Transform[] destinations)
    {
        int rand = Random.Range(0, virusCount + spiderCount);
        GameObject newEnemy;
        if (rand < virusCount)
        {
            newEnemy = Instantiate(virusPrefab, transform.position, transform.rotation);
            virusCount--;
        }
        else
        {
            newEnemy = Instantiate(spiderPrefab, transform.position, transform.rotation);
            spiderCount--;
        }
        EnemyMovement enemyMovement = newEnemy.GetComponent<EnemyMovement>();
        enemyMovement.SetDestinations(destinations);
        enemyMovement.SetEnemy(newEnemy.gameObject.GetComponent<Enemy>());
    }
}
