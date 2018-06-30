using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadedDataRequester : MonoBehaviour {

    private const int maxThreads = 8;
    private const int maxHandlePerUpdate = 3;
	static ThreadedDataRequester instance;
	Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

	void Awake() {
		instance = FindObjectOfType<ThreadedDataRequester> ();
        ThreadPool.SetMaxThreads(maxThreads, maxThreads);
    }

	public static void RequestData(Func<object> generateData, Action<object> callback) {
        // Use a threadpool to calculate data on threads. Way less overhead that starting any number of threads at once
        ThreadPool.QueueUserWorkItem(delegate {
            instance.DataThread(generateData, callback);
        });
	}

	void DataThread(Func<object> generateData, Action<object> callback) {
		object data = generateData ();
		lock (dataQueue) {
			dataQueue.Enqueue(new ThreadInfo (callback, data));
		}
	}
		

	void Update() {
		if (dataQueue.Count > 0) {
            // Don't go overboard and handle too many at once
			for (int i = 0; i < dataQueue.Count && i < maxHandlePerUpdate; i++) {
				ThreadInfo threadInfo = dataQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}
	}

	struct ThreadInfo {
		public readonly Action<object> callback;
		public readonly object parameter;

		public ThreadInfo (Action<object> callback, object parameter)
		{
			this.callback = callback;
			this.parameter = parameter;
		}

	}
}
