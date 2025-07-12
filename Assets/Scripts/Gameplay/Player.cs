using UnityEngine;

public class Player : CharacterMovement
{
    private InputActions inputs;
    public override Vector2 MovementInput => inputs.Player.Move.ReadValue<Vector2>();

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
}
