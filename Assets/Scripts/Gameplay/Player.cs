using UnityEngine;

public class Player : CharacterMovement
{
    [Header("Player Settings")]
    [SerializeField] private float punchForce = 50f;
    [SerializeField] private LayerMask punchTargetLayer = -1;
    [SerializeField] private LayerMask collectableLayer = 0;

    public override Vector2 MovementInput => inputs.Player.Move.ReadValue<Vector2>();
    private InputActions inputs;

    protected override void Start()
    {
        base.Start();
        inputs = new();
        inputs.Enable();
    }

    private void OnDestroy()
    {
        inputs.Disable();
        inputs = null;
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
            EventsManager.Broadcast(new OnCollect());
        }
    }
}
