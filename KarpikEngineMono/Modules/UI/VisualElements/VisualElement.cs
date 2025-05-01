using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KarpikEngineMono.Modules.VisualElements;

public class VisualElement
{
    public VisualElement Parent { get; private set; }
    public List<VisualElement> Children { get; private set; } = [];
    internal IEnumerable<VisualElement> AllChildren => Children.Concat(Children.SelectMany(x => x.AllChildren));
    
    public Rectangle Bounds { get; protected set; }
    public Rectangle OffsetRect { get; set; }
    public Anchor Anchor { get; set; } = Anchor.TopLeft;
    public Vector2 Pivot { get; set; } = new Vector2(0.5f, 0.5f);

    public bool IsVisible
    {
        get => _isVisible && (Parent == null || Parent.IsVisible);
        set => _isVisible = value;
    }

    public bool IsEnabled
    {
        get => _isEnabled && (Parent == null || Parent.IsEnabled);
        set => _isEnabled = value;
    }
    public bool IsHovered { get; private set; }
    public int Order { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    
    private bool _isVisible = true;
    private bool _isEnabled = true;
    private HashSet<string> _tags = new();

    public VisualElement(Rectangle offsetRect)
    {
        OffsetRect = offsetRect;
    }
    
    public void Add(VisualElement child)
    {
        if (child != null && child != this && !Children.Contains(child))
        {
            child.Parent?.Remove(child);
            child.Parent = this;
            Children.Add(child);
            child.UpdateLayout();
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

    public virtual void UpdateLayout()
    {
        if (Parent == null)
        {
            Bounds = OffsetRect;
        }
        else
        {
            var parentBounds = Parent.Bounds;
            var anchorMinPos = new Vector2(parentBounds.X + parentBounds.Width * Anchor.Min.X,
                parentBounds.Y + parentBounds.Height * Anchor.Min.Y);
            var anchorMaxPos = new Vector2(parentBounds.X + parentBounds.Width * Anchor.Max.X,
                parentBounds.Y + parentBounds.Height * Anchor.Max.Y);

            int left = (int)(anchorMinPos.X + OffsetRect.X - OffsetRect.Width / 2f);
            int top = (int)(anchorMinPos.Y + OffsetRect.Y - OffsetRect.Height / 2f);
            int right;
            int bottom;
            
            if (Math.Abs(Anchor.Min.X - Anchor.Max.X) < float.Epsilon)
            {
                right = left + OffsetRect.Width;
            }
            else
            {
                right = (int)(anchorMaxPos.X - OffsetRect.Width);
            }
            
            if (Math.Abs(Anchor.Min.Y - Anchor.Max.Y) < float.Epsilon)
            {
                bottom = top + OffsetRect.Height;
            }
            else
            {
                bottom = (int)(anchorMaxPos.Y - OffsetRect.Height);
            }
            
            Bounds = new Rectangle(left, top, right - left, bottom - top);
        }
    }

    internal void Update(double deltaTime)
    {
        if (!IsVisible) return;
        
        UpdateLayout();

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

        UpdateLayout();
        DrawSelf(spriteBatch);
    }
    
    protected virtual void HandleInput() { }
    
    protected virtual void DrawSelf(SpriteBatch spriteBatch) { }
    
    protected virtual void OnUpdate(double deltaTime) { }
    
    protected void Invalidate()
    {
        
    }
}