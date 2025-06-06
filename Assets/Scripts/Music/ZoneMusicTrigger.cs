using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneMusicTrigger : MonoBehaviour{
    public string musicId;

    public bool inDoors;
    public bool outDoors;

    PlayerScript ps;

    void Start(){
        ps = FindObjectOfType<PlayerScript>();
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Player")){
            MusicManager.Instance?.PlayMusic(musicId);
            ps.inDoors = inDoors;
            ps.outDoors = outDoors;
        }
    }
}
