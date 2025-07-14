using UnityEngine;

public class Player : CharacterMovement
{
    [Header("Player Settings")]
    [SerializeField] private LayerMask punchTargetLayer = -1;

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
        if ((punchTargetLayer & (1 << other.gameObject.layer)) == 0) return;

        if (other.TryGetComponent(out GenericMob mob))
        {
            mob.SetConsciousness(false);
            anim.TriggerPunch();
        }
    }
}
