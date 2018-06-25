using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class tablero : MonoBehaviour
{

    int x = 5;//AssemblyCSharp.PlayerInfo.Instance.matrix_size;
    int y = 5;//AssemblyCSharp.PlayerInfo.Instance.matrix_size;
    public float SeperationValueX = 0.001f; // Distance between each column
    public float SeperationValueZ = 0.001f; // Distance between each Row

    public float tempSepX = 0; // used to calculate the separation between each column
    public float tempSepZ = 0;// used to calculate the separation between each row
    double[,] matrix = new double[5, 5];//AssemblyCSharp.PlayerInfo.Instance.matrix_size, AssemblyCSharp.PlayerInfo.Instance.matrix_size];//matriz que se debe de dibujar
    public Texture3D texture;
    public Material paredMat;
    public Material pisoMat;
    public Cell prefab;

    public static Material matPiso;
    public static Material matPared;
    double[,] matrixM;
    int user=1;
    List<MJson> obj;


    public GameObject gmo = null;
    // Use this for initialization
    void Start()
    {
        //  Debug.Log(3);
        matPiso = paredMat;
        matPared = pisoMat;
        createGrid();//call the createGrid function on start
    }

    // Update is called once per frame
    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))//LLamar cuando selibera una posición
            AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace(1, 1);//Pruebas Ignorar
        else if (Input.GetKeyDown(KeyCode.W))
            AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace(0, 0);//Pruebas Ignorar
        else if (Input.GetKeyDown(KeyCode.L))
            AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace(2, 2);//Pruebas Ignorar
        else if (AssemblyCSharp.PlayerInfo.Instance.read_winner)
        {//Cuando se conecta un ganador eliminar conexión y pasar a escenar de victoria o derrota
            Debug.LogFormat("Winner: {0}", AssemblyCSharp.PlayerInfo.Instance.player_winner);//player_winner si es el ID del ganador
            if (AssemblyCSharp.PlayerInfo.Instance.player_id == AssemblyCSharp.PlayerInfo.Instance.player_winner)
            {
                SceneManager.LoadScene(5);
            }
            else
            {
                SceneManager.LoadScene(6);
            }
            AssemblyCSharp.PlayerInfo.Instance.endConnection(); //Borra conexión
            Debug.Log("Disconnected");
        }
        else if (AssemblyCSharp.PlayerInfo.Instance.alertar_forzado)
        { // Cuando se te desconecta por X motivos
            Debug.Log(AssemblyCSharp.PlayerInfo.Instance.message.ToString()); //Mensaje a mostrar
            AssemblyCSharp.PlayerInfo.Instance.endConnection(); //Eliminar conexión
            Debug.Log("Disconnected");
        }
        else if (AssemblyCSharp.PlayerInfo.Instance.actualizar_map)
        {//Cuando se te da permiso de liberar
            Debug.LogFormat("Liberar X: {0}", AssemblyCSharp.PlayerInfo.Instance.liberar_x);
            Debug.LogFormat("Liberar Y: {0}", AssemblyCSharp.PlayerInfo.Instance.liberar_y);
            double pared = matrix[AssemblyCSharp.PlayerInfo.Instance.liberar_x, AssemblyCSharp.PlayerInfo.Instance.liberar_y];
			string pname = AssemblyCSharp.PlayerInfo.Instance.liberar_x.ToString () + " " + AssemblyCSharp.PlayerInfo.Instance.liberar_y.ToString () + " " + pared.ToString ();
            gmo = GameObject.Find(pname);
            if (pared == 1)
			{
				//this.GetComponent<Renderer>().material = tablero.matPared;
            }
            else
			{
				Destroy(gmo);
				// this.GetComponent<Renderer>().material = tablero.matPiso;
				//  gameObject.GetComponent<Renderer>().enabled = false;
            }

            AssemblyCSharp.PlayerInfo.Instance.actualizar_map = false;
        }
        else if (AssemblyCSharp.PlayerInfo.Instance.alertar_jugador)
        {//Cuando NO se te da permiso de liberar
            Debug.Log("MAL PE");
            AssemblyCSharp.PlayerInfo.Instance.alertar_jugador = false;
        }
        AssemblyCSharp.PlayerInfo.Instance.sendPosition();//Siempre enviar posición del usuario
    }
    */
    public class Tablero
    {
        public int posX, posY;
        public double[,] matrix; //matriz que se debe copiar
        public Cell[,] cellMatrix;

        public Sprite free;
        public Sprite bomb;
        public Sprite full;

        //para poder usar los sprites definidos arriba
        public void changeSpriteFree(Sprite free_)
        {
            free = free_;
        }
        public void changeSpriteBomb(Sprite bomb_)
        {
            bomb = bomb_;
        }
        public void changeSpriteFull(Sprite full_)
        {
            full = full_;
        }
        /****************************************/
        public Tablero(int x, int y, Sprite mine, Sprite full, Sprite free)
        {
            posX = x;
            posY = y;
            changeSpriteFree(free);
            changeSpriteBomb(mine);
            changeSpriteFull(full);
        }

        public void init()
        { //dibuja la matriz desde el inicio
            CellMatrixLoop((i, j) =>
            {
                cellMatrix[i, j].Init(new Vector2Int(i, j),
                (matrix[i, j] != (-1) ? false : true),
                Activate);
                cellMatrix[i, j].sprite = full;
            });
        }

        //funcion para onclick
        void Activate(int i, int j)
        {
            if (cellMatrix[i, j].showed)
                return;
            cellMatrix[i, j].showed = true;

            if (cellMatrix[i, j].mine)
            {
                //acaba el juego
                cellMatrix[i, j].sprite = bomb;
                Debug.Log("Mina");
                //StartCoroutine(your_timer()); //Delay
                init();
                //volver a jugar
            }
            else
            {
                cellMatrix[i, j].sprite = free;
                
                cellMatrix[i, j].text = (matrix[i, j]).ToString();

            }
        }
        void ActivateArround(int i, int j)
        {
            if (PointIsInsideMatrix(i + 1, j))
                Activate(i + 1, j);
            if (PointIsInsideMatrix(i, j + 1))
                Activate(i, j + 1);
            if (PointIsInsideMatrix(i + 1, j + 1))
                Activate(i + 1, j + 1);
            if (PointIsInsideMatrix(i - 1, j))
                Activate(i - 1, j);
            if (PointIsInsideMatrix(i, j - 1))
                Activate(i, j - 1);
            if (PointIsInsideMatrix(i - 1, j - 1))
                Activate(i - 1, j - 1);
            if (PointIsInsideMatrix(i - 1, j + 1))
                Activate(i - 1, j + 1);
            if (PointIsInsideMatrix(i + 1, j - 1))
                Activate(i + 1, j - 1);
        }
        public void CellMatrixLoop(Action<int, int> e)//recibir en un parametro una funcion
        {
            for (int i = 0; i < cellMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < cellMatrix.GetLength(1); j++)
                {
                    e(i, j);//esta funcion dibujara el cuadrado
                }
            }
        }
        bool PointIsInsideMatrix(int i, int j)
        {
            if (i >= cellMatrix.GetLength(0))
                return false;
            if (i < 0)
                return false;
            if (j >= cellMatrix.GetLength(1))
                return false;
            if (j < 0)
                return false;

            return true;
        }
    }

    public class MJson
    {
        public int row { get; set; }
        public int col { get; set; }
        public int content { get; set; }
    }

    void jsonToMatriz()
    {
		while (AssemblyCSharp.PlayerInfo.Instance.read_map == false);
        String data = AssemblyCSharp.PlayerInfo.Instance.map_data;
        Debug.Log(data.ToString());
        obj = JsonConvert.DeserializeObject<List<MJson>>(data);
        for (int i = 0; i < AssemblyCSharp.PlayerInfo.Instance.matrix_size * AssemblyCSharp.PlayerInfo.Instance.matrix_size; i++)
        {
            matrix[obj[i].row, obj[i].col] = obj[i].content;

            Debug.LogFormat("X: {0}",obj[i].row );
            Debug.LogFormat("Y: {0}", obj[i].col);
            Debug.LogFormat("Value: {0}", obj[i].content);
        }
/*
        matrix = new double[,] { { -1, -1, 3, -1, 2 }, { 2, 1, 4, 1, 2 },
                              { 3, 1, 6, 3, 2 }, { 3, 1, 1, 1, 1 },
                              { 2, 1, 4, 2, -1 } };
        int size = x;

        //matrixM = inicializeMat(size, size, 0, 0);

        user = 1;//data.Length;
        if (user != 1)
        {
            matrix = Mirror(matrix, size, user);
            printMat(matrix, size);
        }*/

    }


    void createGrid()
    {
        jsonToMatriz();

        for (float i = 0; i < x; i += 1)
        {
            for (float j = 0; j < y; j += 1)
            {
                     //user 2 = [size-1,0]
             //user 3 = [size-1,size-1]
             //user 4 = [0,size-1]

                if (user == 1 && i == 0 && j == 0)
                    continue;
                if (user == 2 && i == 0 && j == x - 1)
                    continue;
                if (user == 3 && i == x - 1 && j == x - 1)
                    continue;
                if (user == 4 && i == x - 1 && j == 0)
                    continue;

                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Cube); //create a quad primitive as provided by unity
                plane.transform.position = new Vector3(i * 1.05f, 0, 1.05f * j); //position the newly created quad accordingly
                plane.transform.eulerAngles = new Vector3(90f, 0, 0); //rotate the quads 90 degrees in X to face up

                plane.name = ((int)i).ToString() + " " + ((int)j).ToString() + " " + ((int)matrix[(int)i, (int)j]).ToString();

                if (matrix[(int)i, (int)j] < 0)
                {
                    plane.name = i.ToString() + " " + j.ToString() + " 1";
                    Debug.Log(plane.name);
                }
                plane.AddComponent<Hide>();


            }
            tempSepZ = 0;
        }
    }
    void printMat(double[,] matA, int n)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Debug.Log(matA[i, j]);
            }
            Debug.Log("");

        }
        Debug.Log("---------------------------");

    }

    double[,] Mirror(double[,] matA, int n, int idPlayer)
    {
        double[,] matB = new double[n, n];


        if (idPlayer == 2)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matB[i, j] = matA[i, n - j - 1];
                }
            }
        }
        else if (idPlayer == 4)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matB[i, j] = matA[n - i - 1, j];
                }
            }
        }
        else if (idPlayer == 3)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matB[i, j] = matA[n - i - 1, n - j - 1];
                }
            }
        }
        return matB;
    }


    double[,] inicializeMat(int mRows, int nColumns, int withVal, int val)
    {
        double[,] matrix = new double[nColumns, nColumns];
        /*       for (int y = 0; y < nColumns; y++)
               {
                   matrix[y] = new int[nColumns];
               }
               */

        for (int i = 0; i < mRows; i++)
        {
            for (int j = 0; j < nColumns; j++)
            {
                if (withVal == 0)
                {
                    matrix[i, j] = (i * 10) + j;
                }
                else
                {
                    matrix[i, j] = val;
                }
            }
        }
        return matrix;

    }

}





