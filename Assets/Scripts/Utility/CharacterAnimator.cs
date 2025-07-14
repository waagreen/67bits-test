using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;

    private readonly int moveHash = Animator.StringToHash("Move");
    private readonly int punchHash = Animator.StringToHash("Punch");

    public Vector3 Movement
    {
        set
        {
            if (animator == null) return;
            animator.SetBool(moveHash, value.sqrMagnitude > 0.01f);
        }
    }

    public void TriggerPunch()
    {
        if (animator == null) return;
        animator.SetTrigger(punchHash);
    }

    public void SetAnimatorState(bool state)
    {
        animator.enabled = state;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }
}
