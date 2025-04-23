using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public int coins = 0;

    public float boostedAnimationSpeed = 9f;
    public float riseSpeed = 1.5f;
    private bool taken = false;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (taken)
        {
            transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            taken = true;
            if (anim != null)
            {
                anim.speed = boostedAnimationSpeed;
            }

            AudioSource audioSource = GetComponent<AudioSource>();
            
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
                Destroy(gameObject, audioSource.clip.length);
            }
            else
            {
                Destroy(gameObject);
            }

            int coins = PlayerPrefs.GetInt("Coins", 0);
            coins++;
            PlayerPrefs.SetInt("Coins", coins);
            PlayerPrefs.Save();
        }
    }
}
