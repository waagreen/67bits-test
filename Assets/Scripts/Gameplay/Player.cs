using System.Collections;
using UnityEngine;

public class Player : CharacterMovement
{
    [Header("Player Settings")]
    [SerializeField][Min(0)] private int initalCarryingCapacity = 3;
    [SerializeField][Min(0f)] private float punchForce = 50f;
    [SerializeField] private LayerMask punchTargetLayer = -1;
    [SerializeField] private LayerMask collectableLayer = 0;
    [SerializeField] private Transform carryPivot = default;
    [SerializeField][Min(0f)] private float collectCooldown = 0.5f; 

    private float nextCollectTime = 0f; 
    private int carryingAmount = 0;
    private int carryingCapacity = 0;
    private Transform currentCarry = default;
    private const int kCollectDuration = 2;

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

    private void Carry()
    {
        var ragdoll = currentCarry.GetComponentInChildren<HandleRagdoll>();
        if (ragdoll == null) return;
        ragdoll.AttachToTarget(carryPivot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((punchTargetLayer & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent(out GenericMob mob))
            {
                mob.SetConsciousness(false);
                anim.TriggerPunch();
                mob.RecievePunch(transform.forward, punchForce);

                nextCollectTime = Time.time + collectCooldown;

                EventsManager.Broadcast(new OnPunch { duration = 0.1f, magnitude = 0.22f });
            }
        }
        else if ((collectableLayer & (1 << other.gameObject.layer)) != 0)
        {
            if ((carryingAmount >= carryingCapacity) || (currentCarry != null)) return;
            if (Time.time < nextCollectTime) return;

            EventsManager.Broadcast(new OnCollect());
            currentCarry = other.transform.root;
            Debug.Log("CARRY");
            Carry();
        }
    }
}
