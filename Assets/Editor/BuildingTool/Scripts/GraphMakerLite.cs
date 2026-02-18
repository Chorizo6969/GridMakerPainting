using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Grid Map", menuName = "GridMapper/Grid Map")]
public class GridMapperLite : ScriptableObject
{
    public int Row = 1;
    public int Column = 1;
    public int[] GridArray = new int[1];  //Il sert à save dans un tableau 1D les changements de _grid || C'est lui qui nous servira dans la save !!!

    [SerializeField] private ToolIcon _icon;
    public ToolIcon Icon => _icon;

    [SerializeField] private TileDatabase _tileDataBase;
    public TileDatabase TileDataBaseRef => _tileDataBase;

    [SerializeField] private List<GameObject> _tilesObjectList = new();
    public List<GameObject> TileObjects => _tilesObjectList;

    public int[,] Grid()
    {
        return Grid(Row, Column, GridArray);
    }

    public static int[,] Grid(int row, int column, int[] array)
    {
        int[,] grid = new int[row, column];

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                grid[i, j] = array[j * row + i];

            }
        }

        return grid;
    }

}
