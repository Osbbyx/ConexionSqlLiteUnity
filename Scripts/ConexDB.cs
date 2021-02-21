using System.Collections;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class ConexDB : MonoBehaviour
{
    string rutaDB;
    string strConexion;
    string DBFileName = "text.db";
    public Text myText;

    IDbConnection dbConnection;
    IDbCommand dbCommand;
    IDataReader reader;

    void Start()
    {
        AbrirDB();
    }

    void AbrirDB()
    {
        //1- crear y abrir la conexion
        //Compruebo que plataforma es
        if(Application.platform == RuntimePlatform.WindowsEditor)
        { 
            rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        }
        else if(Application.platform == RuntimePlatform.Android)
        { 
            //con persistent data el entra directamente al StreamingAssets, asi que no hay que ponerlo.
            myText.text = "Llegue";
            rutaDB = Application.persistentDataPath + "/" + DBFileName;
            //comprobar si el archivo se encuentra almacenado en persistant data
             if (!File.Exists(rutaDB))
             {
            StartCoroutine(CopyDBAndroid());   
            }
        }
    
        
        strConexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
        //2- crear la consulta
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "select * from Camisa;";
        dbCommand.CommandText = sqlQuery;
        //3- leer la base de datos
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
            myText.text = id.ToString() + " - " + marca + " - " + color + " - " + cantidad.ToString();
        }

        //cerrar las conexiones
        reader.Close();
        reader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    IEnumerator CopyDBAndroid()
    {
        UnityWebRequest uwr = new UnityWebRequest("jar:file://" + Application.dataPath + "!/assets/" + DBFileName);
        uwr.downloadHandler = new DownloadHandlerBuffer();

        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError || uwr.isHttpError)
        {
            myText.text = uwr.error;
        }
        else
        {
            File.WriteAllBytes(rutaDB, uwr.downloadHandler.data);
        }
    }
}
