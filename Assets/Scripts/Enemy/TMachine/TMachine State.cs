using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls animation state of TMachines
public class TMachineState : MonoBehaviour
{
    public AudioClip ding; 
    private AudioSource audioSource;
    public bool eSpawnTime;
    private TMachineEntity TMEntity;
    [SerializeField]
    private bool isBreakable;
    private Animator TMAni;


    void Start()
    {
        // Gets TMachine components
        audioSource = GetComponentInChildren<AudioSource>();
        TMEntity = GetComponentInChildren<TMachineEntity>();
        TMAni = GetComponent<Animator>();
    }

    void Update()
    {
        // If the TMachine health is zero
        if (TMEntity != null && TMEntity.isTMachineDestroyed)
        {
            TMAni.Play("TMachineDestroyed");
        }
        // Open and closing animations
        else if (!isBreakable && eSpawnTime && !PlayerState.isDead && !PlayerState.isWin || eSpawnTime && !PlayerState.isDead && !PlayerState.isWin && TMEntity != null && !TMEntity.isTMachineDestroyed)
        {
            eSpawnTime = false;
            TMAni.Play("TMachineOpen");
            audioSource.PlayOneShot(ding);
            float delay = ding.length; 
            // Closes machine after delay
            Invoke("MachineClose", delay-1f);
        }
        // If player has died or won
        else if (PlayerState.isDead || PlayerState.isWin)
            TMAni.Play("TMachineClosed");
        
    }

    void MachineClose()
    {
        if (!isBreakable || TMEntity.isTMachineDestroyed == false)
        {
            TMAni.Play("TMachineClosed");
        }
    }
}
