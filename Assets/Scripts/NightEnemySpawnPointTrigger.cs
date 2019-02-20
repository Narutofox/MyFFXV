using UnityEngine;

namespace Assets.Scripts
{
    public class NightEnemySpawnPointTrigger : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            NightEnemySpawnPoint nightEnemySpawnPoint = other.gameObject.GetComponent<NightEnemySpawnPoint>();
            if (nightEnemySpawnPoint != null)
            {
                nightEnemySpawnPoint.SpawnEnemies();
            }
        }
    }
}
