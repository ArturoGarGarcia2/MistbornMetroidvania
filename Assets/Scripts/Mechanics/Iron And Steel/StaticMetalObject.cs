using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMetalObject : MonoBehaviour, MetalObject{

    SpriteRenderer sr;
    public Transform player;
    PlayerScript ps;
    PlayerData pd;
    public float allomanticLinesRange = 20f;
    public GameObject allomanticLine;
    float spriteOriginalWidth;

    public bool selected { get; set; }

    // Start is called before the first frame update
    void Start(){
        ps = FindObjectOfType<PlayerScript>();
        sr = GetComponent<SpriteRenderer>();
        if (player == null){
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null){
                player = playerObj.transform;
            }
        }
        spriteOriginalWidth = allomanticLine.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update(){
        if (selected){
            allomanticLine.GetComponent<SpriteRenderer>().color = new Color(0/255f,127/255f,255/255f,100/255f);
        }else{
            allomanticLine.GetComponent<SpriteRenderer>().color = new Color(0/255f,200/255f,255/255f,100/255f);
        }

        pd = ps.pd;
        AloMetal amIro = pd.GetAloMetalIfEquipped((int)Metal.IRON);
        AloMetal amSte = pd.GetAloMetalIfEquipped((int)Metal.STEEL);

        if(
            (amIro != null && amIro.IsBurning()) ||
            (amSte != null && amSte.IsBurning())
        ){
            ShowAllomanticLine();
        }else{
            allomanticLine.gameObject.SetActive(false);
            if(ps.nearMetalObjects.Contains(gameObject)){
                ps.nearMetalObjects.Remove(gameObject);
            }
        }
    }

    void ShowAllomanticLine(){
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        float Mx = (transform.position.x+player.position.x)/2;
        float My = (transform.position.y+player.position.y)/2;

        allomanticLine.transform.position = new Vector2(Mx, My);
        // allomanticLine.transform.width = distanceToPlayer/2;

        float scaleFactor = distanceToPlayer / spriteOriginalWidth;
        allomanticLine.transform.localScale = new Vector2(scaleFactor, allomanticLine.transform.localScale.y);

        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        allomanticLine.transform.rotation = Quaternion.Euler(0f, 0f, angle);


        if (distanceToPlayer < allomanticLinesRange){
            allomanticLine.gameObject.SetActive(true);
            if(!ps.nearMetalObjects.Contains(gameObject)){
                ps.nearMetalObjects.Add(gameObject);
            }
        }else{
            allomanticLine.gameObject.SetActive(false);
            if(ps.nearMetalObjects.Contains(gameObject)){
                ps.nearMetalObjects.Remove(gameObject);
            }
        }
    }
}
