using System.Collections.Generic;

[System.Serializable]
public class Objeto{
    int id;
    string nombre;
    string descripcion;

    public Objeto(int i, string n, string d) {
        id = i;
        nombre = n;
        descripcion = d;
    }

    public void SetId(int i){
        id = i;
    }
    public int GetId(){
        return id;
    }
    
    public void SetNombre(string n){
        nombre = n;
    }
    public string GetNombre(){
        return nombre;
    }
    
    public void SetDescripcion(string d){
        descripcion = d;
    }
    public string GetDescripcion(){
        return descripcion;
    }
}
