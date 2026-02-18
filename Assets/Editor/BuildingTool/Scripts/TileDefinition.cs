using UnityEngine;

[System.Serializable]
public class TileDefinition
{
    [SerializeField] private int _id;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Texture _icon;
    [SerializeField] private float _height = 0f;

    public int Id => _id;
    public GameObject Prefab => _prefab;
    public Texture Icon => _icon;
    public float Height => _height;
}
