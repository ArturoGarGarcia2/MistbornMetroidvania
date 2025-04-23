using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAudio : MonoBehaviour
{
    public AudioSource audioSource;  // Asegúrate de asignar el AudioSource en el Inspector

    void Start()
    {
        // Reproducir el sonido al iniciar
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError("AudioSource no asignado.");
        }
    }
}
