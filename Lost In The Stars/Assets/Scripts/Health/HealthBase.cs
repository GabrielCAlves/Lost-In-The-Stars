using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public int startLife = 10;

    public bool destroyOnKill = false;
    public float delayToKill = 2f;

    //public Player playerScript;
    public string death = "DeathTrigger";
    private Animator animator;
    public Rigidbody2D rigidbody2D;

    public AudioSource audioSource;

    public EndGame endGame;

    private int _currentLife;
    private bool _isDead = false;

    [SerializeField] private FlashColor flashColor;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _isDead = false;
        _currentLife = startLife;

        animator = gameObject.GetComponentInChildren<Animator>();
        flashColor = gameObject.GetComponentInChildren<FlashColor>();

        if (flashColor == null)
        {
            flashColor = GetComponent<FlashColor>();
        }
    }

    public void Damage(int damage)
    {
        if (_isDead)
        {
            return;
        }

        _currentLife -= damage;

        if(flashColor != null)
        {
            flashColor.Flash();
        }
        
        if(_currentLife <= 0)
        {
            StartCoroutine(Kill());
        }
    }

    private IEnumerator Kill()
    {
        _isDead = true;

        if (destroyOnKill)
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;

            audioSource.Play();
            //rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            //animator.SetBool(death, true);
            animator.SetTrigger(death);
            //Debug.Log("playerScript.soPlayerSetup.death = "+ playerScript.soPlayerSetup.death);
            //playerScript.soPlayerSetup.player.SetTrigger(playerScript.soPlayerSetup.death);
            yield return new WaitForSeconds(0.8f);

            if (endGame != null && gameObject.CompareTag(endGame.tagToCompare))
            {
                endGame.CallEndGame();
            }

            Destroy(gameObject);
        }
    }
}
