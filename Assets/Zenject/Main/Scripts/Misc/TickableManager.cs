using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public class TickableManager
    {
        [Inject]
        [InjectOptional]
        readonly List<ITickable> _tickables;

        [Inject]
        [InjectOptional]
        readonly List<IFixedTickable> _fixedTickables;

        [Inject]
        [InjectOptional]
        readonly List<Tuple<Type, int>> _priorities;

        [Inject("Fixed")]
        [InjectOptional]
        readonly List<Tuple<Type, int>> _fixedPriorities;

        TaskUpdater<ITickable> _updater;
        TaskUpdater<IFixedTickable> _fixedUpdater;

        [PostInject]
        public void Initialize()
        {
            InitTickables();
            InitFixedTickables();
        }

        void InitFixedTickables()
        {
            _fixedUpdater = new TaskUpdater<IFixedTickable>(UpdateFixedTickable);

            foreach (var type in _fixedPriorities.Select(x => x.First))
            {
                Assert.That(type.DerivesFrom<IFixedTickable>(),
                    "Expected type '{0}' to drive from IFixedTickable while checking priorities in TickableHandler", type.Name());
            }

            var priorityMap = _fixedPriorities.ToDictionary(x => x.First, x => x.Second);

            foreach (var tickable in _fixedTickables)
            {
                int priority;

                if (priorityMap.TryGetValue(tickable.GetType(), out priority))
                {
                    _fixedUpdater.AddTask(tickable, priority);
                }
                else
                {
                    _fixedUpdater.AddTask(tickable);
                }
            }
        }

        void InitTickables()
        {
            _updater = new TaskUpdater<ITickable>(UpdateTickable);

            foreach (var type in _priorities.Select(x => x.First))
            {
                Assert.That(type.DerivesFrom<ITickable>(),
                    "Expected type '{0}' to drive from ITickable while checking priorities in TickableHandler", type.Name());
            }

            var priorityMap = _priorities.ToDictionary(x => x.First, x => x.Second);

            foreach (var tickable in _tickables)
            {
                int priority;

                if (priorityMap.TryGetValue(tickable.GetType(), out priority))
                {
                    _updater.AddTask(tickable, priority);
                }
                else
                {
                    _updater.AddTask(tickable);
                }
            }
        }

        void UpdateFixedTickable(IFixedTickable tickable)
        {
            using (ProfileBlock.Start("{0}.FixedTick()".With(tickable.GetType().Name())))
            {
                tickable.FixedTick();
            }
        }

        void UpdateTickable(ITickable tickable)
        {
            using (ProfileBlock.Start("{0}.Tick()".With(tickable.GetType().Name())))
            {
                tickable.Tick();
            }
        }

        public void Add(ITickable tickable)
        {
            _updater.AddTask(tickable);
        }

        public void Add(ITickable tickable, int priority)
        {
            _updater.AddTask(tickable, priority);
        }

        public void AddFixed(IFixedTickable tickable)
        {
            _fixedUpdater.AddTask(tickable);
        }

        public void AddFixed(IFixedTickable tickable, int priority)
        {
            _fixedUpdater.AddTask(tickable, priority);
        }

        public void Remove(ITickable tickable)
        {
            _updater.RemoveTask(tickable);
        }

        public void RemoveFixed(IFixedTickable tickable)
        {
            _fixedUpdater.RemoveTask(tickable);
        }

        public void Update()
        {
            _updater.OnFrameStart();
            _updater.UpdateAll();
        }

        public void FixedUpdate()
        {
            _fixedUpdater.OnFrameStart();
            _fixedUpdater.UpdateAll();
        }
    }
}
