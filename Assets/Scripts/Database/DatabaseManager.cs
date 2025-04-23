using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
//using Database;
using System.Collections;
using System.Collections.Generic;
using System;

public class DatabaseManager : MonoBehaviour
{
    string[] metales = {
        "Hierro",
        "Acero",
        "Estaño",
        "Peltre",
        "Zinc",
        "Latón",
        "Cobre",
        "Bronce",
        "Cadmio",
        "Bendaleo",
        "Oro",
        "Electro",
        "Cromo",
        "Nicrosil",
        "Aluminio",
        "Duraluminio",
        "Atium",
    };

    private static DatabaseManager _instance;
    private string dbName = "URI=file:database.db";
    private bool isDatabaseInitialized = false;

    public static DatabaseManager Instance{
        get{
            if (_instance == null){
                GameObject dbManager = new GameObject("DatabaseManager");
                _instance = dbManager.AddComponent<DatabaseManager>();
                DontDestroyOnLoad(dbManager);
            }
            return _instance;
        }
    }

    private void Awake(){
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase(){
        if (!File.Exists(dbName)){
            Debug.Log("Base de datos no encontrada, creando nueva...");
            CreateDB();
            PopulateDB();
        }
        SetDataPlayerPrefs();
    }
    
    public void SetDataPlayerPrefs(){
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Archivo", 1);
    }

    // void SetDataMetales(int archivo){
    //     for(int i = 0 ; i < metales.Length ; i++){
    //         string query = "SELECT ma.cantidad FROM metal_archivo ma " +
    //         "JOIN metales m ON ma.metal_id = m.id " +
    //         "WHERE ma.archivo_id = "+archivo+" AND m.nombre = '" + metales[i] + "';";

    //         object result = ExecuteScalar(query);

    //         if (result == null || result == DBNull.Value) {
    //             Debug.LogWarning("No se encontró cantidad para " + metales[i]);
    //             continue;
    //         }

    //         int capacity = Convert.ToInt32(result);

    //         PlayerPrefs.SetInt("max_" + metales[i], capacity);
    //         PlayerPrefs.SetInt(metales[i], capacity);

    //         Dictionary<string, object[]> metalData = GetMetalData(archivo);

    //         foreach (var entry in metalData){
    //             string metalNombre = entry.Key;
    //             object[] valores = entry.Value;

    //             // Asignar los valores booleanos desb_x
    //             PlayerPrefs.SetInt("desb_a_" + i, Convert.ToBoolean(valores[0]) ? 1 : 0);
    //             PlayerPrefs.SetInt("desb_f_" + i, Convert.ToBoolean(valores[1]) ? 1 : 0);
    //             PlayerPrefs.SetInt("desb_h_" + i, Convert.ToBoolean(valores[2]) ? 1 : 0);

    //             int metalSlotA = Convert.ToInt32(valores[3]); 
    //             int metalSlotF = Convert.ToInt32(valores[4]); 
    //             int metalSlotH = Convert.ToInt32(valores[5]);

    //             //Debug.Log("EL VALOR 3 ES EN "+metalNombre+": "+Convert.ToInt32(valores[3] ?? 0));
                
    //             PlayerPrefs.SetInt("slot_a_" + i, Convert.ToInt32(valores[3] ?? 0));
    //             PlayerPrefs.SetInt("slot_f_" + i, Convert.ToInt32(valores[4] ?? 0));
    //             PlayerPrefs.SetInt("slot_h_" + i, Convert.ToInt32(valores[5] ?? 0));
                
    //             //Debug.Log("Metal: " + metalNombre);
    //             //Debug.Log("Slot A: "+metalSlotA + " Slot F: "+metalSlotF + " Slot H: "+metalSlotH +" - ("+metalNombre+")");

    //             //ALOMANCIA
    //             // for(int j = 1 ; j <= 4; j++){
    //             //     if(!PlayerPrefs.HasKey("slot_a_"+j)){
    //             //         //Debug.Log("Añadiendo campo slot_a_"+j);
    //             //         PlayerPrefs.SetInt("slot_a_"+j , 0);
    //             //     }
    //             //     if(metalSlotA == j){
    //             //         //Debug.Log("El metal usa este slot a "+j);
    //             //         PlayerPrefs.SetInt("slot_a_"+j, i);
    //             //         //Debug.Log("SLOT A "+j+" IN PLAYERSCRIPT: "+PlayerPrefs.GetInt("slot_a_"+j));
    //             //     }
    //             // }
    //             // //FERUQUIMIA
    //             // for(int j = 1 ; j <= 2; j++){
    //             //     if(!PlayerPrefs.HasKey("slot_f_"+j)){
    //             //         //Debug.Log("Añadiendo campo slot_f_"+j);
    //             //         PlayerPrefs.SetInt("slot_f_"+j , 0);
    //             //     }
    //             //     if(metalSlotF == j){
    //             //         //Debug.Log("El metal usa este slot f "+j);
    //             //         PlayerPrefs.SetInt("slot_f_"+j, i);
    //             //         //Debug.Log("SLOT F "+j+"  IN PLAYERSCRIPT: "+PlayerPrefs.GetInt("slot_f_"+j));
    //             //     }
    //             // }
    //             // //HEMALURGIA
    //             // for(int j = 1 ; j <= 7; j++){
    //             //     if(!PlayerPrefs.HasKey("slot_h_"+j)){
    //             //         //Debug.Log("Añadiendo campo slot_h_"+j);
    //             //         PlayerPrefs.SetInt("slot_h_"+j , 0);
    //             //     }
    //             //     if(metalSlotH == j){
    //             //         //Debug.Log("El metal usa este slot h "+j);
    //             //         PlayerPrefs.SetInt("slot_h_"+j, i);
    //             //         //Debug.Log("SLOT H "+j+"  IN PLAYERSCRIPT: "+PlayerPrefs.GetInt("slot_h_"+j));
    //             //     }
    //             // }
    //             //Debug.Log("Terminando "+metalNombre);


    //             // Debug.Log($"METAL {metalNombre}: desbloqueado_a={valores[0]}, desbloqueado_f={valores[1]}, desbloqueado_h={valores[2]}, slot_a={valores[3]}, slot_f={valores[4]}, slot_h={valores[5]}");
    //         }
    //     }
    // }

    // private Dictionary<string, object[]> GetMetalData(int archivo){
    //     string[] columnas = { "desbloqueado_a", "desbloqueado_f", "desbloqueado_h", "slot_a", "slot_f", "slot_h" };
    //     string columnasConcatenadas = string.Join(", ", columnas);

    //     string query = $"SELECT metal_id, {columnasConcatenadas} FROM metal_archivo WHERE archivo_id = @param0";

    //     Dictionary<string, object[]> metalData = new Dictionary<string, object[]>();

    //     using (IDbConnection dbConnection = new SqliteConnection(dbName)){
    //         dbConnection.Open();
    //         using (IDbCommand command = dbConnection.CreateCommand()){
    //             command.CommandText = query;
    //             AddParameters(command, new object[] { archivo });

    //             using (IDataReader reader = command.ExecuteReader()){
    //                 while (reader.Read()){
    //                     int metalId = reader.GetInt32(0);
    //                     string metalNombre = metales[metalId - 1]; // Ajustamos índice (asumiendo que los IDs en la DB están ordenados)

    //                     object[] valores = new object[columnas.Length];
    //                     for (int i = 0; i < columnas.Length; i++){
    //                         valores[i] = reader.IsDBNull(i + 1) ? null : reader.GetValue(i + 1);
    //                     }

    //                     metalData[metalNombre] = valores;
    //                 }
    //             }
    //         }
    //         dbConnection.Close();
    //     }
    //     return metalData;
    // }


    public bool IsDatabaseInitialized(){
        return isDatabaseInitialized;
    }

    private void CreateDB(){
        string[] commands = {
            "CREATE TABLE IF NOT EXISTS archivo ("+
                "id INTEGER PRIMARY KEY AUTOINCREMENT, "+
                "salud_maxima INTEGER, "+
                "punto_control_id INTEGER"+
            ");",

            "CREATE TABLE IF NOT EXISTS metales ("+
                "id INTEGER PRIMARY KEY, "+
                "nombre TEXT UNIQUE"+
            ");",

            "CREATE TABLE IF NOT EXISTS objeto ("+
                "id INTEGER PRIMARY KEY, "+
                "nombre TEXT UNIQUE,"+
                "descripcion TEXT"+
            ");",

            "CREATE TABLE IF NOT EXISTS objeto_desbloqueado ("+
                "id INTEGER PRIMARY KEY AUTOINCREMENT, "+
                "archivo_id INTEGER, objeto_id INTEGER, "+
                "FOREIGN KEY (archivo_id) REFERENCES archivo(id), "+
                "FOREIGN KEY (objeto_id) REFERENCES objeto(id), "+
                "UNIQUE (archivo_id, objeto_id)"+
            ");",

            //Slot 0: no equipado, Slot 1-4, 1-2, 1-(5-7): equipado
            "CREATE TABLE IF NOT EXISTS metal_archivo ("+
                "id INTEGER PRIMARY KEY AUTOINCREMENT, "+
                "archivo_id INTEGER, metal_id INTEGER, "+
                "desbloqueado_a BOOLEAN DEFAULT 0, "+
                "desbloqueado_f BOOLEAN DEFAULT 0, "+
                "desbloqueado_h BOOLEAN DEFAULT 0, "+
                "slot_a INTEGER, "+
                "slot_f INTEGER, "+
                "slot_h INTEGER, "+
                "nombre_habilidad TEXT, cantidad INTEGER DEFAULT 0, "+
                "FOREIGN KEY (archivo_id) REFERENCES archivo(id), "+
                "FOREIGN KEY (metal_id) REFERENCES metales(id), "+
                "UNIQUE (archivo_id, metal_id)"+
            ");",

            "CREATE TABLE IF NOT EXISTS punto_control ("+
                "id INTEGER PRIMARY KEY AUTOINCREMENT, "+
                "nombre TEXT, "+
                "posicion_x REAL, "+
                "posicion_y REAL"+
            ");",

            "CREATE TABLE IF NOT EXISTS punto_control_visitado ("+
                "id INTEGER PRIMARY KEY AUTOINCREMENT, "+
                "archivo_id INTEGER, "+
                "punto_control_id INTEGER, "+
                "FOREIGN KEY (archivo_id) REFERENCES archivo(id), "+
                "FOREIGN KEY (punto_control_id) REFERENCES punto_control(id)"+
            ");"
        };

        ExecuteCommands(commands);
    }

    private void PopulateDB(){
        // Verificar si la tabla metales ya tiene datos
        int countMetales = (int)(long)ExecuteScalar("SELECT COUNT(*) FROM metales;");
        if (countMetales == 0){
            string[] insertMetales = {
                "INSERT INTO metales (id, nombre) VALUES (1, 'Hierro');",
                "INSERT INTO metales (id, nombre) VALUES (2, 'Acero');",
                "INSERT INTO metales (id, nombre) VALUES (3, 'Estaño');",
                "INSERT INTO metales (id, nombre) VALUES (4, 'Peltre');",
                "INSERT INTO metales (id, nombre) VALUES (5, 'Cobre');",
                "INSERT INTO metales (id, nombre) VALUES (6, 'Bronce');",
                "INSERT INTO metales (id, nombre) VALUES (7, 'Zinc');",
                "INSERT INTO metales (id, nombre) VALUES (8, 'Latón');",
                "INSERT INTO metales (id, nombre) VALUES (9, 'Cadmio');",
                "INSERT INTO metales (id, nombre) VALUES (10, 'Bendaleo');",
                "INSERT INTO metales (id, nombre) VALUES (11, 'Oro');",
                "INSERT INTO metales (id, nombre) VALUES (12, 'Electro');",
                "INSERT INTO metales (id, nombre) VALUES (13, 'Cromo');",
                "INSERT INTO metales (id, nombre) VALUES (14, 'Nicrosil');",
                "INSERT INTO metales (id, nombre) VALUES (15, 'Aluminio');",
                "INSERT INTO metales (id, nombre) VALUES (16, 'Duraluminio');",
                "INSERT INTO metales (id, nombre) VALUES (17, 'Atium');"
            };
            ExecuteCommands(insertMetales);
            // Debug.Log("Metales insertados.");
        } else {
            // Debug.Log("Los metales ya estaban insertados.");
        }

        // Verificar si la tabla objeto ya tiene datos
        int countObjetos = (int)(long)ExecuteScalar("SELECT COUNT(*) FROM objeto;");
        if (countObjetos == 0){
            string[] insertObjetos = {
                "INSERT INTO objeto (id, nombre) VALUES (1, 'Vial de metal');",
                "INSERT INTO objeto (id, nombre) VALUES (2, 'Mapa de Luthadel');",
                "INSERT INTO objeto (id, nombre) VALUES (3, 'Llave oxidada');",
                "INSERT INTO objeto (id, nombre) VALUES (4, 'Pergamino antiguo');",
                "INSERT INTO objeto (id, nombre) VALUES (5, 'Moneda de cobre');"
            };
            ExecuteCommands(insertObjetos);
            // Debug.Log("Objetos insertados.");
        } else {
            // Debug.Log("Los objetos ya estaban insertados.");
        }

        // int countPuntoControl = (int)(long)ExecuteScalar("SELECT COUNT(*) FROM punto_control;");
        // if (countPuntoControl == 0){
        //     string[] insertPuntoControl = {
        //         "INSERT INTO punto_control (id, posicion_x, posicion_y) VALUES (1, -2.99, 1.96);",
        //         "INSERT INTO punto_control (id, posicion_x, posicion_y) VALUES (2, 28.6, 23);"
        //     };
        //     ExecuteCommands(insertPuntoControl);
        //     Debug.Log("PuntoControl insertados.");
        // } else {
        //     Debug.Log("Los PuntoControl ya estaban insertados.");
        // }
        
        int eventos = (int)(long)ExecuteScalar("SELECT COUNT(*) FROM evento;");
        if (eventos == 0){
            string[] insertEventos = {
                "INSERT INTO evento (id,nombre,descripcion,tipo) VALUES"+
                "(1,'Carne fresca','matar al carnicero de Hathsin','boss'),"+
                "(2,'Redundancia','leer dos veces Ruina en el menú de hemalurgia','logro'),"+
                "(3,'El Secreto de los Obligadores','descubrir documentos ocultos sobre la hemalurgia','evento')"
            };
            ExecuteCommands(insertEventos);
            // Debug.Log("Eventos insertados.");
        } else {
            // Debug.Log("Los Eventos ya estaban insertados.");
        }

        int countArchivos = (int)(long)ExecuteScalar("SELECT COUNT(*) FROM archivo;");
        if (countArchivos == 0){
            ExecuteNonQuery("INSERT INTO archivo (salud_maxima, punto_control_id) VALUES (100, NULL);");
            Debug.Log("Archivo inicial creado.");
        } else {
            Debug.Log("Ya existe al menos un archivo.");
        }

        int archivoId = (int)(long)ExecuteScalar("SELECT id FROM archivo ORDER BY id ASC LIMIT 1;");

        int countMetalArchivo = (int)(long)ExecuteScalar("SELECT COUNT(*) FROM metal_archivo WHERE archivo_id = @param0;", archivoId);
        if (countMetalArchivo == 0){
            for (int metalId = 1; metalId <= 17; metalId++){
                ExecuteNonQuery("INSERT INTO metal_archivo (archivo_id, metal_id, cantidad) VALUES (@param0, @param1, 60);", archivoId, metalId);
            }
            // Debug.Log("Registros iniciales en metal_archivo añadidos.");
        } else {
            // Debug.Log("Los metales ya estaban registrados en metal_archivo.");
        }

        int countObjetosDesbloqueados = (int)(long)ExecuteScalar("SELECT COUNT(*) FROM objeto_desbloqueado WHERE archivo_id = @param0;", archivoId);
        if (countObjetosDesbloqueados == 0){
            ExecuteNonQuery("INSERT INTO objeto_desbloqueado (archivo_id, objeto_id) VALUES (@param0, @param1);", archivoId, 1); // Vial de metal
            ExecuteNonQuery("INSERT INTO objeto_desbloqueado (archivo_id, objeto_id) VALUES (@param0, @param1);", archivoId, 2); // Mapa de Luthadel
            // Debug.Log("Registros iniciales en objeto_desbloqueado añadidos.");
        } else {
            // Debug.Log("Los objetos iniciales ya estaban desbloqueados.");
        }
    }

    public void ExecuteCommands(string[] commands){
        using (IDbConnection dbConnection = new SqliteConnection(dbName))
        {
            dbConnection.Open();
            foreach (var commandText in commands)
            {
                using (IDbCommand command = dbConnection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }
            dbConnection.Close();
        }
    }

    public void ExecuteNonQuery(string query, params object[] parameters){
        using (IDbConnection dbConnection = new SqliteConnection(dbName))
        {
            dbConnection.Open();
            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = query;
                AddParameters(command, parameters);
                command.ExecuteNonQuery();
            }
            dbConnection.Close();
        }
    }

    public object ExecuteScalar(string query, params object[] parameters){
        using (IDbConnection dbConnection = new SqliteConnection(dbName))
        {
            dbConnection.Open();
            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = query;
                AddParameters(command, parameters);
                object result = command.ExecuteScalar();
                // Debug.Log("RESULT: " + result);
                dbConnection.Close();
                return result;
            }
        }
    }

