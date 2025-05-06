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
            // Debug.Log("Base de datos no encontrada, creando nueva...");
        }
        PlayerPrefs.DeleteAll();
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
        // Debug.Log($"query: {query}");
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
        // Debug.Log($"query: {query}");
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
