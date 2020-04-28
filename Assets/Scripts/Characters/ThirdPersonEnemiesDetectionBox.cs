using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonEnemiesDetectionBox : MonoBehaviour
{
    [SerializeField] ThirdPersonController thirdPersonCharacter = default;
    List<EnemyBase> inZoneEnemies = new List<EnemyBase>();

    public void AddEnemyInZone(EnemyBase enemy)
    {
        inZoneEnemies.Add(enemy);
        enemy.DetectFirstPersonCharacter(thirdPersonCharacter);
    }

    public void RemoveEnemyFromZone(EnemyBase enemy)
    {
        inZoneEnemies.Remove(enemy);
        enemy.LosePlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyBase hitEnemy = other.GetComponent<EnemyBase>();
        if (hitEnemy)
        {
            AddEnemyInZone(hitEnemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EnemyBase hitEnemy = other.GetComponent<EnemyBase>();
        if (hitEnemy)
        {
            RemoveEnemyFromZone(hitEnemy);
        }
    }
}
