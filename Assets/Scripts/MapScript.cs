using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapScript : MonoBehaviour
{
    bool map_active;
    bool map_pressed;
    [SerializeField] GameObject map;
    [SerializeField] GameObject MapCamera;
    [SerializeField] float speed;

    bool buttonMap;
    private Vector2 movementInput = Vector2.zero;

    public void OnMove(InputAction.CallbackContext context){
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnMap(InputAction.CallbackContext context){
        buttonMap = context.action.triggered;
    }

    void Start(){
        map.SetActive(false);
        map_active = false;
    }

    // Update is called once per frame
    void Update(){
        if (buttonMap && !map_pressed){
            if(map_active){
                map_active = false;
                map.SetActive(false);
                PlayerPrefs.SetInt("map_active", 0);
            }else{
                map_active = true;
                map.SetActive(true);
                PlayerPrefs.SetInt("map_active", 1);
            }
            map_pressed = true;
        }
        if(!buttonMap){
            map_pressed = false;
        }
        if(map_active){
            Vector3 pos1 = MapCamera.transform.position;
            pos1.y+=speed*movementInput.y;
            pos1.x+=speed*movementInput.x;
            MapCamera.transform.position = pos1;
        }
    }
}
