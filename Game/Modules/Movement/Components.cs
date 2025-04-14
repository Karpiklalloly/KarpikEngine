namespace Game.Modules;

[Serializable]
public struct HandleInputMovement : IEcsTagComponent
{

}

[Serializable]
public struct Speed : IEcsComponent
{
    public double Value;
}