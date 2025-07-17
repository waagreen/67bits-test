using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterMovement
{
    [Header("Punch Settings")]
    [SerializeField][Min(0f)] private float punchForce = 50f;
    [SerializeField] private LayerMask punchTargetLayer = -1;

    [Header("Collect Settings")]
    [SerializeField] private Transform carryPivot = default;
    [SerializeField][Min(0f)] private float collectCooldown = 0.5f, stackSpacing = 0.6f;
    [SerializeField][Min(0f)] private float followDelay = 0.5f;

    [Header("Collision Masks")]
    [SerializeField] private LayerMask storeLayer = 0;
    [SerializeField] private LayerMask collectableLayer = 0, depositLayer = 0;

    private InputActions inputs;
    private readonly List<HandleRagdoll> carriedRagdolls = new();
    private float nextCollectTime = 0f; 
    private int strength;

    private void Awake()
    {
        inputs = new();
        inputs.Enable();

        EventsManager.AddSubscriber<OnLevelUp>(UpdateStats);
    }

    protected override void Update()
    {
        movementInput = inputs.Player.Move.ReadValue<Vector2>();
        base.Update();
    }

    private void OnDestroy()
    {
        EventsManager.RemoveSubscriber<OnLevelUp>(UpdateStats);

        inputs.Disable();
        inputs = null;
    }

    private void UpdateStats(OnLevelUp evt)
    {
        strength = evt.strength;
        speed = evt.speed;
    }

    private void Carry(HandleRagdoll ragdoll)
    {
        int index = carriedRagdolls.Count;

        Vector3 offset = new(0, index * stackSpacing, 0);

        ragdoll.SetFollowDelay(followDelay, index);
        ragdoll.AttachToTarget(carryPivot, offset);

        carriedRagdolls.Add(ragdoll);
    }

    public void DropAll(Transform deposit)
    {
        if ((carriedRagdolls == null) || (carriedRagdolls.Count < 1)) return;

        for (int i = 0; i < carriedRagdolls.Count; i++)
        {
            HandleRagdoll ragdoll = carriedRagdolls[i];
            ragdoll.SetFollowDelay(0.1f, i + 1);
            ragdoll.AttachToTarget(deposit, Vector3.zero, detachOnReachDestination: true);
        }

        // Reward experience for mob corpses
        EventsManager.Broadcast(new OnExperienceChange
        {
            delta = carriedRagdolls.Count,
            previous = PlayerProfile.Experience
        });
        carriedRagdolls.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((punchTargetLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent(out GenericMob mob))
            {
                mob.SetUnconscious();
                anim.TriggerPunch();
                mob.RecievePunch(transform.forward, punchForce);

                nextCollectTime = Time.time + collectCooldown;

                EventsManager.Broadcast(new OnPunch { duration = 0.1f, magnitude = 0.22f });
            }
        }
        else if ((collectableLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (Time.time < nextCollectTime) return;
            if (carriedRagdolls.Count >= strength) return;

            if (other.transform.parent.TryGetComponent(out HandleRagdoll ragdoll) && !carriedRagdolls.Contains(ragdoll))
            {
                if (!ragdoll.CanBeCollected) return;

                Carry(ragdoll);
                nextCollectTime = Time.time + collectCooldown;
                EventsManager.Broadcast(new OnCollect { amount = 1 });
            }
        }
        else if ((depositLayer & (1 << other.gameObject.layer)) != 0)
        {
            DropAll(other.transform);
        }
        else if ((storeLayer & (1 << other.gameObject.layer)) != 0)
        {
            EventsManager.Broadcast(new OnStoreInteraction());
        }
    }
}
