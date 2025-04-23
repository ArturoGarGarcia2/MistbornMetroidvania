using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocuraManager : MonoBehaviour{
    public enum FaseLocura{
        Ninguna,
        MurmullosDeLasParedes,
        OjosEnLaOscuridad,
        ElMundoSeRompe,
        DominioDeRuin,
        ExtasisHemalurgico,
        DecliveMetalico
    }

    public FaseLocura faseActual = FaseLocura.Ninguna;

    private float tiempoEnFaseActual = 0f;
    private float tiempoParaSiguienteFase = 0f;

    private int clavosEquipados = 0;
    private bool enModoRetroceso = false;
    private bool guardandoEnMentealuminio = false;
    private bool decantandoMentealuminio = false;

    private Dictionary<FaseLocura, float[]> duracionesPorFase = new Dictionary<FaseLocura, float[]>{
        // Duraciones en segundos para 1 a 7 clavos (por fase)
        { FaseLocura.MurmullosDeLasParedes, new float[] {3600f, 3000f, 2400f, 1800f, 1200f, 900f, 600f} },
        { FaseLocura.OjosEnLaOscuridad,     new float[] {3600f, 3000f, 2400f, 1800f, 1200f, 900f, 600f} },
        { FaseLocura.ElMundoSeRompe,        new float[] {2400f, 1800f, 1500f, 1200f, 800f, 600f, 400f} },
        { FaseLocura.DominioDeRuin,         new float[] {1800f, 1500f, 1200f, 900f, 600f, 450f, 300f} },
        { FaseLocura.ExtasisHemalurgico,    new float[] {1800f, 1500f, 1200f, 900f, 600f, 450f, 300f} },
        { FaseLocura.DecliveMetalico,        new float[] {600f, 600f, 600f, 600f, 600f, 600f, 600f} },
    };

    private Coroutine progresoCoroutine;

    private void Start(){
        // No empieza la locura hasta después de 90s de colocar un clavo
    }

    public void AplicarClavo(){
        clavosEquipados++;
        if (clavosEquipados == 1)
            StartCoroutine(EsperarInicioLocura());
    }

    public void QuitarClavo(){
        clavosEquipados = Mathf.Max(0, clavosEquipados - 1);
        if (clavosEquipados == 0)
            IniciarRetroceso();
    }

    IEnumerator EsperarInicioLocura(){
        yield return new WaitForSeconds(90f); // 1.5 minutos
        if (clavosEquipados > 0 && progresoCoroutine == null)
            progresoCoroutine = StartCoroutine(ActualizarLocura());
    }

    IEnumerator ActualizarLocura(){
        while (true){
            float duracionFase = ObtenerDuracionFase(faseActual, clavosEquipados);
            tiempoParaSiguienteFase = duracionFase;
            tiempoEnFaseActual = 0f;

            while (tiempoEnFaseActual < tiempoParaSiguienteFase){
                float delta = Time.deltaTime;

                // Aceleración o reversión
                if (guardandoEnMentealuminio)
                    delta *= 1.33f; // 1 / 0.75
                else if (decantandoMentealuminio)
                    delta *= 5f; // 1 / 0.20

                tiempoEnFaseActual += enModoRetroceso ? -delta : delta;

                if (!enModoRetroceso && tiempoEnFaseActual >= tiempoParaSiguienteFase){
                    AvanzarFase();
                    break;
                }
                else if (enModoRetroceso && tiempoEnFaseActual <= 0f){
                    RetrocederFase();
                    break;
                }

                yield return null;
            }
        }
    }

    float ObtenerDuracionFase(FaseLocura fase, int clavos){
        int index = Mathf.Clamp(clavos - 1, 0, 6); // 0 a 6 para 1-7 clavos
        float baseDuracion = duracionesPorFase.ContainsKey(fase) ? duracionesPorFase[fase][index] : 9999f;

        return baseDuracion;
    }

    void AvanzarFase(){
        if (faseActual < FaseLocura.DecliveMetalico)
            faseActual++;
        else
            faseActual = FaseLocura.ElMundoSeRompe; // Reinicio suave
    }

    void RetrocederFase(){
        if (faseActual > FaseLocura.Ninguna)
            faseActual--;
        else
            DetenerLocura();
    }

    void DetenerLocura(){
        if (progresoCoroutine != null){
            StopCoroutine(progresoCoroutine);
            progresoCoroutine = null;
        }

        faseActual = FaseLocura.Ninguna;
        tiempoEnFaseActual = 0f;
        enModoRetroceso = false;
    }

    void IniciarRetroceso(){
        enModoRetroceso = true;
    }

    // Feruquimia externa
    public void SetGuardandoMenteAluminio(bool guardando){
        guardandoEnMentealuminio = guardando;
        if (guardando)
            decantandoMentealuminio = false;
    }

    public void SetDecantandoMenteAluminio(bool decantando){
        decantandoMentealuminio = decantando;
        if (decantando)
            guardandoEnMentealuminio = false;
    }
}
