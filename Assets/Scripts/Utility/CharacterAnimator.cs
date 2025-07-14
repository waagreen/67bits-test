using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    private Animator animator;

    private readonly int moveHash = Animator.StringToHash("Move");

    public Vector3 Movement { get; set; }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        animator.SetBool(moveHash, Movement.sqrMagnitude > 0.01f);
    }
}
