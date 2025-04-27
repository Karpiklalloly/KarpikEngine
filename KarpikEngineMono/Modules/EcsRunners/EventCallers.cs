using System;
using DCFApixels.DragonECS;
using DCFApixels.DragonECS.RunnersCore;
using KarpikEngineMono;
using KarpikEngineMono.Modules.EcsCore;
using KarpikEngineMono.Modules.EcsRunners;

namespace Karpik.DragonECS
{
    public static class Events
    {
        public static EcsPipeline.Builder AddCaller<T>(this EcsPipeline.Builder b, string layerName = EcsConsts.BEGIN_LAYER)
            where T : struct, IEcsComponentEvent
        {
            b.AddUnique(new EventCallerSystem<T>(b), layerName);
            return b;
        }
        
        public static EcsPipeline.Builder AddFixedCaller<T>(this EcsPipeline.Builder b, string layerName = EcsConsts.BEGIN_LAYER)
            where T : struct, IEcsComponentEvent
        {
            b.AddUnique(new EventFixedCallerSystem<T>(b), layerName);
            return b;
        }
        
        private class EventCallerSystem<T> : IEcsRun, IEcsPipelineMember where T : struct, IEcsComponentEvent
        {
            private EcsEventWorld _eventWorld;
            public EcsPipeline Pipeline { get; set; }

            public EventCallerSystem()
            {
                _eventWorld = Worlds.Instance.EventWorld;
            }

            public EventCallerSystem(EcsPipeline.Builder b)
            {
                b.AddRunner<OnEventRunner<T>>();
            }

            public void Run()
            {
                Pipeline.GetRunner<OnEventRunner<T>>().Run();
            }
        }
        
        private class EventFixedCallerSystem<T> : IEcsFixedRun, IEcsPipelineMember where T : struct, IEcsComponentEvent
        {
            private EcsEventWorld _eventWorld;
            public EcsPipeline Pipeline { get; set; }

            public EventFixedCallerSystem()
            {
                _eventWorld = Worlds.Instance.EventWorld;
            }

            public EventFixedCallerSystem(EcsPipeline.Builder b)
            {
                b.AddRunner<OnEventFixedRunner<T>>();
            }

            public void FixedRun()
            {
                Pipeline.GetRunner<OnEventFixedRunner<T>>().Run();
            }
        }
        
        public class OnEventRunner<T> : EcsRunner<IEcsRunOnEvent<T>>, IEcsRunOnEvent<T> where T : struct, IEcsComponentEvent
        {
            private class Aspect : EcsAspect
            {
                public EcsPool<T> evt = Inc;
            }

            private EcsEventWorld _eventWorld;

            public OnEventRunner()
            {
                _eventWorld = Worlds.Instance.EventWorld;
            }

            public void Run()
            {
                var span = _eventWorld.Where(out Aspect a);
                if (span.Count == 0)
                {
                    return;
                }

                foreach (var e in span)
                {
                    foreach (var run in Process)
                    {
                        run.RunOnEvent(ref a.evt.Get(e));
                    }
                }
                
                a.evt.ClearAll();
            }

            public void RunOnEvent(ref T evt)
            {
            }
        }
        
        public class OnEventFixedRunner<T> : EcsRunner<IEcsFixedRunOnEvent<T>>, IEcsFixedRunOnEvent<T> where T : struct, IEcsComponentEvent
        {
            private class Aspect : EcsAspect
            {
                public EcsPool<T> evt = Inc;
            }

            private EcsEventWorld _eventWorld;

            public OnEventFixedRunner()
            {
                _eventWorld = Worlds.Instance.EventWorld;
            }

            public void Run()
            {
                var span = _eventWorld.Where(out Aspect a);
                if (span.Count == 0)
                {
                    return;
                }

                var pool = _eventWorld.GetPool<T>();
                var count = pool.Count;
                if (typeof(T) == typeof(CollisionsEvent) && (span.Count >= 3 || count >= 3))
                {
                    if (span.Count == count)
                    {
                        
                    }
                }

                foreach (var e in span)
                {
                    foreach (var run in Process)
                    {
                        run.RunOnEvent(ref a.evt.Get(e));
                    }
                }
                
                a.evt.ClearAll();
            }

