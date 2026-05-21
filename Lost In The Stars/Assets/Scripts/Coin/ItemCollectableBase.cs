using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCollectableBase : MonoBehaviour
{
    public string compareTag = "Player";
    public ParticleSystem particleSystem;
    public AudioSource audioSource;
    public float timeToEliminateAS = 2f;

    private TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        textMeshProUGUI = GameObject.Find("Text_(TMP)_Contabilizador").GetComponent<TextMeshProUGUI>();

        if (particleSystem != null)
        {
            particleSystem.transform.SetParent(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(compareTag))
        {
            Collect();
        }
    }

    protected virtual void Collect()
    {
        gameObject.SetActive(false);
        OnCollect();
    }

    protected virtual void OnCollect()
    {
        if(particleSystem != null)
        {
            particleSystem.Play();
        }

        if(audioSource != null)
        {
            audioSource.transform.SetParent(null);
            audioSource.Play();
        }
    }
}
