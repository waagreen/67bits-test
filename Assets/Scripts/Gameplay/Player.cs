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

    private IEnumerator CarryCoroutine()
    {
        HandleRagdoll ragdoll = currentCarry.GetComponentInChildren<HandleRagdoll>();
        if (ragdoll == null) yield break;

        Transform ragdollRoot = ragdoll.GetRootForCarrying();

        float elapsed = 0f;
        while (elapsed < kCollectDuration)
        {
            float delta = Time.deltaTime;
            elapsed += delta;

            Vector3 newPosition = Vector3.MoveTowards(ragdollRoot.position, carryPivot.position, delta * 5f);
            ragdollRoot.position = newPosition;

            yield return null;
        }

        ragdollRoot.SetParent(carryPivot);
        ragdollRoot.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        currentCarry = null;
        carryingAmount++;
    }


    private void Carry()
    {
        StopAllCoroutines();
        StartCoroutine(CarryCoroutine());
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

                OnPunch punchEvt = new()
                {
                    duration = 0.1f,
                    magnitude = 0.22f
                };
                EventsManager.Broadcast(punchEvt);
            }
        }
        else if ((collectableLayer & (1 << other.gameObject.layer)) != 0)
        {
            if ((carryingAmount >= carryingCapacity) || (currentCarry != null)) return;

            EventsManager.Broadcast(new OnCollect());
            currentCarry = other.transform.root;
            Debug.Log("CARRY");
            Carry();
        }
    }
}
