using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Zenject;

public class AsyncTaskProcessor : ITickable
{
    public event Action LoadStarted = delegate { };
    public event Action LoadComplete = delegate { };

    // New workers to prevent a process from being popped before completion
    List<WorkerData> _newWorkers = new List<WorkerData>();

    // We use a stack here because otherwise sub process of current workers would never execute
    // causing a state of limbo.
    Stack<WorkerData> _workers = new Stack<WorkerData>();

    public AsyncTaskProcessor()
    {
    }

    public bool IsRunning
    {
        get
        {
            return _workers.Any() || _newWorkers.Any();
        }
    }

    public void Tick()
    {
        AddNewWorkers(); //Adding newworkers waiting to be added

        if (!_workers.Any())
        {
            return;
        }

        var topWorker = _workers.Peek();

        if (!topWorker.Process.MoveNext())
        {
            Assert.That(topWorker == _workers.Peek());//Make sure the worker being removed is the worker that is finished
            _workers.Pop();

            OnFinishedWorker(topWorker);
        }

        var subProcess = topWorker.Process.Current as IEnumerator;
        if (subProcess != null)
        {
            _workers.Push(new WorkerData
            {
                Process = subProcess
            });
        }

        AddNewWorkers(); //Added any workers that might have been added when the last worker was removed

        if (!_workers.Any())
        {
            LoadComplete();
        }
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
            _workers.Push(worker);
        }
        _newWorkers.Clear();
    }

    class WorkerData
    {
        public IEnumerator Process;
        public Action<object> Callback;
    }
}