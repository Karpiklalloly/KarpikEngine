using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.VisualElements;

public class VisualElement
{
    public VisualElement Parent { get; private set; }
    public List<VisualElement> Children { get; private set; } = [];
    internal IEnumerable<VisualElement> AllChildren => Children.Concat(Children.SelectMany(x => x.AllChildren));
    
    public Rectangle Bounds { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    public bool IsHovered { get; private set; }
    public int Order { get; set; } = 0;
    public string Name { get; set; } = string.Empty;

    internal bool NeedRedraw => _needRedraw || Children.Any(x => x.NeedRedraw);

    public Vector2 UIPosition =>
        Parent != null ? Parent.UIPosition + Bounds.Location.ToVector2() : Bounds.Location.ToVector2();
    public Rectangle UIBounds => new(UIPosition.ToPoint(), Bounds.Size);
    
    private bool _needRedraw = true;
    private HashSet<string> _tags = new();

    public VisualElement(Rectangle bounds)
    {
        Bounds = bounds;
    }
    
    public void Add(VisualElement child)
    {
        if (child != null && child != this && !Children.Contains(child))
        {
            child.Parent?.Remove(child);
            child.Parent = this;
            Children.Add(child);
        }
    }

    public void Remove(VisualElement child)
    {
        if (child != null && Children.Contains(child))
        {
            if (child.Parent == this)
            {
                child.Parent = null;
            }

            Children.Remove(child);
        }
    }

    public void AddTag(string tag)
    {
        _tags.Add(tag);
    }
    
    public void RemoveTag(string tag)
    {
        _tags.Remove(tag);
    }
    
    public bool HasTag(string tag)
    {
        return _tags.Contains(tag);
    }

    internal void Update(double deltaTime)
    {
        if (!IsVisible) return;

        var mousePosition = Input.MousePosition;
        IsHovered = Bounds.Contains(mousePosition);

        for (int i = Children.Count - 1; i >= 0; i--)
        {
            Children[i].Update(deltaTime);
        }
        
        HandleInput();
        OnUpdate(deltaTime);
    }

    internal void Draw(double deltaTime, SpriteBatch spriteBatch)
    {
        if (!IsVisible) return;
        //TODO: Добавить редрав только если нужно
        //if (!_needRedraw) return;
        
        DrawSelf(spriteBatch);

        _needRedraw = false;
    }
    
    protected virtual void HandleInput() { }
    
    protected virtual void DrawSelf(SpriteBatch spriteBatch) { }
    
    protected virtual void OnUpdate(double deltaTime) { }
    
    protected void Invalidate()
    {
        _needRedraw = true;
    }
}