using System;
using System.Linq;
using ModestTree;
using ModestTree.Zenject;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallelAsyncTaskProcessor : ITickable
{
    public event Action LoadStarted = delegate { };
    public event Action LoadComplete = delegate { };

    // New workers to prevent a process from being popped before completion
    readonly List<WorkerData> _newWorkers = new List<WorkerData>();

    //Gonna try and have the ability to have multiple workers by tracking multiple workers that can have a stack
    List<Stack<WorkerData>> _workerStacks = new List<Stack<WorkerData>>();

    public ParallelAsyncTaskProcessor()
    {
    }

    public bool IsRunning
    {
        get
        {
            return _workerStacks.Any() || _newWorkers.Any();
        }
    }

    public void Tick()
    {
        AddNewWorkers(); //Adding newworkers waiting to be added

        if (!_workerStacks.Any())
        {
            return;
        }

        foreach (var worker in _workerStacks)
        {
            var topWorker = worker.Peek();

            if (!topWorker.Process.MoveNext())
            {
                Assert.That(topWorker == worker.Peek()); //Make sure the worker being removed is the worker that is finished
                worker.Pop();

                OnFinishedWorker(topWorker);
            }

            var subProcess = topWorker.Process.Current as IEnumerator;
            if (subProcess != null)
            {
                worker.Push(new WorkerData
                {
                    Process = subProcess
                });
            }
        }

        CullWorkerStacks();

        AddNewWorkers(); //Added any workers that might have been added when the last worker was removed

        if (!_workerStacks.Any())
        {
            LoadComplete();
        }
    }

    private void CullWorkerStacks()
    {
        _workerStacks = _workerStacks.Where(w => !w.IsEmpty()).ToList();
    }

    void OnFinishedWorker(WorkerData worker)
    {
        var result = worker.Process.Current;
        Assert.That(!worker.Process.MoveNext());

        if (worker.Callback != null)
        {
            worker.Callback(result);
        }
    }

    public void Process(IEnumerator process)
    {
        ProcessInternal(process, null);
    }

    public void Process(IEnumerator process, Action callback)
    {
        ProcessInternal(process, delegate { callback(); });
    }

    public void Process<T>(IEnumerator process, Action<T> callback)
    {
        ProcessInternal(process, ConvertDelegate(callback));
    }

    public void Process<T>(IEnumerator<T> process, Action<T> callback)
    {
        ProcessInternal(process, ConvertDelegate(callback));
    }

    Action<object> ConvertDelegate<T>(Action<T> callback)
    {
        if (callback == null)
        {
            return null;
        }

        return delegate(object result) { callback((T)result); };
    }

    public void ProcessInternal(
        IEnumerator process, Action<object> callback)
    {
        if (!IsRunning)
        {
            LoadStarted();
        }

        _newWorkers.Add(
            new WorkerData()
            {
                Process = process,
                Callback = callback,
            });
    }

    void AddNewWorkers()
    {
        foreach (var worker in _newWorkers)
        {
            var newStack = new Stack<WorkerData>();
            newStack.Push(worker);
            _workerStacks.Add(newStack);
        }
        _newWorkers.Clear();
    }

    class WorkerData
    {
        public IEnumerator Process;
        public Action<object> Callback;
    }
}
