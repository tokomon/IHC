using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using AssemblyCSharp;
using UnityEngine.SceneManagement;

namespace Interprete
{

    public class GameManager : MonoBehaviour
{
   // const int fijo = 5;//tamaño de los tableros
   // int x = fijo;
   // int y = fijo;
    int x = AssemblyCSharp.PlayerInfo.Instance.matrix_size;
    int y = AssemblyCSharp.PlayerInfo.Instance.matrix_size;
    int posX = 0, posY = 0;
    List<List<MJson>> obj;
    //lista que lee el JSON
    List<double[,]> matrix = new List<double[,]>();
    int[,] matrixPos;

        double[,] matrixTest; 
     public   String[] data;


    Tablero[] matriz;
    public Cell prefab;

    public Sprite vanilaSprite;//valor no descubierto
    public Sprite mineSprite;//mina
    public Sprite freeSprite;//espacio presionado
    public Sprite paredSprite;//espacio presionado
                              //posiciones actuales
    int posXC = 0;
    int posYC = 0;



    void Start()
    {
            obj = new List<List<MJson>>();
            while (AssemblyCSharp.PlayerInfo.Instance.read_map == false) ;
            data = AssemblyCSharp.PlayerInfo.Instance.maps_data.Split('&');

            int size = AssemblyCSharp.PlayerInfo.Instance.matrix_size;

            matrixTest = new double[size, size];


        double[,] matrixM = inicializeMat(size, size, 0, 0);

        matrixPos = new int[4,2]; 


        //inicializando posiciones de tablero segun jugador
        matrixPos[0,0] = posX - (size + 1);
        matrixPos[0,1] = posY - (size + 1);
        matrixPos[1, 0] = posX - (size + 1);
        matrixPos[1, 1] = posY;
        matrixPos[2, 0] = posX;
        matrixPos[2, 1]= posY;
        matrixPos[3, 0] = posX;
        matrixPos[3, 1] = posY - (size + 1);


        Debug.Log(data.ToString());
            int user = data.Length;
            for (int i = 0; i < user; i++)
            {
                matrix.Add(new double[size,size]);

            }



            matriz = new Tablero[user];
            /*      matrixTest = new double[,] { { 1, 1, 3, 0, 2 }, { 2, 0, 4, 0, 2 },
                                        { 3, 0, 6, 3, 2 }, { 3, 0, 0, 0, 1 },
                                        { 2, 0, 4, 2, 1 } };
            */
            /*
                  matrix.Add(matrixM);


                  for (int i = 2; i < user +1; i++)
                  {
                      //  matrix.Add(new double[size,size]);//matriz que se debe de dibujar
                      matrix.Add(Mirror(matrixM, size, i));
                  }*/

            for (int i = 0; i < user; i++)
            {
                Debug.Log(data[i].ToString());
                obj.Add(JsonConvert.DeserializeObject<List<MJson>>(data[i]));
            }



            Debug.Log(size);
            for (int i = 0; i < size * size; i++)
            {
                int j = 0;
           /*     for (int j = 0; j < user; j++)
                {
             */       matrix[j][obj[j][i].row, obj[j][i].col] = obj[j][i].content;

//                }
            }
            //aqui debo cargar las matrices
            //La primera matriz es la correcta, las demas se debe de cambiar:
            //Probar en clase    
            /*   for (int i = 2; i < user + 1; i++)
            {
                //  matrix.Add(new double[size,size]);//matriz que se debe de dibujar
                matrix.Add(Mirror(matrix[0], size, i));
            }
            */
            for (int i = 0; i<user;i++)
        {
            TableroInstance(i, matrixPos[i,0], matrixPos[i,1], matrix[i]);
                Debug.Log(i);
                Debug.Log(matrix[i][0,0]);
        }

            //todos los tableros empiezan en 0.0 de la esquina izq de abajo
            //se tendria que cambiar para que empiecen en cada esquina de los cuadrados
            //es decir (0,0) (n,n) (n,n) (n,n)
        }
    
