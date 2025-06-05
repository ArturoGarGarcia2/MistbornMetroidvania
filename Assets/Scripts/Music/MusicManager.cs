using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour{
    public static MusicManager Instance;

    public AudioSource audioSource;
    public List<MusicTrack> tracks;

    private string currentTrackId = "";

    void Awake(){
        if (Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayMusic(string trackId){
        if (trackId == currentTrackId) return;

        MusicTrack track = tracks.Find(t => t.id == trackId);
        if (track != null){
            audioSource.clip = track.clip;
            audioSource.loop = true;
            audioSource.Play();
            currentTrackId = trackId;
        }
        // StartCoroutine(MusicCooldown());
    }

    // IEnumerator MusicCooldown(){
    //     yield return new WaitForSeconds(.5f);
    //     if (trackId == currentTrackId) return;

    //     MusicTrack track = tracks.Find(t => t.id == trackId);
    //     if (track != null){
    //         audioSource.clip = track.clip;
    //         audioSource.loop = true;
    //         audioSource.Play();
    //         currentTrackId = trackId;
    //     }
    // }
}

[System.Serializable]
public class MusicTrack{
    public string id;
    public AudioClip clip;
}
