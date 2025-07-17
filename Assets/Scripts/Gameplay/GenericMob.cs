using System.Collections.Generic;
using UnityEngine;

public class GenericMob : CharacterMovement
{
    [Header("Mob settings")]
    [SerializeField] private Collider mainCollider;
    [SerializeField] private LayerMask obstacleLayer = 0;
    [SerializeField][Min(0f)] private float minMoveTime = 2f, maxMoveTime = 4f, minIdleTime = 1f, maxIdleTime = 2f, collisionCooldown = 0.5f;
    [SerializeField] private List<GameObject> skins;

    private GameObject activeSkin = null;
    private HandleRagdoll ragdoll = null;
    private bool isConscious = true;
    private float lastCollisionTime = -Mathf.Infinity;
    private float movementTimer = 0f, idleTimer = 0f;
    private float movementDuration = 0f, idleDuration = 0f;
    private Vector2 currentDirection = default;

    private void OnValidate()
    {
        maxIdleTime = Mathf.Max(minIdleTime, maxIdleTime);
        minIdleTime = Mathf.Min(minIdleTime, maxIdleTime);

        maxMoveTime = Mathf.Max(minMoveTime, maxMoveTime);
        minMoveTime = Mathf.Min(minMoveTime, maxMoveTime);
    }

    private void Awake()
    {
        OnValidate();
        
        EventsManager.AddSubscriber<OnDropCorpse>(RestoreInitialState);
    }

    protected override void Start()
    {
        base.Start();

        // Choose a random direction and a amount of time to move
        currentDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        movementDuration = Random.Range(minMoveTime, maxMoveTime);
    }

    protected override void Update()
    {
        movementInput = GetDirection();
        base.Update();
    }

    private void OnEnable()
    {
        activeSkin = skins?[Random.Range(0, skins.Count)];
        if (activeSkin != null)
        {
            activeSkin.SetActive(true);
            if (activeSkin.TryGetComponent(out HandleRagdoll obj)) ragdoll = obj;
        }
    }

    private void OnDisable()
    {
        if (activeSkin != null)
        {
            activeSkin.SetActive(false);
            ragdoll = null;
        }
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnDropCorpse>(RestoreInitialState);
    }

    private Vector2 GetDirection()
    {
        if (!isConscious) return Vector2.zero;

        float delta = Time.deltaTime;

        idleTimer += delta;
        if (idleTimer < idleDuration) return Vector2.zero;

        movementTimer += delta;
        if (movementTimer >= movementDuration)
        {
            currentDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            idleDuration = Random.Range(minIdleTime, maxIdleTime);
            movementDuration = Random.Range(minMoveTime, maxMoveTime);

            movementTimer = 0f;
            idleTimer = 0f;
        }
        return currentDirection;
    }

    private void RestoreInitialState(OnDropCorpse evt)
    {
        if (!evt.id.Equals(transform.GetInstanceID())) return;

        // Prepare this object so it can be re-used by the object pooler
        ragdoll.RestoreInitialState();
        ToggleMovement(true);
        gameObject.SetActive(false);
    }

    private void ToggleMovement(bool state)
    {
        // In case of unconsciousness, disable animator and pass body control over to the physics engine
        anim.SetAnimatorState(state);

        // In case of unconsciousness, disable collider so the player can't bump on the corpse
        mainCollider.enabled = state;

        // In case of unconsciousness, restrain rigidbody and disable gravity to prevent flickering
        rb.constraints = state ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeAll;
        rb.useGravity = state;
    }

    public void SetUnconscious()
    {
        if ((ragdoll == null) || (anim == null)) return;

        ragdoll.DisableKinematics();
        ToggleMovement(false);
        isConscious = false;
    }

    public void RecievePunch(Vector3 direction, float power)
    {
        ragdoll.AddImpulse(direction * power);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((obstacleLayer & (1 << collision.gameObject.layer)) == 0) return;

        if (Time.time - lastCollisionTime < collisionCooldown) return;
        lastCollisionTime = Time.time;

        // Makes an agregate with every contact point normal
        Vector3 totalNormal = Vector3.zero;
        foreach (var contact in collision.contacts) totalNormal += contact.normal;
        Vector3 averageNormal = totalNormal.normalized;

        // Project vector to XZ plane
        Vector2 newDir = new(averageNormal.x, averageNormal.z);
        if (newDir.sqrMagnitude < 0.1f) // fallback in case normal is almost zero
        {
            newDir = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        currentDirection = newDir.normalized;

        // Reset only movement timer so the mob instantly change it's movement direction
        movementTimer = 0f;
        movementDuration = Random.Range(minMoveTime, maxMoveTime);
    }

}
