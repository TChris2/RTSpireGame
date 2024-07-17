using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Controls player state and player ui alongside lv load transitions
public class PlayerState : MonoBehaviour
{
    // Loading Transitions
    [SerializeField]
    private GameObject DeathUI;
    private Animator deathUITransition;
    [SerializeField]
    private GameObject DeathLoad;
    private Animator deathLoadTransition;
    [SerializeField]
    private GameObject LvLoad;
    private Animator lvLoadTransition;
    // Text
    private TMPro.TMP_Text healthDisplay;
    // Audio
    public AudioClip[] hurt; 
    public AudioClip vonLaugh; 
    public AudioClip win; 
    private AudioSource audioSource;
    // Health
    public static float health = 100;
    public static float hMax = 100;
    [SerializeField]
    private float eAttackCool = 2f;
    public static bool isDead;
    public static bool isDamaged;
    public static bool isWin;
    public static bool hasWon;
    [SerializeField]
    private bool isInvincible;

    // Health UI Animator
    private Animator playerUIAni;

    AttackInfo eAtkInfo;
    EnemyHurt eHurt;

    private Coroutine CoroutCheck;

    void Awake()
    { 
        isDead = false;
        isDamaged = false;
        isWin = false;
        hasWon = false;
    }
    
    void Start()
    {   
        audioSource = gameObject.GetComponent<AudioSource>();
        
        // Lv intro transition
        Instantiate(LvLoad, Vector3.zero, Quaternion.identity);
        lvLoadTransition = GameObject.Find("LvLoad").GetComponent<Animator>();
        lvLoadTransition.Play("LvLoadIntro");

        // Sets health display
        healthDisplay = GameObject.Find("HealthDisplay").GetComponent<TMPro.TMP_Text>();
        // Gets player's health from previous level
        health = PlayerPrefs.GetFloat("PlayerHealth", 0);
        healthDisplay.text = $"{health}";

        playerUIAni = GameObject.Find("Player UI").GetComponent<Animator>();
    }

    void Update()
    {
        // Checks if the player has won
        if (isWin) {
            PlayerAniMovement.isWinning = false;
            if (!hasWon)
            {
                StartCoroutine(PlayerWin());
            }
        } 
    }
    
    // Takes damage from enemy
    private void OnTriggerEnter(Collider other)
    {
        // Checks to see if the player has already taken damage
        if (!isDamaged && !isDead && !isWin)
        {
            // Check if the object entering collider is an enemy
            if (other.CompareTag("Enemy") && !isInvincible)
            {
                eHurt = other.GetComponent<EnemyHurt>();
                if (!eHurt.isHit)
                {
                    isDamaged = true;
                    eAtkInfo = other.GetComponent<AttackInfo>();

                    if (health - eAtkInfo.dmg < 0)
                        health = 0;
                    else
                        health -= eAtkInfo.dmg;

                    healthDisplay.text = $"{health}";

                    if (health > 0)
                        StartCoroutine(PlayerHurt());
                    // Death
                    else if (health <= 0) {
                        isDead = true;
                        StartCoroutine(PlayerDead());
                    }
                }
            }
        }
    }

    private IEnumerator PlayerHurt()
    {
        // Changes UI
        playerUIAni.Play("Hurt");

        audioSource.PlayOneShot(hurt[0]);
        // Gives a short time of invincibility to the player until they can get hit again
        yield return new WaitForSeconds(eAttackCool);

        // Changes UI back
        playerUIAni.Play("Normal");

        isDamaged = false;
    }

    private IEnumerator PlayerDead()
    {
        audioSource.PlayOneShot(hurt[1]);
        float delay = hurt[1].length; 
        
        playerUIAni.Play("Dead");
        // Waits until the player dead sfx finishes playing
        yield return new WaitForSeconds(delay-1f);
        
        // Death load screen
        Instantiate(DeathLoad, Vector3.zero, Quaternion.identity);

        audioSource.PlayOneShot(vonLaugh);
        delay = vonLaugh.length; 
        // Waits until the laugh sfx plays
        yield return new WaitForSeconds(delay+.5f);

        // Restart screen + Main menu screen
        Instantiate(DeathUI, Vector3.zero, Quaternion.identity);
        
        yield return null;
    }
    private IEnumerator PlayerWin()
    {
        hasWon = true;
        audioSource.PlayOneShot(win);
        float delay = win.length; 
        // Waits until the player win sfx finishes playing
        yield return new WaitForSeconds(delay);
        
        // Lv outro transition
        lvLoadTransition.Play("LvLoadOutro");

        yield return new WaitForSeconds(2f);
        // Next level
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex+1);

        yield return null;
    }

    private void OnDisable()
    {
        // Saves player health for next lv
        PlayerPrefs.SetFloat("PlayerHealth", health);
        PlayerPrefs.Save();
    }
}
