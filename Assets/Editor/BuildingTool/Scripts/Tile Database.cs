using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "GridMapper/TileDatabase")]
public class TileDatabase : ScriptableObject //Id, prefab, Icon, color, rules
{
    [SerializeField] private List<TileDefinition> _tileDefinitionList = new();

    public List<TileDefinition> TileDefinitionList { get { return _tileDefinitionList; } }
}
