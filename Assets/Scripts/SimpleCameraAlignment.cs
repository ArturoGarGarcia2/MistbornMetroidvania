using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimpleCameraAlli : MonoBehaviour
{
    [Header("Componentes para la alineación de la cámara")]
    [SerializeField] Grid grid;
    [SerializeField] GameObject MainCamera;
    [SerializeField] GameObject player;

    [Header("Componentes para indicar la sala")]
    [SerializeField] Tilemap map_tilemap;
    // [SerializeField] Tilemap foreground_map_tilemap;
    [SerializeField] GameObject playerDot;

    [Header("Agrupaciones de Tiles")]
    [SerializeField] Tile[] sala1x1Tiles;
    [SerializeField] Tile[] esquinaSuperiorIzquierdaTiles;
    [SerializeField] Tile[] centroSuperiorSalaGrandeTiles;
    [SerializeField] Tile[] esquinaSuperiorDerechaTiles;
    [SerializeField] Tile[] izquierdaSalaGrandeTiles;
    [SerializeField] Tile[] centroSalaGrandeTiles;
    [SerializeField] Tile[] derechaSalaGrandeTiles;
    [SerializeField] Tile[] esquinaInferiorIzquierdaTiles;
    [SerializeField] Tile[] centroInferiorSalaGrandeTiles;
    [SerializeField] Tile[] esquinaInferiorDerechaTiles;
    [SerializeField] Tile[] superiorPasilloVerticalTiles;
    [SerializeField] Tile[] centroPasilloVerticalTiles;
    [SerializeField] Tile[] inferiorPasilloVerticalTiles;
    [SerializeField] Tile[] izquierdaPasilloHorizontalTiles;
    [SerializeField] Tile[] centroPasilloHorizontalTiles;
    [SerializeField] Tile[] derechaPasilloHorizontalTiles;

    PauseManager pauseManager;

    void Start(){
        pauseManager = FindObjectOfType<PauseManager>();
    }

    void Update(){
        Vector3Int cellPosition = grid.WorldToCell(player.transform.position);
        Tile currentTile = map_tilemap.GetTile<Tile>(cellPosition);

        // if (pauseManager.isPaused /*== null*/) return;

        // foreground_map_tilemap.SetTile(cellPosition, null);

        if (PlayerPrefs.GetInt("map_active") == 0){
                MainCamera.transform.position = new Vector2(player.transform.position.x,player.transform.position.y);
                MainCamera.transform.position = grid.GetCellCenterWorld(cellPosition);
            if (System.Array.Exists(sala1x1Tiles, tile => tile == currentTile)){ //0
                MainCamera.transform.position = grid.GetCellCenterWorld(cellPosition);
            }
            else if (System.Array.Exists(esquinaSuperiorIzquierdaTiles, tile => tile == currentTile)){ //1
                XmenosYmas(cellPosition);
            }
            else if (System.Array.Exists(centroSuperiorSalaGrandeTiles, tile => tile == currentTile)){ //2
                Ymas(cellPosition);
            }
            else if (System.Array.Exists(esquinaSuperiorDerechaTiles, tile => tile == currentTile)){ //3
                XmasYmas(cellPosition);
            }
            else if (System.Array.Exists(izquierdaSalaGrandeTiles, tile => tile == currentTile)){ //4
                Xmenos(cellPosition);
            }
            else if (System.Array.Exists(centroSalaGrandeTiles, tile => tile == currentTile)){ //5
                noXY(cellPosition);
            }
            else if (System.Array.Exists(derechaSalaGrandeTiles, tile => tile == currentTile)){ //6
                Xmas(cellPosition);
            }
            else if (System.Array.Exists(esquinaInferiorIzquierdaTiles, tile => tile == currentTile)){ //7
                XmenosYmenos(cellPosition);
            }
            else if (System.Array.Exists(centroInferiorSalaGrandeTiles, tile => tile == currentTile)){ //8
                Ymenos(cellPosition);
            }
            else if (System.Array.Exists(esquinaInferiorDerechaTiles, tile => tile == currentTile)){ //9
                XmasYmenos(cellPosition);
            }
            else if (System.Array.Exists(superiorPasilloVerticalTiles, tile => tile == currentTile)){ //10
                XigualYmas(cellPosition);
            }
            else if (System.Array.Exists(centroPasilloVerticalTiles, tile => tile == currentTile)){ //11
                Xigual(cellPosition);
            }
            else if (System.Array.Exists(inferiorPasilloVerticalTiles, tile => tile == currentTile)){ //12
                XigualYmenos(cellPosition);
            }
            else if (System.Array.Exists(izquierdaPasilloHorizontalTiles, tile => tile == currentTile)){ //13
                XmenosYigual(cellPosition);
            }
            else if (System.Array.Exists(centroPasilloHorizontalTiles, tile => tile == currentTile)){ //14
                Yigual(cellPosition);
            }
            else if (System.Array.Exists(derechaPasilloHorizontalTiles, tile => tile == currentTile)){ //15
                XmasYigual(cellPosition);
            }

            // foreground_map_tilemap.gameObject.SetActive(false);
        }else{
            playerDot.transform.position = map_tilemap.GetCellCenterWorld(cellPosition);
            // foreground_map_tilemap.gameObject.SetActive(true);
        }
    }

    private void XmenosYmas(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.x<grid.GetCellCenterWorld(cellPosition).x){Pos.x = grid.GetCellCenterWorld(cellPosition).x;}
        if(Pos.y>grid.GetCellCenterWorld(cellPosition).y){Pos.y = grid.GetCellCenterWorld(cellPosition).y;}
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void XmasYmenos(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.x>grid.GetCellCenterWorld(cellPosition).x){Pos.x = grid.GetCellCenterWorld(cellPosition).x;}
        if(Pos.y<grid.GetCellCenterWorld(cellPosition).y){Pos.y = grid.GetCellCenterWorld(cellPosition).y;}
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void XmenosYmenos(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.x<grid.GetCellCenterWorld(cellPosition).x){Pos.x = grid.GetCellCenterWorld(cellPosition).x;}
        if(Pos.y<grid.GetCellCenterWorld(cellPosition).y){Pos.y = grid.GetCellCenterWorld(cellPosition).y;}
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void XmasYmas(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.x>grid.GetCellCenterWorld(cellPosition).x){Pos.x = grid.GetCellCenterWorld(cellPosition).x;}
        if(Pos.y>grid.GetCellCenterWorld(cellPosition).y){Pos.y = grid.GetCellCenterWorld(cellPosition).y;}
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void Xmas(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.x>grid.GetCellCenterWorld(cellPosition).x){Pos.x = grid.GetCellCenterWorld(cellPosition).x;}
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void Xmenos(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.x<grid.GetCellCenterWorld(cellPosition).x){Pos.x = grid.GetCellCenterWorld(cellPosition).x;}
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void Ymas(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.y>grid.GetCellCenterWorld(cellPosition).y){Pos.y = grid.GetCellCenterWorld(cellPosition).y;}
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void Ymenos(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.y<grid.GetCellCenterWorld(cellPosition).y){Pos.y = grid.GetCellCenterWorld(cellPosition).y;}
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void SeguirXQuietaY(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        Pos.x = Mathf.Max(Pos.x, grid.GetCellCenterWorld(cellPosition).x);
        Pos.y = grid.GetCellCenterWorld(cellPosition).y;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void Xigual(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        Pos.x = grid.GetCellCenterWorld(cellPosition).x;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void Yigual(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        Pos.y = grid.GetCellCenterWorld(cellPosition).y;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void XigualYmenos(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.y<grid.GetCellCenterWorld(cellPosition).y){Pos.y = grid.GetCellCenterWorld(cellPosition).y;}
        Pos.x = grid.GetCellCenterWorld(cellPosition).x;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void XigualYmas(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.y>grid.GetCellCenterWorld(cellPosition).y){Pos.y = grid.GetCellCenterWorld(cellPosition).y;}
        Pos.x = grid.GetCellCenterWorld(cellPosition).x;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void XmenosYigual(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.x<grid.GetCellCenterWorld(cellPosition).x){Pos.x = grid.GetCellCenterWorld(cellPosition).x;}
        Pos.y = grid.GetCellCenterWorld(cellPosition).y;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void XmasYigual(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        if(Pos.x>grid.GetCellCenterWorld(cellPosition).x){Pos.x = grid.GetCellCenterWorld(cellPosition).x;}
        Pos.y = grid.GetCellCenterWorld(cellPosition).y;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void noXY(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void SeguirJugadorXQuieta(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        Pos.y = grid.GetCellCenterWorld(cellPosition).y;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
    private void SeguirJugadorYQuieta(Vector3Int cellPosition)
    {
        Vector3 Pos = player.transform.position;
        Pos.x = grid.GetCellCenterWorld(cellPosition).x;
        Pos.z = grid.GetCellCenterWorld(cellPosition).z;
        MainCamera.transform.position = Pos;
    }
}
