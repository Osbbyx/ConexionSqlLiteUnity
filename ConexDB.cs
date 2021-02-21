using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;


public class ConexDB : MonoBehaviour
{
    string rutaDB;
    string strConexion;
    string DBFileName = "text.db";

    IDbConnection dbConnection;
    IDbCommand dbCommand;
    IDataReader reader;

    void Start()
    {
        AbrirDB();
    }

    void AbrirDB()
    {
        //crear y abrir ;a conexion
        rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        strConexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
        //crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "select * from Camisa;";
        dbCommand.CommandText = sqlQuery;
        //leer la base de datos
        reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            //id
            int id = reader.GetInt32(0);
            //marca
            string marca = reader.GetString(1);
            //color
            string color = reader.GetString(2);
            //cantidad
            int cantidad = reader.GetInt32(3);
            Debug.Log(id + " - " + marca + " - " + color + " - " + cantidad);
        }

        //cerrar las conexiones
        reader.Close();
        reader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    
}
