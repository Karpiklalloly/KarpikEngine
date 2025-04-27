namespace KarpikEngineMono.Modules.VisualElements;

public static class VisualElementExtensions
{
    public static VisualElement Q<T>(this VisualElement element)
    {
        return element.Children.First(x => x.GetType() == typeof(T));
    }
    
    public static VisualElement Q<T>(this VisualElement element, string name)
    {
        return element.Children.First(x =>
        {
            if (x.Name != name) return false;
            if (x.GetType() != typeof(T)) return false;
            
            return true;
        });
    }
    
    public static IEnumerable<VisualElement> Qs<T>(this VisualElement element)
    {
        return element.Children.Where(x => x.GetType() == typeof(T));
    }
    
    public static IEnumerable<VisualElement> Qs<T>(this VisualElement element, string name)
    {
        return element.Children.Where(x =>
        {
            if (x.Name != name) return false;
            if (x.GetType() != typeof(T)) return false;
            
            return true;
        });
    }
    
    public static VisualElement DeepQ<T>(this VisualElement element)
    {
        return element.AllChildren.First(x => x.GetType() == typeof(T));
    }
    
    public static VisualElement DeepQ<T>(this VisualElement element, string name)
    {
        return element.AllChildren.First(x =>
        {
            if (x.Name != name) return false;
            if (x.GetType() != typeof(T)) return false;
            
            return true;
        });
    }
    
    public static IEnumerable<VisualElement> DeepQs<T>(this VisualElement element)
    {
        return element.AllChildren.Where(x => x.GetType() == typeof(T));
    }
    
    public static IEnumerable<VisualElement> DeepQs<T>(this VisualElement element, string name)
    {
        return element.AllChildren.Where(x =>
        {
            if (x.Name != name) return false;
            if (x.GetType() != typeof(T)) return false;
            
            return true;
        });
    }
}