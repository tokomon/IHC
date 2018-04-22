using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;




public class GameManager : MonoBehaviour
{
    Cell[,] cellMatrix;
    bool ingame;
    const int fijo = 5;
    int posX = 0, posY = 0;
    List<Mrx> obj;
    //hacer 3d
    double[,] matrix = new double[fijo, fijo];

    public Vector2Int dimension;
    public Cell prefab;

    [Range(0, 100)]
    public int minePercent = 10;

    public Sprite vanilaSprite;//valor no descubierto
    public Sprite mineSprite;
    public Sprite freeSprite;
    /*
        dimension.x = 10;
        dimension.y = 10;
    */
    //  vanilaSprite.transform.localScale.x =1;

    void Start()
    {
        MatrizInstance(posX, posY);
        //       MatrizInstance(posX-fijo,posY-fijo);
    }


    public class Mrx
    {
        public int row { get; set; }
        public int col { get; set; }
        public int content { get; set; }
    }

    void MatrizFull()
    {
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
    	{'row': 4, 'col': 4, 'content': 1}]";

        
        obj = JsonConvert.DeserializeObject<List<Mrx>>(json);

        
        Debug.Log(obj);
        //matriz que nos daran llena
        
        for (int i = 0; i < fijo * fijo; i++)
        {
            Console.Write("asfsaf");
          
            matrix[obj[i].row, obj[i].col] = obj[i].content;
            Debug.Log(0.0001);

            Debug.Log(obj[i].col);
            Debug.Log(obj[i].row);
            Debug.Log(matrix[obj[i].row, obj[i].col]);
            Debug.Log(0.1);


        }
    }


    void MatrizInstance(int posx, int posy)
    { int x = dimension.x;
      int y = dimension.y;
      x = fijo;
      y = fijo;
    	MatrizFull();
        if (cellMatrix == null)
        {
          //  MatrizFull();
            cellMatrix = new Cell[x, y];//crea un objeto matriz con 2 dimensiones
            CellMatrixLoop((i, j) =>
            {
                Cell go = Instantiate(prefab,
                //    new Vector3(i - x -posx , j - y - posy ),
                new Vector3(i , j ),
                    Quaternion.identity,
                    transform);//hacer una copia de un objeto y ponerlo en otro lugar 
                go.name = string.Format("(X:{0},Y:{1})", i, j);

                
                cellMatrix[i, j] = go;
            });
        }

        CellMatrixLoop((i, j) =>//caracter lamba
        {
            
            cellMatrix[i, j].Init(new Vector2Int(i, j),
            ( matrix[i,j]!=(-1) ? false : true),
            Activate);//el compilador entiende que los dos parametros son iguales
               /* if(cellMatrix[i,j].mine)
                { 
                Debug.Log(i);
                Debug.Log(j);
                Debug.Log(matrix[i,j]);
                }*/

            cellMatrix[i, j].sprite = vanilaSprite;
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
            // FAIL STATE
            //acaba el juego
            cellMatrix[i, j].sprite = mineSprite;
            MatrizInstance(posX, posY);
            //cuando se acaba el juego
        }
        else
        {
            cellMatrix[i, j].sprite = freeSprite;
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
            cellMatrix[i, j].text = (obj[i * (fijo) + j].content).ToString();
        
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
    void CellMatrixLoop(Action<int, int> e)//recibir en un parametro una funcion
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