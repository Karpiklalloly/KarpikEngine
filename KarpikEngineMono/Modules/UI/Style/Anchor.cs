using Microsoft.Xna.Framework;

namespace KarpikEngineMono.Modules;

public struct Anchor
{
    // Значения от 0.0 до 1.0, представляют процент от размера родителя
    public Vector2 Min { get; set; } // К какому проценту родителя привязан левый/верхний край
    public Vector2 Max { get; set; } // К какому проценту родителя привязан правый/нижний край

    // Статические пресеты для удобства
    public static readonly Anchor TopLeft = new Anchor(0f, 0f, 0f, 0f);
    public static readonly Anchor TopCenter = new Anchor(0.5f, 0f, 0.5f, 0f);
    public static readonly Anchor TopRight = new Anchor(1f, 0f, 1f, 0f);
    public static readonly Anchor MiddleLeft = new Anchor(0f, 0.5f, 0f, 0.5f);
    public static readonly Anchor Center = new Anchor(0.5f, 0.5f, 0.5f, 0.5f);
    public static readonly Anchor MiddleRight = new Anchor(1f, 0.5f, 1f, 0.5f);
    public static readonly Anchor BottomLeft = new Anchor(0f, 1f, 0f, 1f);
    public static readonly Anchor BottomCenter = new Anchor(0.5f, 1f, 0.5f, 1f);
    public static readonly Anchor BottomRight = new Anchor(1f, 1f, 1f, 1f);

    // Якоря для растягивания
    public static readonly Anchor StretchHorizontal = new Anchor(0f, 0f, 1f, 0f); // Растянуть по горизонтали (верх)
    public static readonly Anchor StretchVertical = new Anchor(0f, 0f, 0f, 1f);   // Растянуть по вертикали (лево)
    public static readonly Anchor StretchAll = new Anchor(0f, 0f, 1f, 1f);       // Растянуть полностью
    
    public Anchor(float minX, float minY, float maxX, float maxY)
    {
        Min = new Vector2(minX, minY);
        Max = new Vector2(maxX, maxY);
    }

    public Anchor(Vector2 min, Vector2 max)
    {
        Min = min;
        Max = max;
    }
}