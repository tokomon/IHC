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
        int x = AssemblyCSharp.PlayerInfo.Instance.matrix_size;
        int y = AssemblyCSharp.PlayerInfo.Instance.matrix_size;
        int posX = 0, posY = 0;
        List<MJson> obj1;
        List<MJson> obj2;
        List<MJson> obj3;
        List<MJson> obj4;
        //lista que lee el JSON
        //hacer 3d
        List<double[,]> matrix = new List<double[,]>();
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
            for (int i = 0; i < 4; i++)
            {
                matrix.Add(new double[AssemblyCSharp.PlayerInfo.Instance.matrix_size, AssemblyCSharp.PlayerInfo.Instance.matrix_size]);//matriz que se debe de dibujar
            }
            jsonToMatriz();//llenamos la matriz con la data del json
           /* for (int i = 0; i < 4; i++)
            {
                 matriz[i] = new Tablero();
            }*/


            TableroInstance(0,posX, posY, matrix[0]);
            TableroInstance(1,posX - (AssemblyCSharp.PlayerInfo.Instance.matrix_size + 1), posY - (AssemblyCSharp.PlayerInfo.Instance.matrix_size + 1), matrix[1]);
            TableroInstance(2,posX, posY - (AssemblyCSharp.PlayerInfo.Instance.matrix_size + 1), matrix[2]);
            TableroInstance(3,posX - (AssemblyCSharp.PlayerInfo.Instance.matrix_size + 1), posY,matrix[3]);
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
            if (AssemblyCSharp.PlayerInfo.Instance.hayJugada()) {
                Vector3 jugada = AssemblyCSharp.PlayerInfo.Instance.get_jugada();
                Debug.Log(jugada.ToString());
                matriz[(int)(jugada.z)-1].Activate((int)jugada.x, (int)jugada.y);

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
            public int posX, posY;
            public double[,] matrix; //matriz que se debe copiar
            public Cell[,] cellMatrix;

            public Sprite free;
            public Sprite bomb;
            public Sprite wall;
            public Sprite full;

            //para poder usar los sprites definidos arriba
            public void changeSpriteFree(Sprite free_)
            { free = free_; }
            public void changeSpriteBomb(Sprite bomb_)
            { bomb = bomb_; }
            public void changeSpriteFull(Sprite full_)
            { full = full_; }
            public void changeSpriteWall(Sprite wall_)
            { wall = wall_; }
            /****************************************/
            public Tablero(int x, int y, Sprite mine, Sprite full, Sprite free, Sprite wall)
            {
                posX = x;
                posY = y;
                changeSpriteFree(free);
                changeSpriteBomb(mine);
                changeSpriteFull(full);
                changeSpriteWall(wall);
            }

            public void init()
            { //dibuja la matriz desde el inicio
                for (int i = 0; i < cellMatrix.GetLength(0); i++)
                {
                    for (int j = 0; j < cellMatrix.GetLength(1); j++)
                    {
                        cellMatrix[i, j].Init(new Vector2Int(i, j),
                        (matrix[i, j] != (-1) ? false : true),
                        Activate);
                        cellMatrix[i, j].sprite = full;

                    }
                }
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
            int ArroundCount(int i, int j)
            {

                int arround = 0;

                if (PointIsInsideMatrix(i + 1, j) && cellMatrix[i + 1, j].mine)
                    arround++;
                if (PointIsInsideMatrix(i, j + 1) && cellMatrix[i, j + 1].mine)
                    arround++;
                if (PointIsInsideMatrix(i + 1, j + 1) && cellMatrix[i + 1, j + 1].mine)
                    arround++;
                if (PointIsInsideMatrix(i - 1, j) && cellMatrix[i - 1, j].mine)
                    arround++;
                if (PointIsInsideMatrix(i, j - 1) && cellMatrix[i, j - 1].mine)
                    arround++;
                if (PointIsInsideMatrix(i - 1, j - 1) && cellMatrix[i - 1, j - 1].mine)
                    arround++;
                if (PointIsInsideMatrix(i - 1, j + 1) && cellMatrix[i - 1, j + 1].mine)
                    arround++;
                if (PointIsInsideMatrix(i + 1, j - 1) && cellMatrix[i + 1, j - 1].mine)
                    arround++;

                return arround;
            }
        }

        void jsonToMatriz()
        {

            while (AssemblyCSharp.PlayerInfo.Instance.map_data == null) ;
            String[] data = AssemblyCSharp.PlayerInfo.Instance.maps_data.Split('&');
            Debug.Log(data.ToString());

            obj1 = JsonConvert.DeserializeObject<List<MJson>>(data[0]);
            obj2 = JsonConvert.DeserializeObject<List<MJson>>(data[1]);
            obj3 = JsonConvert.DeserializeObject<List<MJson>>(data[2]);
            obj4 = JsonConvert.DeserializeObject<List<MJson>>(data[3]);


            for (int i = 0; i < AssemblyCSharp.PlayerInfo.Instance.matrix_size * AssemblyCSharp.PlayerInfo.Instance.matrix_size; i++)
            {
                matrix[0][obj1[i].row, obj1[i].col] = obj1[i].content;
                matrix[1][obj2[i].row, obj2[i].col] = obj2[i].content;
                matrix[2][obj3[i].row, obj3[i].col] = obj3[i].content;
                matrix[3][obj4[i].row, obj4[i].col] = obj4[i].content;
            }
        }


        void TableroInstance(int p, int posx, int posy, double[,] m)
        {
             matriz[p] = new Tablero(posx, posy, mineSprite, vanilaSprite, freeSprite, paredSprite);

            matriz[p].matrix = m;

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
        
    }
}