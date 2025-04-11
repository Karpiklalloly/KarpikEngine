namespace KarpikEngineMono.Modules.EcsCore
{
    internal class VisualModule : IEcsModule
    {
        public void Import(EcsPipeline.Builder b)
        {
            b.Add(new DrawSpriteSystem());
        }
    }
}
