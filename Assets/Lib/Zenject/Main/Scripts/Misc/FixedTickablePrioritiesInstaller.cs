using System;
using System.Collections.Generic;
using ModestTree.Zenject;

namespace ModestTree.Zenject
{
    public class FixedTickablePrioritiesInstaller : Installer
    {
        List<Type> _tickables;

        public FixedTickablePrioritiesInstaller(
            List<Type> tickables)
        {
            _tickables = tickables;
        }

        public override void InstallBindings()
        {
            int priorityCount = 1;

            foreach (var tickableType in _tickables)
            {
                BindPriority(Container, tickableType, priorityCount);
                priorityCount++;
            }
        }

        public static void BindPriority(
            DiContainer container, Type tickableType, int priorityCount)
        {
            Assert.That(tickableType.DerivesFrom<IFixedTickable>(),
                "Expected type '{0}' to derive from IFixedTickable", tickableType.Name());

            container.Bind<Tuple<Type, int>>().To(
                Tuple.New(tickableType, priorityCount)).WhenInjectedInto<TickableManager>()
                .As("Fixed");
        }
    }
}

