using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : CharacterMovement
{
    [Header("Punch Settings")]
    [SerializeField][Min(0f)] private float punchForce = 50f;
    [SerializeField] private LayerMask punchTargetLayer = -1;

    [Header("Collect Settings")]
    [SerializeField] private LayerMask collectableLayer = 0, depositLayer = 0;
    [SerializeField] private Transform carryPivot = default;
    [SerializeField][Min(0)] private int initalCarryingCapacity = 3;
    [SerializeField][Min(0f)] private float collectCooldown = 0.5f, stackSpacing = 0.6f;
    [SerializeField][Min(0f)] private float followDelay = 0.5f;

    private List<HandleRagdoll> carriedRagdolls = new();
    private float nextCollectTime = 0f; 
    private int carryingCapacity = 0;

    public override Vector2 MovementInput => inputs.Player.Move.ReadValue<Vector2>();
    private InputActions inputs;

    protected override void Start()
    {
        base.Start();

        inputs = new();
        inputs.Enable();

        carryingCapacity = initalCarryingCapacity;
    }

    private void OnDestroy()
    {
        inputs.Disable();
        inputs = null;
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

        Debug.Log("DROP ALL");
        EventsManager.Broadcast(new OnXpGain { amount = carriedRagdolls.Count });
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
            if (carriedRagdolls.Count >= carryingCapacity) return;

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
    }
}
