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
        List<MJson> obj;
        //lista que lee el JSON
        //hacer 3d
        double[,] matrix = new double[AssemblyCSharp.PlayerInfo.Instance.matrix_size, AssemblyCSharp.PlayerInfo.Instance.matrix_size];//matriz que se debe de dibujar
        public Cell prefab;

        public Sprite vanilaSprite;//valor no descubierto
        public Sprite mineSprite;//mina
        public Sprite freeSprite;//espacio presionado


        void Start()
        {
            TableroInstance(posX, posY);
            TableroInstance(posX - (AssemblyCSharp.PlayerInfo.Instance.matrix_size + 1), posY - (AssemblyCSharp.PlayerInfo.Instance.matrix_size + 1));
            TableroInstance(posX, posY - (AssemblyCSharp.PlayerInfo.Instance.matrix_size + 1));
            TableroInstance(posX - (AssemblyCSharp.PlayerInfo.Instance.matrix_size + 1), posY);
            //todos los tableros empiezan en 0.0 de la esquina izq de abajo
            //se tendria que cambiar para que empiecen en cada esquina de los cuadrados
            //es decir (0,0) (n,n) (n,n) (n,n)
        }

        // Update is called once per frame
        void Update()
		{
			if (Input.GetKeyDown (KeyCode.Space))//LLamar cuando selibera una posición
				AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace (1, 1);//Pruebas Ignorar
			else if (Input.GetKeyDown (KeyCode.W))
				AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace (0, 0);//Pruebas Ignorar
			else if (Input.GetKeyDown (KeyCode.L))
				AssemblyCSharp.PlayerInfo.Instance.sendFreeSpace (2, 2);//Pruebas Ignorar
			else if ( AssemblyCSharp.PlayerInfo.Instance.read_winner) {//Cuando se conecta un ganador eliminar conexión y pasar a escenar de victoria o derrota
				Debug.LogFormat ("Winner: {0}", AssemblyCSharp.PlayerInfo.Instance.player_winner);//player_winner si es el ID del ganador
                if(AssemblyCSharp.PlayerInfo.Instance.player_id == AssemblyCSharp.PlayerInfo.Instance.player_winner){
                    SceneManager.LoadScene(4);
                }
                else{
                    SceneManager.LoadScene(5);
                }
				AssemblyCSharp.PlayerInfo.Instance.endConnection (); //Borra conexión
				Debug.Log ("Disconnected");
			} else if (AssemblyCSharp.PlayerInfo.Instance.alertar_forzado) { // Cuando se te desconecta por X motivos
				Debug.Log (AssemblyCSharp.PlayerInfo.Instance.message.ToString ()); //Mensaje a mostrar
				AssemblyCSharp.PlayerInfo.Instance.endConnection (); //Eliminar conexión
				Debug.Log ("Disconnected");
			} else if (AssemblyCSharp.PlayerInfo.Instance.actualizar_map) {//Cuando se te da permiso de liberar
				Debug.LogFormat ("Liberar X: {0}", AssemblyCSharp.PlayerInfo.Instance.liberar_x);
				Debug.LogFormat ("Liberar Y: {0}", AssemblyCSharp.PlayerInfo.Instance.liberar_y);

				AssemblyCSharp.PlayerInfo.Instance.actualizar_map = false;
			} else if (AssemblyCSharp.PlayerInfo.Instance.alertar_jugador) {//Cuando NO se te da permiso de liberar
				Debug.Log ("MAL PE");
				AssemblyCSharp.PlayerInfo.Instance.alertar_jugador = false;
			}
			AssemblyCSharp.PlayerInfo.Instance.sendPosition ();//Siempre enviar posición del usuario
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
                    //si hay un campo libre de debe de hacer recursion para mostrar
                    /*
                    if (ArroundCount(i, j) == 0)
                    {
                        ActivateArround(i,j);
                    }
                    else
                    {
                        cellMatrix[i, j].text = ArroundCount(i, j).ToString();
                    }*/
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
        {/*
            string json = @"[{'row': 0, 'col': 0, 'content': 1}, {'row': 0, 'col': 1, 'content': 1}, 
    	{'row': 0, 'col': 2, 'content': 3}, {'row': 0, 'col': 3, 'content': -1}, 
    	{'row': 0, 'col': 4, 'content': 2}, {'row': 1, 'col': 0, 'content': 2}, 
    	{'row': 1, 'col': 1, 'content': -1}, {'row': 1, 'col': 2, 'content': 4}, 
    	{'row': 1, 'col': 3, 'content': -1}, {'row': 1, 'col': 4, 'content': 2}, 
    	{'row': 2, 'col': 0, 'content': 3}, {'row': 2, 'col': 1, 'content': -1}, 
    	{'row': 2, 'col': 2, 'content': 6}, {'row': 2, 'col': 3, 'content': 3}, 
    	{'row': 2, 'col': 4, 'content': 2}, {'row': 3, 'col': 0, 'content': 3}, 
    	{'row': 3, 'col': 1, 'content': -1}, {'row': 3, 'col': 2, 'content': -1}, 
    	{'row': 3, 'col': 3, 'content': -1}, {'row': 3, 'col': 4, 'content': 1}, 
    	{'row': 4, 'col': 0, 'content': 2}, {'row': 4, 'col': 1, 'content': -1}, 
    	{'row': 4, 'col': 2, 'content': 4}, {'row': 4, 'col': 3, 'content': 2}, 
    	{'row': 4, 'col': 4, 'content': 1}]";*/

            while (AssemblyCSharp.PlayerInfo.Instance.map_data == null);
            String data = AssemblyCSharp.PlayerInfo.Instance.map_data;
            Debug.Log(data.ToString());
            
            obj = JsonConvert.DeserializeObject<List<MJson>>(data);

            for (int i = 0; i < AssemblyCSharp.PlayerInfo.Instance.matrix_size  * AssemblyCSharp.PlayerInfo.Instance.matrix_size; i++)
            {
                matrix[obj[i].row, obj[i].col] = obj[i].content;
            }
        }


        void TableroInstance(int posx, int posy)
        {
            Tablero matriz = new Tablero(posx, posy, mineSprite, vanilaSprite, freeSprite);
            jsonToMatriz();//llenamos la matriz con la data del json
            matriz.matrix = matrix;

            if (matriz.cellMatrix == null)
            {
                matriz.cellMatrix = new Cell[x, y];//crea un objeto matriz con 2 dimensiones
                matriz.CellMatrixLoop((i, j) =>
                {
                    Cell go = Instantiate(prefab,
                        new Vector3(i + posx, j + posy),
                        Quaternion.identity,
                        transform);
                //hace una copia de un objeto y ponerlo en otro lugar 
                go.name = string.Format("(X:{0},Y:{1})", i, j);

                    matriz.cellMatrix[i, j] = go;
                });
            }
            matriz.init();

        }

        System.Collections.IEnumerator your_timer()
        {
            Debug.Log("Your enter Coroutine at" + Time.time);
            yield return new WaitForSeconds(10000000000000000.0f);
            //funcion de delay para que se muestre la mina al acabar el juego
        }
    }
}