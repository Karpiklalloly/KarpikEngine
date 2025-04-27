namespace KarpikEngineMono.Modules.EcsCore
{
    public class VisualModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new DrawSpriteSystem());
        }
    }
}
