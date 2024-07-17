using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls health of enemies
public class Entity : MonoBehaviour
{
    // Enemy hurt sounds
    public AudioClip[] hurtClips; 
    private AudioSource audioSource;
    // Controls starting health for enemies
    [SerializeField]
    private float StartHealth = 5;
    // Keeps track of enemy health
    private float health;
    private UnityEngine.AI.NavMeshAgent enemy;
    Animator eAni;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {   
            health = value;
            // Plays clip when enemy is hurt
            if (audioSource != null && health > 0f) {
                audioSource.PlayOneShot(hurtClips[0]);
                float delay = hurtClips[0].length;
            }
            // When enemy health reaches zero and dies
            if (audioSource != null && health <= 0f)
            {
                // Has a 1 in 50 chance to play the scream
                int randomClipNum = Random.Range(0, 51);
                if (randomClipNum > 0)
                    randomClipNum = 0;
                else
                    randomClipNum = 1;
                audioSource.PlayOneShot(hurtClips[randomClipNum]);
                enemy.enabled = false;
                float delay = hurtClips[randomClipNum].length;
                eAni.Play("DeathFade");
                // Destroys enemy after the sound is played
                Invoke("DestroyEnemy", delay);
            }
        }
    }

    // Destroys the enemy
    void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    void Start()
    {
        // Sets start health for enemies
        Health = StartHealth;
        // Gets enemy components
        audioSource = gameObject.GetComponent<AudioSource>();
        enemy = GetComponent<UnityEngine.AI.NavMeshAgent>();
        eAni = GetComponent<Animator>();
    }
}
