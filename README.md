# ConexionSqlLiteUnity 
<h1>**Conexion SQLite a Unity (SOLVED)**</h1>

<h3>1- Entrar a la pagina web oficial de SQLite, descargar la version x84 y x64.</h3> <br> 
<h3>2- Crea la base de datos , y dos carpetas en assets, una Plugins y otra StreamingAssets, dentro de StreamingAssets se pone la base de datos, en plugins creamos
3 sub carpetas x84 , x64 , y Android, ponen el sql 84 y 64 donde corresponden.</h3><br>
<h3>3- En plugins hay que poner el MonoData, ese lo descargan de mi repositorio y en android van 2 sub carpetas con sus respectivas confifuraciones, igual lo descarga
de mi repositorio, ya que el MonoData que trae unity no funciona, y el plugin de Android es complicado, aqui tienen una version simplificada.</h3> <br>
<h3>4-En assets creas el codigo para conectar la base de datos y hacer las consultas. </h3><br>

<h1>Script</h1>
<h3>Primero hay que llamar las librerias</h3>
<h4> NOTA: Antes habia que colocar el SystemData, pero ya unity lo trae por defecto despues de la version 2018.</h4>

``` C#
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.Networking; // para el UnityWebRequest
using UnityEngine.UI; // para el texto de prueba
using System.IO; // para el File


public class ConexDB : MonoBehaviour
{
    //Aqui colocaremos la ruta en donde se encuentra la base de datos
    string rutaDB;
    /*Aqui completaremos el URI, ya que puede variar la direccion, lo divido en dos y 
    no lo pongo todo en ruta, para asi cambiarlo dependiendo del dispositivo*/
    string strConexion;
    //Aqui simplemente ponemos el nombre de la base de dataos y su extension.
    string DBFileName = "text.db";

    //Creamos las variables detipo IDB para controlar la connection , que dira que base de datos es, 
    //el command que es la consulta que pediremos, y el reader que es la que leera la base de datos
    IDbConnection dbConnection;
    IDbCommand dbCommand;
    IDataReader reader;

    void Start()
    {
    //metodo que creamos fuera para abrir la basi de datos al empezar la app
        AbrirDB();
    }

    void AbrirDB()
    {
        1-//crear y abrir la conexion
        //Compruebo que plataforma es
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
           
            rutaDB = Application.dataPath + "/StreamingAssets/" + DBFileName; //aplication.datapath nos da la ruta
        //empezando desde assets, ponemos la carpeta donde esta, y por ultimo el nombre del la base de datos
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            
            //con persistent data el entra directamente al StreamingAssets, asi que no hay que ponerlo.
            rutaDB = Application.persistentDataPath + "/" + DBFileName;
            //comprobar si el archivo se encuentra almacenado en persistant data
             if (!File.Exists(rutaDB))
             {
             //coroutina que hace todo en caso de no existir
            StartCoroutine(CopyDBAndroid());
            }
        }
        
        strConexion = "URI=file:" + rutaDB; // aqui como dijimos ponemos el URI correspondiente, 
                                            //en el caso de la pc URI=file:
        dbConnection = new SqliteConnection(strConexion); // aqui ya llamamos al constructor de sqlite 
                                                          //y le mandamos la ruta completa.
        dbConnection.Open(); // llamamos el metodo abrir, para que abra la base de datos
        
        2-//crear la consulta
        dbCommand = dbConnection.CreateCommand(); //ya como esta abierta la db, puedo hacer comandos, por 
                                                  //lo que llamo el metodo CreateCommand del dbConnection.
        string sqlQuery = "SELECT * FROM Camisa;"; // ya como el dbCommand esta esperando la consulta, guardamos 
                                                   //en un string guardo el comando o consulta deseada.
        dbCommand.CommandText = sqlQuery; // y al metodo setter CommandText de dbCommand le pasamos el valor 
                                          //que es la consulta que guardamos en sqlQuery.
        
        3-//leer la base de datos
        reader = dbCommand.ExecuteReader();  // ahora necesitamos leer, en la variable reader llamamos el 
                                             // metodo ExecureReader del dbCommand para almacenarlo ahi.
        while (reader.Read()) // con un while  recorremos el resultado y hacemos lo que queramos con los datos.
        {
            //id
            int id = reader.GetInt32(0); /*los datos que el va a leer hay que guardarlos en variables,
                                         por lo llamamoslo recibido y el metodo correspondiente al
                                         tipo de dato de dicho valor recibido, y entre los parentesis 
                                         se pone el orden en que se recibe, si el id es el primero
                                         en salir pues se pone 0 ya que empieza desde 0 como casi todo 
                                         en c#, y asi sucesivamente con todo lo demas.*/
            //marca
            string marca = reader.GetString(1);
            //color
            string color = reader.GetString(2);
            //cantidad
            int cantidad = reader.GetInt32(3);
            Debug.Log(id + " - " + marca + " - " + color + " - " + cantidad);
        }

       4- //cerrar las conexiones
        //Por ultimo una vez ya la base de datos hizo lo que tenia que hacer, hay que cerrar todo,
        //para eso llamamos elmetodo close en el reader y connection
        //y en el command le llamamos el Dispose(), y completamos poniendole de valor null a las 
        //3 variables.
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

    
}
```

<h2>Ahora que entendemos el codigo, le dejo el codigo base organizado</h2>
<br>

``` C#
   string strConexion;
    string DBFileName = "text.db";
    public Text myText;

    IDbConnection dbConnection;
    IDbCommand dbCommand;
    IDataReader reader;

    void Start()
    {
        AbrirDB();
        ComandoSelect("*","Camisa");
        LeyendoDatos();
        CerrarDB();
    }

    //1- crear y abrir la conexion
    void AbrirDB()
    {
        strConexion = "URI=file:" + Application.dataPath + "/StreamingAssets/" + DBFileName;
        dbConnection = new SqliteConnection(strConexion);
        dbConnection.Open();
    }

    //2- crear la consulta
    void ComandoSelect(string item, string tabla)
    {
        dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT " + item + " FROM " + tabla;
        dbCommand.CommandText = sqlQuery; 
    }

    //3- leer la base de datos
    void LeyendoDatos()
    {
        reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            Debug.Log(reader.GetInt32(0) + " -- " + reader.GetString(1) + " -- " + reader.GetString(2) + " -- " + reader.GetInt32(3));
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
```

