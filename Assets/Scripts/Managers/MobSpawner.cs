using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private GenericMob mobReference;
    [SerializeField][Min(0)] private int initalAmount = 10, totalAmount = 10;
    [SerializeField][Min(0)] private float spawnRadius = 5f, heightOffset = 1f;

    private Queue<GenericMob> mobs;
    private const float kSpawnInterval = 6f;
    private float spawnTimer;

    void OnValidate()
    {
        initalAmount = Mathf.Min(initalAmount, totalAmount);
        totalAmount = Mathf.Max(initalAmount, totalAmount);
    }

    private void Start()
    {
        OnValidate();

        mobs = new();

        for (int i = 0; i < totalAmount; i++)
        {
            Vector3 position = Random.insideUnitSphere * spawnRadius;
            position.y = heightOffset;

            GenericMob mob = Instantiate(mobReference, position, Quaternion.identity, transform);
            bool active = i < initalAmount;
            mob.gameObject.SetActive(active);
            if (!active) mobs.Enqueue(mob);
        }
    }

    private void FixedUpdate()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer > kSpawnInterval)
        {
            Spawn();
            spawnTimer = 0f;
        }
    }

    private void Spawn()
    {
        mobs.Dequeue().gameObject.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
