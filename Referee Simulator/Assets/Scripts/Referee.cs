using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Referee : MonoBehaviour
{
    // This script is used to identify the referee

    private bool isWhistling;
    [SerializeField] private AudioClip whistleClip;
    private AudioSource audioSource;
    
    private Image yellowOrRedCardImage;
    [SerializeField] private Sprite yellowSprite;
    [SerializeField] private Sprite redSprite;
    
    private void Start()
    {
        isWhistling = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = whistleClip;
        yellowOrRedCardImage = GameObject.Find("YellowOrRedCardImage").GetComponent<Image>();

    }

    private void Update()
    {
        if (GameEvent.isSimulationOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isWhistling = true;
                audioSource.Play();
            }
            if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl))
            {
                yellowOrRedCardImage.enabled = true;
                yellowOrRedCardImage.sprite = redSprite;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                yellowOrRedCardImage.enabled = true;
                yellowOrRedCardImage.sprite = yellowSprite;
            }
            
        }
        
    }

    public void SetRefereePosition(Vector3 position)
    {
        transform.position = position;
    }

    private void OnDestroy()
    {
        GameEvent.isRefereeDropped = false;
    }
}
