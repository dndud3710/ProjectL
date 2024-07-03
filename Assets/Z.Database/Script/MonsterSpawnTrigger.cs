using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnTrigger : MonoBehaviour
{
    private Collider coll;
    public int spawnPointNum;

    private void Awake()
    {
        coll = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            coll.enabled = false;
            if (DungeonManager.Instance == null)
            {
                Debug.Log("Dungeon Manager is null");
                StartCoroutine(WaitForDungeonManager());
            }
            else
                DungeonManager.Instance.SpawnMonster(spawnPointNum);
        }
    }

    IEnumerator WaitForDungeonManager()
    {
        yield return new WaitUntil(() => DungeonManager.Instance != null);
        DungeonManager.Instance.SpawnMonster(spawnPointNum);
    }
}
