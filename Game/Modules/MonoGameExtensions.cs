using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Game.Modules;

public static class MonoGameExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalized(this Vector2 vector)
    {
        return vector == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(vector);
    }
}