            public void RunOnEvent(ref T evt)
            {
            }
        }
    }
    
    public static class Requests
    {
        public static EcsPipeline.Builder AddCaller<T>(this EcsPipeline.Builder b, string layerName = EcsConsts.BEGIN_LAYER)
            where T : struct, IEcsComponentRequest
        {
            b.AddUnique(new EventCallerSystem<T>(b), layerName);
            return b;
        }
        
        public static EcsPipeline.Builder AddFixedCaller<T>(this EcsPipeline.Builder b, string layerName = EcsConsts.BEGIN_LAYER)
            where T : struct, IEcsComponentRequest
        {
            b.AddUnique(new EventFixedCallerSystem<T>(b), layerName);
            return b;
        }
        
        private class EventCallerSystem<T> : IEcsRun, IEcsPipelineMember where T : struct, IEcsComponentRequest
        {
            private EcsEventWorld _eventWorld;
            public EcsPipeline Pipeline { get; set; }

            public EventCallerSystem()
            {
                _eventWorld = Worlds.Instance.EventWorld;
            }

            public EventCallerSystem(EcsPipeline.Builder b)
            {
                b.AddRunner<OnRequestRunner<T>>();
            }

            public void Run()
            {
                Pipeline.GetRunner<OnRequestRunner<T>>().Run();
            }
        }
        
        private class EventFixedCallerSystem<T> : IEcsFixedRun, IEcsPipelineMember where T : struct, IEcsComponentRequest
        {
            public EcsPipeline Pipeline { get; set; }

            public EventFixedCallerSystem(EcsPipeline.Builder b)
            {
                b.AddRunner<OnRequestFixedRunner<T>>();
            }

            public void FixedRun()
            {
                Pipeline.GetRunner<OnRequestFixedRunner<T>>().Run();
            }
        }
        
        public class OnRequestRunner<T> : EcsRunner<IEcsRunOnRequest<T>>, IEcsRunOnRequest<T> where T : struct, IEcsComponentRequest
        {
            private class Aspect : EcsAspect
            {
                public EcsPool<T> evt = Inc;
            }

            private EcsDefaultWorld _eventWorld;

            public OnRequestRunner()
            {
                _eventWorld = Worlds.Instance.World;
            }

            public void Run()
            {
                var span = _eventWorld.Where(out Aspect a);
                if (span.Count == 0)
                {
                    return;
                }

                foreach (var e in span)
                {
                    foreach (var run in Process)
                    {
                        run.RunOnEvent(ref a.evt.Get(e));
                    }
                }
                
                a.evt.ClearAll();
            }

            public void RunOnEvent(ref T evt)
            {
            }
        }
        
        public class OnRequestFixedRunner<T> : EcsRunner<IEcsFixedRunOnRequest<T>>, IEcsFixedRunOnRequest<T> where T : struct, IEcsComponentRequest
        {
            private class Aspect : EcsAspect
            {
                public EcsPool<T> evt = Inc;
            }

            private EcsDefaultWorld _eventWorld;

            public OnRequestFixedRunner()
            {
                _eventWorld = Worlds.Instance.World;
            }

            public void Run()
            {
                var span = _eventWorld.Where(out Aspect a);
                if (span.Count == 0)
                {
                    return;
                }

                foreach (var e in span)
                {
                    foreach (var run in Process)
                    {
                        run.RunOnEvent(ref a.evt.Get(e));
                    }
                }
                
                a.evt.ClearAll();
            }

            public void RunOnEvent(ref T evt)
            {
            }
        }
    }

