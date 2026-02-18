using UnityEditor;
using UnityEngine;

public static class GridMakerData //Sert à stocker les méthodes indépendantes et static utiles pour la map
{
    public static Color GetCellColor(int value) //Sert à connaître la color d'une tile de la map
    {
        return value switch
        {
            0 => Color.gray,
            1 => Color.green,
            2 => Color.cyan,
            3 => Color.red,
            4 => Color.yellow,
            5 => Color.white,
            _ => Color.black
        };
    }

    public static bool IsCellInRect(int x, int y, Vector2Int a, Vector2Int b) //Vérifie que le carré est bien dans l'inspector
    {
        int minX = Mathf.Min(a.x, b.x);
        int maxX = Mathf.Max(a.x, b.x);
        int minY = Mathf.Min(a.y, b.y);
        int maxY = Mathf.Max(a.y, b.y);

        return x >= minX && x <= maxX && y >= minY && y <= maxY;
    }

    public static void DrawCellBorder(Rect rect) //Méthode récupérer sur Internet pour faire un contour noir sur des Rect (c'est jolie...)
    {
        Handles.color = new Color(0, 0, 0, 0.25f);
        Handles.DrawAAPolyLine(
            new Vector3(rect.x, rect.y),
            new Vector3(rect.xMax, rect.y),
            new Vector3(rect.xMax, rect.yMax),
            new Vector3(rect.x, rect.yMax),
            new Vector3(rect.x, rect.y)
        );
    }
}
