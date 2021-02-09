using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading;

public class ThreadWorker
{
    private Thread ChildThread = null;
    private EventWaitHandle SuspendHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
    private EventWaitHandle AbortHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
    private bool WantAbort = false;
    
    public IEnumerator task;

    public delegate void OnDestroyActions();

    public ThreadWorker(IEnumerator _task)
    {
        task = _task;
    }

    public void Start()
    {
        ChildThread = new Thread(ThreadLoop);
        ChildThread.Start(task);
    }

    public void Resume() {
        SuspendHandle.Set();
    }

    public void Suspend() {
        if(!WantAbort)
            SuspendHandle.Reset();
    }

    public void Abort(bool block=true) {
        WantAbort = true;

        Resume();

        if(block)
            AbortHandle.WaitOne();
    }

    private void ThreadLoop(object taskObject) {
        try
        {
            var task = taskObject as IEnumerator;

            while(!WantAbort &&  task.MoveNext()) {
                if(WantAbort)
                    break;
                
                SuspendHandle.WaitOne();
            }
        }
        catch (Exception e)
        {
            
            WantAbort = true;
            UnityEngine.Debug.LogException(e);
        }

        AbortHandle.Set();
        ChildThread = null;
    }

    public bool IsRunning() {return ChildThread != null;}
    public bool IsCompleted() {return ChildThread == null;}
}