    public abstract class RunOnEventSystem<TEvent, TAspect> : IEcsRunOnEvent<TEvent>
        where TEvent : struct, IEcsComponentEvent
        where TAspect : EcsAspect, new()
    {
        private EcsDefaultWorld _world = Worlds.Instance.World;
        
        public void RunOnEvent(ref TEvent evt)
        {
            try
            {
                var aspect = _world.GetAspect<TAspect>();
                if (aspect.IsMatches(evt.Target.ID))
                {
                    RunOnEvent(ref evt, ref aspect);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        
        protected abstract void RunOnEvent(ref TEvent evt, ref TAspect aspect);
    }
    
    public abstract class RunOnRequestSystem<TRequest, TAspect> : IEcsRunOnRequest<TRequest>
        where TRequest : struct, IEcsComponentRequest
        where TAspect : EcsAspect, new()
    {
        private EcsWorld _world = Worlds.Instance.World;
        
        public void RunOnEvent(ref TRequest evt)
        {
            try
            {
                var aspect = _world.GetAspect<TAspect>();
                if (aspect.IsMatches(evt.Target.ID))
                {
                    RunOnEvent(ref evt, ref aspect);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        
        protected abstract void RunOnEvent(ref TRequest evt, ref TAspect aspect);
    }
    
    public abstract class FixedRunOnEventSystem<TEvent, TAspect> : IEcsFixedRunOnEvent<TEvent>
        where TEvent : struct, IEcsComponentEvent
        where TAspect : EcsAspect, new()
    {
        private EcsEventWorld _world = Worlds.Instance.EventWorld;
        
        public void RunOnEvent(ref TEvent evt)
        {
            var aspect = _world.GetAspect<TAspect>();
            if (aspect.IsMatches(evt.Target.ID))
            {
                FixedRunOnEvent(ref evt, ref aspect);
            }
        }
        
        public abstract void FixedRunOnEvent(ref TEvent evt, ref TAspect aspect);
    }
    
    public abstract class FixedRunOnRequestSystem<TRequest, TAspect> : IEcsFixedRunOnRequest<TRequest>
        where TRequest : struct, IEcsComponentRequest
        where TAspect : EcsAspect, new()
    {
        private EcsDefaultWorld _world = Worlds.Instance.World;
        
        public void RunOnEvent(ref TRequest evt)
        {
            var aspect = _world.GetAspect<TAspect>();
            if (aspect.IsMatches(evt.Target.ID))
            {
                FixedRunOnEvent(ref evt, ref aspect);
            }
        }
        
        public abstract void FixedRunOnEvent(ref TRequest evt, ref TAspect aspect);
    }
    
    public interface IEcsRunOnEvent<T> : IEcsProcess where T : struct, IEcsComponentEvent
    {
        public void RunOnEvent(ref T evt);
    }
    
    public interface IEcsFixedRunOnEvent<T> : IEcsProcess where T : struct, IEcsComponentEvent
    {
        public void RunOnEvent(ref T evt);
    }
    
    public interface IEcsRunOnRequest<T> : IEcsProcess where T : struct, IEcsComponentRequest
    {
        public void RunOnEvent(ref T evt);
    }
    
    public interface IEcsFixedRunOnRequest<T> : IEcsProcess where T : struct, IEcsComponentRequest
    {
        public void RunOnEvent(ref T evt);
    }

    public interface IEcsComponentEvent : IEcsComponent
    {
        public entlong Source { get; set; }
        public entlong Target { get; set; }
    }
    
    public interface IEcsComponentRequest : IEcsComponent
    {
        public entlong Target { get; set; }
    }

    public static class EventCallersWorldExtensions
    {
        public static void SendEvent<T>(this EcsEventWorld world, T evt) where T : struct, IEcsComponentEvent
        {
            world.GetPool<T>().TryAddOrGet(world.NewEntity()) = evt;
        }

        public static void SendRequest<T>(this EcsDefaultWorld world, T evt) where T : struct, IEcsComponentRequest
        {
            world.GetPool<T>().TryAddOrGet(evt.Target.ID) = evt;
        }
    }
}