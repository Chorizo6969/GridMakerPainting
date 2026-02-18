using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMapperLite))]
public class GridMapperLiteEditor : Editor
{
    #region Variables
    private int _row;
    private int _column;
    private int[,] _grid; //Attention Unity ne serialize pas les tableaux 2D (donc au restart il est reset

    private TileDatabase _tileDatabase;
    private GridMapperLite _gridMapper;

    private int _selectedTileId = 1; //Dépendant de _TileDataBase, change selon la tile choisi pour peindre

    private const float CELL_SIZE = 24f; //Over le settings de Unity pour changer la dimension des cases (par défaut 24)

    private bool _isRectDrawing;
    private Vector2Int _rectStart;
    private Vector2Int _rectEnd;

    private enum PaintTool
    {
        Brush,
        Rect,
        Fill
    }

    private PaintTool _currentTool = PaintTool.Brush; //Tool actuel que manie le joueur

    private Transform _parentBuildMap; //Parent des objets tiles
    #endregion

    #region OnEnable
    private void OnEnable() // Se lance à chaque fois que je clic sur l'asset ou si je change une value !
    {
        _gridMapper = (GridMapperLite)target; //Je choppe la ref de qui crée l'éditor (comme je veux GridMapperLite je fais un cast)

        _row = _gridMapper.Row;
        _column = _gridMapper.Column;
        _tileDatabase = _gridMapper.TileDataBaseRef;

        if (_parentBuildMap == null )
            _parentBuildMap = GameObject.FindGameObjectWithTag("MapParent").transform;

        RebuildGrid();
        RebuildVisual();
    }

    #endregion

    #region OnInspectorGUI(Draw)
    public override void OnInspectorGUI() //Globalement je crée des champs dans lequel mettre mes variables et je mets en forme le tout
    {
        serializedObject.Update(); //mets à jour l'affichage de l'inspector
        EditorGUILayout.LabelField("Data Field", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_icon"), new GUIContent("ToolIcon"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_tileDataBase"), new GUIContent("Tile Database"));
        EditorGUILayout.Space(10);

