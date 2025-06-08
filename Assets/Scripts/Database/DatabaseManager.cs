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
        "Duralumín",
        "Atium",
    };

    int[][] metalRates = {
        new int[] {3,4,5}, // HIERRO
        new int[] {3,3,5}, // ACERO
        new int[] {2,4,2}, // ESTAÑO
        new int[] {7,3,5}, // PELTRE
        new int[] {7,2,5}, // ZINC
        new int[] {7,5,5}, // LATÓN
        new int[] {2,0,5}, // COBRE
        new int[] {3,2,5}, // BRONCE
        new int[] {10,5,5}, // CADMIO
        new int[] {10,5,5}, // BENDALEO
        new int[] {5,3,15}, // ORO
        new int[] {10,3,8}, // ELECTRO
        new int[] {5,2,5}, // CROMO
        new int[] {6,2,5}, // NICROSIL
        new int[] {10,2,5}, // ALUMINIO
        new int[] {10,2,5}, // DURALUMÍN
        new int[] {30,5,30} // ATIUM
    };

    float[][] fuentesCoords = {
        new float[] {-154.0f,-122.5f},
        new float[] {-116.0f,-74.5f},
        new float[] {226.0f,-74.5f},
        new float[] {-1.0f,45.5f},
        new float[] {-182.0f,-38.5f},
        new float[] {308.0f,9.5f},
        new float[] {-182.0f,81.5f},
        new float[] {88.0f,9.5f},
        new float[] {176.0f,-226.5f},
        new float[] {147.0f,237.5f},
        new float[] {583.0f,105.5f},
        new float[] {616.0f,-302.5f},
        new float[] {478.0f,-122.5f},
        new float[] {-60.0f,-266.5f}
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
        string dbFilePath = "database.db";  // nombre real del archivo

        bool databaseExists = File.Exists(dbFilePath);

        if (!databaseExists || IsEmptyDatabase()){
            Debug.Log("Base de datos no encontrada o vacía. Creando e inicializando...");
            CreateAndPopulateDatabase();
            isDatabaseInitialized = true;
        } else {
            Debug.Log("Base de datos encontrada.");
            isDatabaseInitialized = true;
        }
    }

    private bool IsEmptyDatabase(){
        try {
            int fileCount = GetInt("SELECT COUNT(*) FROM file;");
            return fileCount == 0;
        } catch {
            return true;
        }
    }

    private void CreateAndPopulateDatabase(){
        string[] setupCommands = new string[] {
            @"CREATE TABLE IF NOT EXISTS metals (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL
            );",

            @"CREATE TABLE IF NOT EXISTS checkpoint (
                id INTEGER PRIMARY KEY,
                x_pos FLOAT NOT NULL,
                y_pos FLOAT NOT NULL
            );",

            @"CREATE TABLE IF NOT EXISTS visited_checkpoint (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                file_id INTEGER NOT NULL,
                checkpoint_id INTEGER NOT NULL,
                FOREIGN KEY (file_id) REFERENCES file(id),
                FOREIGN KEY (checkpoint_id) REFERENCES checkpoint(id)
            );",

            @"CREATE TABLE IF NOT EXISTS file (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                max_health INTEGER DEFAULT 30,
                checkpoint_id INTEGER DEFAULT 0,
                damage INTEGER DEFAULT 8,
                vials INTEGER DEFAULT 2,
                vial_power INTEGER DEFAULT 15,
                phase TEXT DEFAULT 'silencio',
                phase_time INTEGER DEFAULT 0,
                coins INTEGER DEFAULT 0,
                laudano_bottles INTEGER DEFAULT 0,
                metal_vials INTEGER DEFAULT 1,
                description TEXT
            );",
            
            @"CREATE TABLE IF NOT EXISTS event (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                type TEXT NOT NULL,
                description TEXT
            );",
            
            @"CREATE TABLE IF NOT EXISTS unlocked_vial (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                file_id INTEGER NOT NULL,
                vial_id INTEGER NOT NULL,
                vial_type TEXT NOT NULL,
                unlocked INTEGER DEFAULT 0,
                FOREIGN KEY (file_id) REFERENCES file(id)
            );",

            @"CREATE TABLE IF NOT EXISTS achieved_event (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                file_id INTEGER NOT NULL,
                event_id INTEGER NOT NULL,
                FOREIGN KEY (file_id) REFERENCES file(id),
                FOREIGN KEY (event_id) REFERENCES event(id)
            );",

            @"CREATE TABLE IF NOT EXISTS metal_file (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                file_id INTEGER NOT NULL,
                metal_id INTEGER NOT NULL,
                unlocked_a INTEGER DEFAULT 0,
                unlocked_f INTEGER DEFAULT 0,
                unlocked_h INTEGER DEFAULT 0,
                slot_a INTEGER DEFAULT 0,
                slot_f INTEGER DEFAULT 0,
                slot_h INTEGER DEFAULT 0,
                capacity_a INTEGER DEFAULT 1000,
                capacity_f INTEGER DEFAULT 1000,
                raw_amount INTEGER DEFAULT 500,
                amount_a INTEGER DEFAULT 0,
                amount_f INTEGER DEFAULT 0,
                burning_rate INTEGER NOT NULL,
                storing_rate INTEGER NOT NULL,
                tapping_rate INTEGER NOT NULL,
                FOREIGN KEY (file_id) REFERENCES file(id),
                FOREIGN KEY (metal_id) REFERENCES metals(id)
            );",

            @"CREATE TABLE IF NOT EXISTS metal_vial_content (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                file_id INTEGER NOT NULL,
                metal_id INTEGER NOT NULL,
                vial_slot INTEGER NOT NULL,
                amount INTEGER NOT NULL,
                FOREIGN KEY (file_id) REFERENCES file(id),
                FOREIGN KEY (metal_id) REFERENCES metals(id)
            );",

            @"CREATE TABLE IF NOT EXISTS inventory (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                file_id INTEGER NOT NULL,
                slot INTEGER NOT NULL,
                slot_vial INTEGER DEFAULT 0,
                laudano INTEGER DEFAULT 0,
                proj_1 INTEGER DEFAULT 0,
                proj_2 INTEGER DEFAULT 0,
                proj_3 INTEGER DEFAULT 0,
                FOREIGN KEY (file_id) REFERENCES file(id)
            );"
        };

        ExecuteCommands(setupCommands);

        for (int i = 1; i <= metales.Length; i++) {
            ExecuteNonQuery($"INSERT INTO metals (name) VALUES ('{metales[i-1]}');");
        }

        for (int i = 0; i < fuentesCoords.Length; i++) {
            float x = fuentesCoords[i][0];
            float y = fuentesCoords[i][1];
            Debug.Log($"x: {x} / y: {y}");
            ExecuteNonQuery(
                "INSERT INTO checkpoint (id, x_pos, y_pos) VALUES (@param0, @param1, @param2);",
                i, x, y
            );
        }

        for (int i = 1; i <= 3; i++){
            ExecuteNonQuery($"INSERT INTO file (id) VALUES ({i})");

            for (int j = 1; j <= metales.Length; j++) {
                ExecuteNonQuery($"INSERT INTO metal_file (file_id,metal_id,burning_rate,storing_rate,tapping_rate) VALUES ({i},{j},{metalRates[j-1][0]},{metalRates[j-1][1]},{metalRates[j-1][2]});");
                if(j==11){
                    ExecuteNonQuery($"UPDATE metal_file SET unlocked_a = 1, slot_a = 1 WHERE file_id={i} AND metal_id={j}");
                }
            }
            for (int j = 1; j <= 4; j++){
                ExecuteNonQuery($"INSERT INTO unlocked_vial (file_id,vial_id,vial_type,unlocked) VALUES ({i},{j},'health',0)");
            }
            for (int j = 1; j <= 7; j++){
                ExecuteNonQuery($"INSERT INTO unlocked_vial (file_id,vial_id,vial_type,unlocked) VALUES ({i},{j},'metal',0)");
            }
            for (int j = 1; j <= 3; j++){
                ExecuteNonQuery($"INSERT INTO unlocked_vial (file_id,vial_id,vial_type,unlocked) VALUES ({i},{j},'max_health',0)");
            }
            for (int j = 1; j <= 8; j++){
                ExecuteNonQuery($"INSERT INTO inventory (file_id,slot) VALUES ({i},{j})");
            }
        }

        ExecuteNonQuery(
            @"INSERT INTO event (name,type,description) VALUES
                ('killed_koloss_volcano','boss','matar al koloss'),
                ('killed_kaenar','boss','matar a Kaenar')"
        );

        Debug.Log("Base de datos creada y eventos iniciales insertados.");
    }


    public bool IsDatabaseInitialized(){
        return isDatabaseInitialized;
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
        // Debug.Log($"ExecuteNonQuery query: {query}");
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
        // Debug.Log($"ExecuteScalar query: {query}");
        using (IDbConnection dbConnection = new SqliteConnection(dbName))
        {
            dbConnection.Open();
            using (IDbCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = query;
                AddParameters(command, parameters);
                object result = command.ExecuteScalar();
                // Debug.Log($"ExecuteScalar result: {result}");
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
                        result.Add(reader.GetInt32(0));
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

        return null;
    }
}
