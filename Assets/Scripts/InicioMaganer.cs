using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InicioMaganer : MonoBehaviour{

    public GameObject panelInicio;
    public GameObject panelArchivos;
    public GameObject panelControles;

    bool showingControles = false;

    void Start(){
        panelArchivos.SetActive(false);
        panelControles.SetActive(false);
    }

    public void ShowControles(){
        if(!showingControles){
            showingControles = true;
            panelArchivos.SetActive(false);
            panelInicio.SetActive(false);
        }else{
            panelInicio.SetActive(true);
            showingControles = false;
        }
        panelControles.SetActive(showingControles);
    }

    public void Jugar1(){
        PlayerPrefs.SetInt("File",1);
        SceneManager.LoadScene("Level");
    }
    public void Jugar2(){
        PlayerPrefs.SetInt("File",2);
        SceneManager.LoadScene("Level");
    }
    public void Jugar3(){
        PlayerPrefs.SetInt("File",3);
        SceneManager.LoadScene("Level");
    }

    public void IrAArchivos(){
        panelArchivos.SetActive(true);
        panelInicio.SetActive(false);
    }

    public void Volver(){
        panelInicio.SetActive(true);
        panelArchivos.SetActive(false);
    }

    public void Salir(){
        Application.Quit();
    }
}
