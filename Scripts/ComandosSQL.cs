using System.Collections;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class ComandosSQL : MonoBehaviour
{
    string rutaDB;
    string strConexion;
    string DBFileName = "chinook.db";
    public Text myText;

    IDbConnection dbConnection;
    IDbCommand dbCommand;
    IDataReader reader;

    void Start()
    {
        AbrirDB();
        //Comando_LIBRE("select * from albums where AlbumId = 1");
        //Comando_SELECT("*","albums");
        //Comando_WHERE("*", "albums", "ArtistId", "=" , "2");
        /* Comando_WHERE_AND("*",
                         "albums", 
                         "ArtistId", "=", "2",
                         "AlbumId",">", "2");*/
        /* Comando_WHERE_AND_ORDERBY("*",
                                   "albums",
                                   "ArtistId", "=", "2",
                                   "AlbumId", ">", "1", 
                                   "AlbumId" , "desc"); // or asc*/
        //LeyendoDatos();
        // CerrarDB();
        // FuncCount();
        // ContandoDB();
        //INSERT("KLK");
        // UPDATE();
        DELETE();
        CerrarContador();
    }

    //------------------------------------------------------------------------------------------------------------------------
    //Metodo Bases

    //1- crear y abrir la conexion
    void AbrirDB()
    {
        rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName;
        strConexion = "URI=file:" + rutaDB;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
    }

    //3- leer la base de datos
    void LeyendoDatos()
    {
        reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            try
            {
                Debug.Log(reader.GetInt32(0) + " -- " + reader.GetString(1) + " -- "  + reader.GetInt32(2));
            }
            catch (FormatException fe)
            {
                Debug.LogError(fe.Message);
                continue;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                continue;
            }
        }
    }

    //4-cerrar las conexiones
    void CerrarDB()
    {
        reader.Close();
        reader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }



    //---------------------------------------------------------------------------------------------------------------------------

    //metodos creados para las funciones  y el INSERT UPDATE Y DELETE , ya que necesitan una pequena modificacion, 
    //para todos los demas los metodos base de arriba
    void ContandoDB()
    {
        int FilaCont = Convert.ToInt32(dbCommand.ExecuteScalar());
            try
            {
                Debug.Log(FilaCont);
            }
            catch (FormatException fe)
            {
                Debug.LogError(fe.Message);
                throw;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }  
    }

    void LeyendoDoubleDB()
    {
        double FilaCont = Convert.ToDouble(dbCommand.ExecuteScalar());
        try
        {
            Debug.Log(FilaCont);
        }
        catch (FormatException fe)
        {
            Debug.LogError(fe.Message);
            throw;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    
    void CerrarContador()
    {
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    //----------------------------------------------------------------------------------------------------------------------------------


    //usamos leyendodatos y cerrandoDB

    //2- crear la consultas
    void Comando_LIBRE(string sqlQuery)
    {
        dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = sqlQuery;
    }

    void Comando_SELECT(string item, 
                        string tabla)
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT " + item + 
                          " FROM " + tabla;
        dbCommand.CommandText = sqlQuery;
    }


    void Comando_WHERE(string item, 
                       string tabla, 
                       string campo , string comparador, string dato)
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT " + item + 
                          " FROM " + tabla + 
                          " WHERE " + campo+ " " + comparador + " " + dato;
        dbCommand.CommandText = sqlQuery;
    }

    void Comando_WHERE_AND(string item,
                           string tabla, 
                           string campo, string comparador, string dato,
                           string campo2, string comparador2, string dato2)
    {
        dbCommand = dbConnection.CreateCommand();
       
        string sqlQuery =   "SELECT " + item +
                            " FROM " + tabla + 
                            " WHERE " + campo + " " + comparador + " " + dato +
                            " AND " + campo2 + " " + comparador2 + " " + dato2;
        
        dbCommand.CommandText = sqlQuery;
    }

    void Comando_WHERE_AND_ORDERBY( string item,
                                    string tabla,
                                    string campo, string comparador, string dato,
                                    string campo2, string comparador2, string dato2,
                                    string campo3 , string orden)
    {
        dbCommand = dbConnection.CreateCommand();

        string sqlQuery = "SELECT " + item +
                            " FROM " + tabla +
                            " WHERE " + campo + " " + comparador + " " + dato +
                            " AND " + campo2 + " " + comparador2 + " " + dato2 +
                            " ORDER BY " + campo3 + " " + orden.ToUpper();

        dbCommand.CommandText = sqlQuery;
    }

    void ComandoIN(string item,
                   string tabla,
                   string campo, string dato1, string dato2)
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT " + item +
                          " FROM " + tabla +
                          " WHERE " + campo + " " +  " IN " + "(" + '"' + dato1 + '"' + "," + '"' + dato2 + '"';
        dbCommand.CommandText = sqlQuery;
    }

    //Like, Between , As , Limit, Left Join , Right Join , Inner Join, Is Null , etc



    //-------------------------------------------------------------------------------------------------------------------------

    //Ahora vamos con las funciones en base de datos
    //usamos cerrarcontador y contadorDB
    void FuncCount()
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT COUNT(*) FROM customers WHERE Country = " + '"' + "Canada" + '"';
        dbCommand.CommandText = sqlQuery;
    }

    // aqui ccerarcontador y LeyendoDoubleDB
    void FuncAVG()
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT AVG(Total) FROM invoices";
        dbCommand.CommandText = sqlQuery;
    }

    // SUM , MIN , MAX , etc de la misma forma 





    //---------------------------------------------------------------------------------------------------------------------------------------
    //usamos cerrar contador con todo ellos
    void INSERT(string dato)
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = String.Format("INSERT INTO media_types(Name) VALUES(\"{0}\")",dato);
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteScalar();
    }

    void UPDATE()
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "UPDATE media_types SET Name = \"KLWA!!\" WHERE MediaTypeId = 9" ;
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteScalar();
    }


    void DELETE()
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "DELETE FROM media_types WHERE MediaTypeId = 8";
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteScalar();
    }
}
