using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GenericMob mobReference;

    [Header("Settings")]
    [SerializeField][Min(0)] private int initalAmount = 10;
    [SerializeField][Min(0)] private float spawnRadius = 5f, heightOffset = 1f;
    private List<GenericMob> mobs;

    private void Start()
    {
        mobs = new();
        
        for (int i = 0; i < initalAmount; i++)
        {
            Vector3 position = Random.insideUnitSphere * spawnRadius;
            position.y = heightOffset;

            GenericMob mob = Instantiate(mobReference, position, Quaternion.identity, transform);
            mobs.Add(mob);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
