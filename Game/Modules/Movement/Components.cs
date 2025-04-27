using System.Runtime.Serialization;
using Karpik.StatAndAbilities;

namespace Game.Modules;

[Serializable]
public struct HandleInputMovement : IEcsTagComponent;

[Stat]
public partial struct Speed { }