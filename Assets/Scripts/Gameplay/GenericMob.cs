using System.Collections.Generic;
using UnityEngine;

public class GenericMob : CharacterMovement
{
    [Header("Mob settings")]
    [SerializeField] private List<GameObject> skins;
    [SerializeField][Min(0f)] private float minMoveTime = 2f, maxMoveTime = 4f, minIdleTime = 1f, maxIdleTime = 2f;

    private GameObject activeSkin = null;

    private float movementDelta = 0f, idleDelta = 0f;
    private float movementTime = 0f, idleTime = 0f;
    private Vector2 lastDirection = default;

    public override Vector2 MovementInput => GetDirection();

    private void OnValidate()
    {
        maxIdleTime = Mathf.Max(minIdleTime, maxIdleTime);
        minIdleTime = Mathf.Min(minIdleTime, maxIdleTime);

        maxMoveTime = Mathf.Max(minMoveTime, maxMoveTime);
        minMoveTime = Mathf.Min(minMoveTime, maxMoveTime);
    }

    protected override void Start()
    {
        base.Start();
        OnValidate();

        // Choose a random direction and a amount of time to move
        lastDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        movementTime = Random.Range(minMoveTime, maxMoveTime);
    }

    private void OnEnable()
    {
        activeSkin = skins?[Random.Range(0, skins.Count)];
        if (activeSkin != null) activeSkin.SetActive(true);
    }

    private void OnDisable()
    {
        if (activeSkin != null) activeSkin.SetActive(false);
    }

    // Mobs start moving in a random direction for the given movement time
    // After they choose another random direction and wait for the idle time
    private Vector2 GetDirection()
    {
        float delta = Time.deltaTime;

        idleDelta += delta;
        if (idleDelta < idleTime) return Vector2.zero;

        movementDelta += delta;
        if (movementDelta >= movementTime)
        {
            lastDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            idleTime = Random.Range(minIdleTime, maxIdleTime);
            movementTime = Random.Range(minMoveTime, maxMoveTime);

            movementDelta = 0f;
            idleDelta = 0f;
        }
        return lastDirection.normalized;
    } 
}
