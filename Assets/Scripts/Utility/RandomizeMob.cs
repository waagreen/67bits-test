using System.Collections.Generic;
using UnityEngine;

public class RandomizeMob : MonoBehaviour
{
    [SerializeField] private List<GameObject> hairs;
    [SerializeField] private List<GameObject> glasses;
    [SerializeField] private List<GameObject> hats;
    [SerializeField] private List<GameObject> accessories;

    private GameObject hair = null;
    private GameObject glass = null;
    private GameObject hat = null;
    private GameObject accesory = null;

    private void SetAssetsState(bool state)
    {
        if (hair != null) hair.SetActive(state);
        if (glass != null) glass.SetActive(state);
        if (hat != null) hat.SetActive(state);
        if (accesory != null) accesory.SetActive(state);
    }

    private void OnEnable()
    {
        hair = hairs?[Random.Range(0, hairs.Count)];
        glass = glasses?[Random.Range(0, glasses.Count)];
        hat = hats?[Random.Range(0, hats.Count)];
        accesory = accessories?[Random.Range(0, accessories.Count)];

        SetAssetsState(true);
    }

    private void OnDisable()
    {
        SetAssetsState(false);
    }
}
