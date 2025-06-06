using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class FuenteBehavior : MonoBehaviour
{
    
    [SerializeField] GameObject Fuente;
    PlayerScript player;
    bool playerInside;
    bool encendida = false;

    // Start is called before the first frame update
    void Start(){
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        int id = 0;

        string[] nombrePartes = Fuente.name.Split(" ");
        if (nombrePartes.Length > 1) {
            int fuenteID;
            if (int.TryParse(nombrePartes[1], out fuenteID)) {
                id = fuenteID;
            } else {
            }
        }

        // Obtener la posición de la fuente
        float posicionX = Fuente.transform.position.x;
        float posicionY = Fuente.transform.position.y;

        // Verificar si ya existe un registro en la tabla punto_control para ese ID
        int count = (int)(long)DatabaseManager.Instance.ExecuteScalar(
            "SELECT COUNT(*) FROM checkpoint WHERE id = @param0;", id);

        if (count == 0) {
            // Si no existe, insertar el nuevo punto de control con el id y las posiciones
            string insertQuery = "INSERT INTO checkpoint (id, x_pos, y_pos) " +
                                "VALUES (@param0, @param1, @param2);";
            DatabaseManager.Instance.ExecuteNonQuery(insertQuery, id, posicionX, posicionY-1.5);
        } else {
        }

        // Comprobar si la fuente está activa
        int activo = (int)(long)DatabaseManager.Instance.ExecuteScalar(
            "SELECT COUNT(*) FROM visited_checkpoint WHERE file_id = 1 AND checkpoint_id = @param0;", id);

        if (activo > 0) {
            Fuente.GetComponent<Animator>().SetBool("Encendida", true);
        } else {
            Fuente.GetComponent<Animator>().SetBool("Encendida", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInside && player.interacting && !encendida){
            Fuente.GetComponent<Animator>().SetBool("Encendiendo",true);
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag=="Player"){
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if(other.tag=="Player"){
            playerInside = false;
        }
    }
}
