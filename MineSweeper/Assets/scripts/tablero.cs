using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class tablero : MonoBehaviour
{
    public int x = AssemblyCSharp.PlayerInfo.Instance.matrix_size; // number of columns for the grid
    public int y = AssemblyCSharp.PlayerInfo.Instance.matrix_size; // number of rows for the grid
    public float SeperationValueX = 0.001f; // Distance between each column
    public float SeperationValueZ = 0.001f; // Distance between each Row

    public float tempSepX = 0; // used to calculate the separation between each column
    public float tempSepZ = 0;// used to calculate the separation between each row
    double[,] matrix = new double[AssemblyCSharp.PlayerInfo.Instance.matrix_size, AssemblyCSharp.PlayerInfo.Instance.matrix_size];//matriz que se debe de dibujar
    public Texture3D texture;
    public Material paredMat;
    public Material pisoMat;
    public Cell prefab;

    public static Material matPiso;
    public static Material matPared;

    List<MJson> obj;
	public GameObject gmo = null;

    public Sprite vanilaSprite;//valor no descubierto
    public Sprite mineSprite;//mina
    public Sprite freeSprite;//espacio presionado

    // Use this for initialization
    void Start()
    {
        //  Debug.Log(3);
        matPiso = paredMat;
        matPared = pisoMat;
        createGrid();//call the createGrid function on start
    }

    // Update is called once per frame
    void Update()
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

    public class MJson
    {
        public int row { get; set; }
        public int col { get; set; }
        public int content { get; set; }
    }

    void jsonToMatriz()
    {
        /*string json = @"[{'row': 0, 'col': 0, 'content': 1}, {'row': 0, 'col': 1, 'content': 1}, 
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

        while (AssemblyCSharp.PlayerInfo.Instance.map_data == null) ;
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

    void createGrid()
    {
        jsonToMatriz();

        for (float i = 0; i < x; i += 1)
        {//loop 1 to loop through columns
            for (float j = 0; j < y; j += 1)
            {//loop 2 to loop through rows
             //   Debug.Log(j);
				if (j == 0 && i == 0)
					continue;
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Cube); //create a quad primitive as provided by unity
                plane.transform.position = new Vector3(i * 1.05f, 0, 1.05f * j); //position the newly created quad accordingly
                plane.transform.eulerAngles = new Vector3(90f, 0, 0); //rotate the quads 90 degrees in X to face up
          //      plane.name = i.ToString() + " " + j.ToString();

          /*     texture = (Texture3D)Resources.Load("Assets/textures/piso");
               newMat = Resources.Load("Assets/Materials/piso", typeof(Material)) as Material;
            */    

                //   Debug.Log(newMat);
				plane.name = ((int)i).ToString() + " " + ((int)j).ToString()+ " " + ((int)matrix[(int)i,(int)j]).ToString();
                    //plane.renderer.material = newMat;

               //     plane.GetComponent<Renderer>().material.mainTexture = texture;
               //     plane.GetComponentInChildren<Renderer>().material = newMat;
                        // plane.GetComponent<Renderer>().material = newMat;
                 /*   MeshRenderer rend = GetComponent<MeshRenderer>();
                    rend.material = newMat;
                    */
            //        Debug.Log(10)

                    //     plane.GetComponent<Renderer>().enabled = false;
              //  plane.AddComponent<MaterialChange>();

                plane.AddComponent<Hide>();
                //      rend.material = newMat;
                //      tempSepZ += SeperationValueZ;//change the value of seperation between rows
                //   Debug.Log(tempSepX);
                /*    Texture3D texture = CreateTexture3D(256);
                    Debug.Log(texture);
                    plane.GetComponent<Renderer>().material.mainTexture = texture;
                    */
                /*
                Texture3D texture = (Texture3D)Resources.Load("Assets/textures/piso");
                Debug.Log(texture);

                plane.GetComponent<Renderer>().material.mainTexture = texture;
                Instantiate(plane);
                */


                /*                Debug.Log(tempSepZ);
                                Debug.Log(i);
                                Debug.Log(j);
                  */
            }
            //    tempSepX += SeperationValueX;//change the value of seperation between columns
            tempSepZ = 0;//Reset the value of the seperation between the rows so it won‘t cumulate
        }
    }


    Texture3D CreateTexture3D(int size)
    {
        Color[] colorArray = new Color[size * size * size];
        Texture3D texture = new Texture3D(size, size, size, TextureFormat.RGBA32, true);
        float r = 1.0f / (size - 1.0f);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Color c = new Color(x * r, y * r, z * r, 1.0f);
                    colorArray[x + (y * size) + (z * size * size)] = c;
                }
            }
        }
        texture.SetPixels(colorArray);
        texture.Apply();
        return texture;
    }

}


    
   

