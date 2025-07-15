using System.Collections;
using UnityEngine;

public class HandleRagdoll : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private Transform ragdollHips;

    [Header("Configurações")]
    [SerializeField] private float enablePhysicsDelay = 0.1f; // Delay entre ativar cada Rigidbody
    [SerializeField] private float positionThreshold = 0.1f;  // Distância mínima para considerar alinhado
    [SerializeField] private float rotationThreshold = 5f;    // Ângulo mínimo para considerar alinhado
    [SerializeField] private bool enableOnAwake = false;
    [SerializeField] private LayerMask collisionBlacklist = 0;

    private Rigidbody[] ragdollRigidbodies;
    private Vector3[] initialLocalPositions;
    private Quaternion[] initialLocalRotations;
    private Transform targetPosition;
    private bool isMoving = false;

    private void Awake()
    {
        // Cache todos os Rigidbodies do Ragdoll
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        // Armazena as posições e rotações locais iniciais
        initialLocalPositions = new Vector3[ragdollRigidbodies.Length];
        initialLocalRotations = new Quaternion[ragdollRigidbodies.Length];

        for (int i = 0; i < ragdollRigidbodies.Length; i++)
        {
            Rigidbody body = ragdollRigidbodies[i];
            initialLocalPositions[i] = body.transform.localPosition;
            initialLocalRotations[i] = body.transform.localRotation;
            body.isKinematic = enableOnAwake;
            body.excludeLayers = collisionBlacklist;
        }
    }

    public void MoveRagdollToTarget(Transform targetPosition)
    {
        this.targetPosition = targetPosition;
        if (!isMoving)
        {
            StartCoroutine(MoveRagdollCoroutine());
        }
    }

    private IEnumerator MoveRagdollCoroutine()
    {
        isMoving = true;

        // Zerar velocidades para evitar comportamentos estranhos
        ResetAllVelocities();

        // 1. Desativar física temporariamente
        SetRagdollKinematic(true);

        // 2. Mover o root object para a posição desejada
        yield return StartCoroutine(SmoothMoveToPosition());

        // 3. Reativar física gradualmente
        yield return StartCoroutine(EnablePhysicsGradually());

        isMoving = false;
    }

    public void SetRagdollKinematic(bool isKinematic)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = isKinematic;

            // Configuração importante para movimentação suave
            rb.interpolation = isKinematic ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = isKinematic ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
        }
    }

    private void ResetAllVelocities()
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private IEnumerator SmoothMoveToPosition()
    {
        float duration = 0.5f; // Duração do movimento suave
        float elapsed = 0f;

        transform.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);

        // Calcula a posição final considerando o offset do hips
        Vector3 hipsOffset = ragdollHips.position - transform.position;
        Vector3 targetPos = targetPosition.position - hipsOffset;

        // Calcula a rotação final
        Quaternion targetRot = targetPosition.rotation * (Quaternion.Inverse(ragdollHips.rotation) * transform.rotation);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            // Movimento suave
            transform.SetPositionAndRotation(Vector3.Lerp(startPosition, targetPos, t), Quaternion.Slerp(startRotation, targetRot, t));
            yield return null;
        }

        // Garante posição exata no final
        transform.SetPositionAndRotation(targetPos, targetRot);

        // Restaura a hierarquia local dos membros (importante para evitar distorções)
        for (int i = 0; i < ragdollRigidbodies.Length; i++)
        {
            ragdollRigidbodies[i].transform.SetLocalPositionAndRotation(initialLocalPositions[i], initialLocalRotations[i]);
        }
    }

    private IEnumerator EnablePhysicsGradually()
    {
        // Começa pelos membros mais distantes do hips (melhor resultado físico)
        System.Array.Sort(ragdollRigidbodies, (a, b) =>
            Vector3.Distance(a.position, ragdollHips.position).CompareTo(
            Vector3.Distance(b.position, ragdollHips.position)));

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = false;
            yield return new WaitForSeconds(enablePhysicsDelay);
        }
    }

    // Método opcional para verificar se o ragdoll está alinhado com o alvo
    public bool IsAlignedWithTarget()
    {
        float distance = Vector3.Distance(ragdollHips.position, targetPosition.position);
        float angle = Quaternion.Angle(ragdollHips.rotation, targetPosition.rotation);

        return distance < positionThreshold && angle < rotationThreshold;
    }
    
    public void AddImpulse(Vector3 force)
    {
        if (ragdollRigidbodies == null) return;

        foreach (var body in ragdollRigidbodies)
        {
            if (body.name.ToLower().Contains("spine"))
            {
                body.AddForceAtPosition(force, body.position, ForceMode.Impulse);
                break;
            }
        }
    }
}