    private void AddParameters(IDbCommand command, object[] parameters){
        for (int i = 0; i < parameters.Length; i++)
        {
            command.Parameters.Add(new SqliteParameter($"@param{i}", parameters[i]));
        }
    }

    public int GetInt(string query){
        object result = ExecuteScalar(query);

        if (result == null || result == DBNull.Value) {
            Debug.LogWarning("Error al conseguir el INTEGER");
            return -1;
        }

        return Convert.ToInt32(result);
    }

    public string GetString(string query){
        using (IDbConnection dbConnection = new SqliteConnection(dbName)){
            dbConnection.Open();
            using (IDbCommand command = dbConnection.CreateCommand()){
                command.CommandText = query;
                object result = command.ExecuteScalar();
                dbConnection.Close();
                return result != null && result != DBNull.Value ? result.ToString() : null;
            }
        }
    }

    public List<int> GetIntListFromQuery(string query){
        List<int> result = new List<int>();

        using (var connection = new SqliteConnection(dbName)){
            connection.Open();
            using (var command = connection.CreateCommand()){
                command.CommandText = query;
                using (var reader = command.ExecuteReader()){
                    while (reader.Read()){
                        result.Add(reader.GetInt32(0)); // Primera columna como entero
                    }
                }
            }
        }

        return result;
    }

    public List<Dictionary<string, object>> GetRowsFromQuery(string query, params object[] parameters) {
        List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

        using (var connection = new SqliteConnection(dbName)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = query;
                AddParameters(command, parameters);

                using (var reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++) {
                            string columnName = reader.GetName(i);
                            object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            row[columnName] = value;
                        }
                        result.Add(row);
                    }
                }
            }
        }

        return result;
    }

    public Dictionary<string, object> GetSingleRowFromQuery(string query, params object[] parameters) {
        using (var connection = new SqliteConnection(dbName)) {
            connection.Open();
            using (var command = connection.CreateCommand()) {
                command.CommandText = query;
                AddParameters(command, parameters);

                using (var reader = command.ExecuteReader()) {
                    if (reader.Read()) {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++) {
                            string columnName = reader.GetName(i);
                            object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            row[columnName] = value;
                        }
                        return row;
                    }
                }
            }
        }

        return null; // No se encontró ninguna fila
    }
}
