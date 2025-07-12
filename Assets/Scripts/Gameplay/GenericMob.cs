using System.Collections.Generic;
using UnityEngine;

public class GenericMob : CharacterMovement
{
    [SerializeField] private List<GameObject> skins;
    [Header("Movement options")]
    [SerializeField][Min(0f)] private float timeToChangeDirection = 1f;

    private GameObject activeSkin = null;

    private float timer = 0f;
    private Vector2 lastDirection = default;

    public override Vector2 MovementInput
    {
        get
        {
            timer += Time.deltaTime;
            if (timer >= timeToChangeDirection)
            {
                lastDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                timer = 0f;
            }
            return lastDirection.normalized;
        }
    }

    protected override void Start()
    {
        base.Start();
        lastDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    private void OnEnable()
    {
        activeSkin = skins?[Random.Range(0, skins.Count)];
        if (activeSkin != null) activeSkin.SetActive(true);
    }

    private void OnDisable()
    {
        if (activeSkin != null) activeSkin.SetActive(false);
    }
}