    // Update is called once per frame
    void Update()
    {
        if (AssemblyCSharp.PlayerInfo.Instance.read_winner)
        {//Cuando se conecta un ganador eliminar conexión y pasar a escenar de victoria o derrota
            Debug.LogFormat("Winner: {0}", AssemblyCSharp.PlayerInfo.Instance.player_winner);//player_winner si es el ID del ganador
            SceneManager.LoadScene(7);
            AssemblyCSharp.PlayerInfo.Instance.endConnection(); //Borra conexión
            Debug.Log("Disconnected");
        }
        if (AssemblyCSharp.PlayerInfo.Instance.hayJugada())
        {
            Vector3 jugada = AssemblyCSharp.PlayerInfo.Instance.get_jugada();
            Debug.Log(jugada.ToString());
        //    matriz[(int)(jugada.z) - 1].Activate((int)jugada.x, (int)jugada.y);
            matriz[(int)(jugada.z) - 1].Activate((int)jugada.x, (int)jugada.y);


                // Liberar esa jugada donde jugada.x es X, jugada.y es Y y jugada.z es el ID del jugador.
            }
        }
    
    //Lista que se obtiene de leer el json
    public class MJson
    {
        public int row { get; set; }
        public int col { get; set; }
        public int content { get; set; }
    }

    public class Tablero
    {
        public double[,] matrix; //matriz que se debe copiar
        public Cell[,] cellMatrix;

        public Sprite free;
        public Sprite bomb;
        public Sprite wall;
        public Sprite full;

        //para poder usar los sprites definidos arriba
        public void changeSpriteFree(Sprite free_)
        {   free = free_;}
        public void changeSpriteBomb(Sprite bomb_)
        {   bomb = bomb_;}
        public void changeSpriteFull(Sprite full_)
        {   full = full_;}
        public void changeSpriteWall(Sprite wall_)
        {   wall = wall_;}
        /****************************************/
        public Tablero( Sprite mine, Sprite full, Sprite free, Sprite wall)
        {
            changeSpriteFree(free);
            changeSpriteBomb(mine);
            changeSpriteFull(full);
            changeSpriteWall(wall);

        }

        public void init()
        { //dibuja la matriz desde el inicio
            CellMatrixLoop((i, j) =>
            {
                cellMatrix[i, j].Init(new Vector2Int(i, j),
                (matrix[i, j] != 0 ? false : true),
                Activate);
                cellMatrix[i, j].sprite = full;
            });
        }

        //funcion para onclick
        public void Activate(int i, int j)
        {
            if (cellMatrix[i, j].showed)
                return;
            cellMatrix[i, j].showed = true;

            if (cellMatrix[i, j].mine)
            {
                //acaba el juego
                cellMatrix[i, j].sprite = wall;
                Debug.Log("Pared");
                //StartCoroutine(your_timer()); //Delay
            //    init();
                //volver a jugar
            }
            else
            {
                cellMatrix[i, j].sprite = free;
       

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



    void TableroInstance(int p, int posx, int posy, double[,] m)
    {
        matriz[p] = new Tablero( mineSprite, vanilaSprite, freeSprite, paredSprite)
        {
            matrix = m
        };
            if (matriz[p].cellMatrix == null)
        {
            matriz[p].cellMatrix = new Cell[x, y];//crea un objeto matriz con 2 dimensiones
            matriz[p].CellMatrixLoop((i, j) =>
            {
                Cell go = Instantiate(prefab,
                    new Vector3(i + posx, j + posy),
                    Quaternion.identity,
                    transform);
                //hace una copia de un objeto y ponerlo en otro lugar 
                go.name = string.Format("(X:{0},Y:{1})", i, j);

                matriz[p].cellMatrix[i, j] = go;
            });
        }
        matriz[p].init();
     }




    void printMat(double[,] matA, int n)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Debug.Log(matA[i,j]);
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
                    matB[i,j] = matA[i,n - j - 1];
                }
            }
        }
        else if (idPlayer == 4)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matB[i,j] = matA[n - i - 1,j];
                }
            }
        }
        else if (idPlayer == 3)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matB[i,j] = matA[n - i - 1,n - j - 1];
                }
            }
        }
        return matB;
    }


    double[,] inicializeMat( int mRows, int nColumns, int withVal, int val)
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
                    matrix[i,j] = (i * 10) + j;
                }
                else
                {
                    matrix[i,j] = val;
                }
            }
        }
        return matrix;

    }
}
}