        if (_gridMapper.Icon != null)
        {
            DrawGridSettings();
            DrawToolButtons();
            DrawTileButtons();
            DrawGrid();
        }

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Clear Grid"))
            ClearGrid();
    }
    #endregion

    private void DrawTileButtons() //Affichage des tiles dans l'inspector (carré vert/rouge/bleu, etc...)
    {
        if (_tileDatabase == null || _tileDatabase.TileDefinitionList.Count == 0)
            return;

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal(); //c'est équivalent au div en web

        int maxTiles = Mathf.Min(6, _tileDatabase.TileDefinitionList.Count); //Pour l'instant j'ai mis max 6 Tiles différentes

        for (int i = 0; i < maxTiles; i++)
        {
            TileDefinition tile = _tileDatabase.TileDefinitionList[i];
            Texture icon = tile.Icon != null ? tile.Icon : Texture2D.whiteTexture;

            GUI.backgroundColor = (_selectedTileId == tile.Id) ? Color.cyan : Color.white;

            if (GUILayout.Button(icon, GUILayout.Width(40), GUILayout.Height(40)))
            {
                _selectedTileId = tile.Id;
            }

            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndHorizontal(); //Close le div
    }

    private void DrawGridSettings() //Affichage des sliders Row et Column
    {
        EditorGUILayout.LabelField("Grid Dimensions", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Row", GUILayout.MaxWidth(50));
        _row = EditorGUILayout.IntSlider(_row, 1, 64);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Column", GUILayout.MaxWidth(50));
        _column = EditorGUILayout.IntSlider(_column, 1, 64);
        EditorGUILayout.EndHorizontal();

        if (_grid.GetLength(0) != _row || _grid.GetLength(1) != _column)
            ResizeGrid();

        if (_gridMapper.Row != _row || _gridMapper.Column != _column)
        {
            _gridMapper.Row = _row;
            _gridMapper.Column = _column;
            EditorUtility.SetDirty(_gridMapper); //IMPORTANT  Set Dirty permet de save le Scriptable Object quoiqu'il arrive ! Sans lui les modifs ne sont pas save.
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawToolButtons() //Affichage de mes magnifiques boutons Tool (pinceau, Rect et Fill)
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();

        DrawToolButton(PaintTool.Brush, _gridMapper.Icon.BrushIcon);
        DrawToolButton(PaintTool.Rect, _gridMapper.Icon.RectToolIcon);
        DrawToolButton(PaintTool.Fill, _gridMapper.Icon.FillIcon);

        EditorGUILayout.EndHorizontal();
    }

    private void DrawToolButton(PaintTool tool, Texture icon) //Crée les Bouton Tool
    {
        GUI.backgroundColor = _currentTool == tool ? Color.cyan : Color.white;

        if (GUILayout.Button(icon, GUILayout.Width(75), GUILayout.Height(40)))
            _currentTool = tool;

        GUI.backgroundColor = Color.white;
    }

    private void DrawGrid() // Le vif du sujet avant les calculs relou, affichage de la grille !  Il est call à chaque Frames (je crois)
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical("box");

        for (int i = 0; i < _row; i++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int j = 0; j < _column; j++)
            {
                Rect cellRect = GUILayoutUtility.GetRect(CELL_SIZE, CELL_SIZE, GUILayout.ExpandWidth(false)); //Je crée un Rect avec comme taille le Cell_Size

                if (_currentTool == PaintTool.Rect)
                {
                    HandleRectTool(i, j, cellRect); //Détecte si je clic / peint. En gros si je sélectionne le tool Rect il va essayer de chopper la position de ma souris (il ne dessine rien)
                }

                EditorGUI.DrawRect(cellRect, GridMakerData.GetCellColor(_grid[i, j])); //Il dessine ce que la tile contient (Ex : si son Id est à 0 c'est le vide)

                GridMakerData.DrawCellBorder(cellRect); //Fait un contour noir

                if (_currentTool == PaintTool.Rect && _isRectDrawing) //Preview du RectTool
                {
                    if (GridMakerData.IsCellInRect(i, j, _rectStart, _rectEnd))
                    {
                        EditorGUI.DrawRect(cellRect, new Color(1f, 1f, 1f, 0.25f));
                    }
                }
                HandleTool(i, j, cellRect); //Gestion des autres tools (Brush et fill)
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    //-----------------------------------------------------------Calcul dépendant de la classe------------------------------------------------------------------------------------------
    private void HandleTool(int x, int y, Rect rect)//Comme expliqué plus haut, il gère le comportement des autres Tool
    {
        Event CurrentEvent = Event.current; //Sert à connaître le comportement de ma souris !

        if (!rect.Contains(CurrentEvent.mousePosition)) return;

        if ((CurrentEvent.type == EventType.MouseDown || CurrentEvent.type == EventType.MouseDrag) && CurrentEvent.button == 0)
        {
            switch (_currentTool)
            {
                case PaintTool.Brush: //Si c'est le pinceau 
                    PaintCell(x, y); //Je peins
                    CurrentEvent.Use();
                break;

                case PaintTool.Fill: //Si c'est le pot de peinture 
                    if (CurrentEvent.type == EventType.MouseDown)
                    {
                        int target = _grid[x, y];
                        FloodFill(x, y, target, _selectedTileId); //Je fais un floodFill
                        CurrentEvent.Use();
                    }
                break;
            }
        }
    }

    private void PaintCell(int x, int y) //Utilisé par les pinceau, le rectTool et le floodFill (dans son algo), Il permet de peindre les tiles et donc de changer leur Id !
    {
        int index = Index(x, y);

        _grid[x, y] = _selectedTileId;
        _gridMapper.GridArray[index] = _selectedTileId;
        UpdateSingleTile(x, y);

        EditorUtility.SetDirty(_gridMapper); //Je save
    }

    private void ResizeGrid() //Update la taille de la grille
    {
        int oldRow = _gridMapper.Row;
        int oldColumn = _gridMapper.Column;

        int[,] oldGrid = _grid;
        List<GameObject> oldTiles = new List<GameObject>(_gridMapper.TileObjects);

        _grid = new int[_row, _column];
        _gridMapper.GridArray = new int[_row * _column];

        _gridMapper.TileObjects.Clear(); // resize TileObjects
        for (int i = 0; i < _row * _column; i++)
            _gridMapper.TileObjects.Add(null);

        for (int x = 0; x < Mathf.Min(oldRow, _row); x++)
        {
            for (int y = 0; y < Mathf.Min(oldColumn, _column); y++)
            {
                int oldIndex = y * oldRow + x;
                int newIndex = Index(x, y);

                int value = oldGrid[x, y];

                _grid[x, y] = value;
                _gridMapper.GridArray[newIndex] = value;

                if (oldIndex < oldTiles.Count)
                {
                    _gridMapper.TileObjects[newIndex] = oldTiles[oldIndex];
                }
            }
        }

        for (int i = 0; i < oldTiles.Count; i++)  // destroy les tiles hors limite
        {
            int x = i % oldRow;
            int y = i / oldRow;

            if (x >= _row || y >= _column)
            {
                if (oldTiles[i] != null)
                    DestroyImmediate(oldTiles[i]);
            }
        }

        EditorUtility.SetDirty(_gridMapper);
    }


    private void RebuildGrid() // Recrée la grille lors du OnEnable      Pour rappel le GridArray est le seul à être save ! C'est lui qui se rappel de la map !
    {
        _grid = new int[_row, _column];

        for (int i = 0; i < _gridMapper.GridArray.Length; i++)
        {
            int x = i % _row;
            int y = i / _row;

            _grid[x, y] = _gridMapper.GridArray[i];
        }
    }

    private void HandleRectTool(int x, int y, Rect rect) //Détecte si je clic / peint, Attention il ne dessine rien !
    {
        Event CurrentEvent = Event.current;

        if (!rect.Contains(CurrentEvent.mousePosition))
            return;

        if (CurrentEvent.type == EventType.MouseDown && CurrentEvent.button == 0)
        {
            _isRectDrawing = true;
            _rectStart = new Vector2Int(x, y);
            _rectEnd = _rectStart;
            CurrentEvent.Use();
        }

        if (_isRectDrawing && CurrentEvent.type == EventType.MouseDrag)
        {
            _rectEnd = new Vector2Int(x, y);
            Repaint();
            CurrentEvent.Use();
        }

        if (_isRectDrawing && CurrentEvent.type == EventType.MouseUp)
        {
            _rectEnd = new Vector2Int(x, y);
            PaintRect(_rectStart, _rectEnd);
            _isRectDrawing = false;
            CurrentEvent.Use();
        }
    }

    private void PaintRect(Vector2Int a, Vector2Int b) //Dessine un carré (Pour le RectTool)
    {
        int minX = Mathf.Min(a.x, b.x);
        int maxX = Mathf.Max(a.x, b.x);
        int minY = Mathf.Min(a.y, b.y);
        int maxY = Mathf.Max(a.y, b.y);

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                PaintCell(x, y); //On change la couleur !
            }
        }
    }

    private void ClearGrid() //Reset la grille avec que du vide (Replace tout les id des tiles par 0)
    {
        for (int x = 0; x < _row; x++)
        {
            for (int y = 0; y < _column; y++)
            {
                int index = Index(x, y);

                _grid[x, y] = 0;
                _gridMapper.GridArray[Index(x, y)] = 0;
                UpdateSingleTile(x, y);
            }
        }

        EditorUtility.SetDirty(_gridMapper); //Je save
    }

    private void FloodFill(int startX, int startY, int targetValue, int replacementValue) //FloodFill (BFS)
    {
        if (targetValue == replacementValue) return;

        Queue<Vector2Int> nodes = new();
        nodes.Enqueue(new Vector2Int(startX, startY));

        while (nodes.Count > 0)
        {
            Vector2Int node = nodes.Dequeue();

            int x = node.x;
            int y = node.y;

            if (x < 0 || y < 0 || x >= _row || y >= _column)
                continue;

            if (_grid[x, y] != targetValue)
                continue;

            _grid[x, y] = replacementValue;

            int index = Index(x, y);
            _gridMapper.GridArray[index] = replacementValue;
            UpdateSingleTile(x, y);

            nodes.Enqueue(new Vector2Int(x + 1, y));
            nodes.Enqueue(new Vector2Int(x - 1, y));
            nodes.Enqueue(new Vector2Int(x, y + 1));
            nodes.Enqueue(new Vector2Int(x, y - 1));
        }

        EditorUtility.SetDirty(_gridMapper);
    }

    private void UpdateSingleTile(int x, int y)
    {
        int index = Index(x, y);

        if (_gridMapper.TileObjects[index] != null)
            DestroyImmediate(_gridMapper.TileObjects[index]);

        int tileId = _gridMapper.GridArray[index];
        TileDefinition tileDef = _tileDatabase.TileDefinitionList[tileId];

        if (tileDef.Prefab != null)
        {
            GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(tileDef.Prefab,_parentBuildMap);

            tile.transform.position = new Vector3(x, tileDef.Height, y);
            _gridMapper.TileObjects[index] = tile;
        }
        else
            _gridMapper.TileObjects[index] = null;
    }

    private void RebuildVisual()
    {
        int size = _row * _column;

        while (_gridMapper.TileObjects.Count < size)
            _gridMapper.TileObjects.Add(null);

        for (int x = 0; x < _row; x++)
            for (int y = 0; y < _column; y++)
                UpdateSingleTile(x, y);
    }

    private int Index(int x, int y) => y * _row + x; //Formule générique
}