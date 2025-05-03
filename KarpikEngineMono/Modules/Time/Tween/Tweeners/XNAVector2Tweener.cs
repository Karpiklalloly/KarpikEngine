using GTweens.Delegates;
using GTweens.Interpolators;
using Microsoft.Xna.Framework;

namespace GTweens.Tweeners
{
    public sealed class XNAVector2Tweener : Tweener<Vector2>
    {
        public XNAVector2Tweener(
            Getter currValueGetter, 
            Setter setter, 
            Getter to, 
            float duration, 
            ValidationDelegates.Validation validation
        )
            : base(
                currValueGetter, 
                setter, 
                to, 
                duration,
                XNAVector2Interpolator.Instance, 
                validation
            )
        {
        }
    }
}