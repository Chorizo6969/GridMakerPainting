using UnityEngine;

[CreateAssetMenu(fileName = "ToolIcon", menuName = "GridMapper/ToolIcon")]
public class ToolIcon : ScriptableObject
{
    [SerializeField] private Texture _brushIcon;
    [SerializeField] private Texture _rectToolIcon;
    [SerializeField] private Texture _fillIcon;

    public Texture BrushIcon => _brushIcon;
    public Texture RectToolIcon => _rectToolIcon;
    public Texture FillIcon => _fillIcon;

}
