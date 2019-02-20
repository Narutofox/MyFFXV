using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using UnityEngine;
using UnityEngine.AI;

public class NightEnemySpawnPoint : MonoBehaviour
{
    public  GameObject[] EnemiesThatCanBeSpawend;
    public  IList<GameObject> _enemiesThatCanBeSpawend;
    private IList<GameObject> _spawendEnemies;
    private bool _canSpawn;
    private int _numberOfSpawnCycles;
    void Start()
    {
        _spawendEnemies = new List<GameObject>();
        _enemiesThatCanBeSpawend = new List<GameObject>();
        _canSpawn = true;
        _numberOfSpawnCycles = 1;

        foreach (GameObject enemy in EnemiesThatCanBeSpawend)
        {
            if (enemy != null
                && enemy.GetComponent<Damageable>() != null                                   
                && enemy.GetComponent<EnemyController>() != null)
            {
               
                enemy.GetComponent<Damageable>().maxHitPoints += 3;
                enemy.GetComponent<Damageable>().ResetDamage();
                enemy.GetComponent<NavMeshAgent>().enabled = true;
                _enemiesThatCanBeSpawend.Add(enemy);
            }
        }
    }

    public void DestroySpawendEnemies()
    {
        Damageable.DamageMessage data = new Damageable.DamageMessage();

        foreach (var spawendEnemy in _spawendEnemies)
        {
            if (spawendEnemy != null)
            {
                Damageable damageable = spawendEnemy.GetComponent<Damageable>();
                damageable.isInvulnerable = false;
                data.amount = damageable.currentHitPoints;
                data.damageSource = Vector3.zero;
                damageable.ApplyDamage(data);
            }
            
        }
        _spawendEnemies.Clear();
        _canSpawn = true;
    }

    public void SpawnEnemies()
    {
        if (!_canSpawn || EnemiesThatCanBeSpawend == null || EnemiesThatCanBeSpawend.Length <= 0  || _spawendEnemies.Count > 0) return;

        Vector3 center = transform.position;
        int numberOfSpawnCycles = 0;
        _numberOfSpawnCycles = Random.Range(1, 4);

        while (_numberOfSpawnCycles <= numberOfSpawnCycles)
        {
            foreach (GameObject enemy in _enemiesThatCanBeSpawend)
            {
                Vector3 pos = RandomCircle(center, 5.0f);
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);
                NavMeshHit closestHit;
                if (NavMesh.SamplePosition(pos, out closestHit, 100, 1))
                {
                    enemy.transform.position = closestHit.position;
                    _spawendEnemies.Add(Instantiate(enemy, closestHit.position, rot));
                }
                else
                {
                    _spawendEnemies.Add(Instantiate(enemy, pos, rot));
                }
            }

            numberOfSpawnCycles++;
        }


        StartCoroutine(SpawnWaitCorutine());
    }

   private Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }
   IEnumerator SpawnWaitCorutine()
    {
        _canSpawn = false;

        yield return new WaitForSeconds(60);

        _canSpawn = true;
    }
}
