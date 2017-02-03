using Egg82LibEnhanced.Patterns.Prototypes;
using System;
using System.Collections.Concurrent;
using System.Runtime;
using System.Threading;

namespace Egg82LibEnhanced.Patterns {
	public class ObjectPool<T> : IDisposable where T : IPrototype {
		//vars
		private ConcurrentBag<T> pool = new ConcurrentBag<T>();
		private T masterInstance;
		private object lockObj = new object();
		private bool _isDynamic = true;

		//constructor
		public ObjectPool(T masterInstance, int numInstances = 0, bool isDynamic = true) {
			if (masterInstance == null) {
				throw new ArgumentNullException("masterInstance");
			}
			if (numInstances < 0) {
				numInstances = 0;
			}
			this.masterInstance = masterInstance;
			_isDynamic = isDynamic;

			AddInstances(numInstances);
		}
		~ObjectPool() {
			Dispose();
		}

		//public
		public int NumFreeInstances {
			get {
				return pool.Count;
			}
		}
		public bool IsDynamic {
			get {
				return _isDynamic;
			}
		}

		public T GetObject() {
			T result;
			return (pool.TryTake(out result)) ? result : ((_isDynamic) ? (T) masterInstance.Clone() : default(T));
		}
		public void ReturnObject(T obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			pool.Add(obj);
		}

		public void AddInstances(int numInstances) {
			if (numInstances <= 0) {
				return;
			}
			for (int i = 0; i < numInstances; i++) {
				pool.Add((T) masterInstance.Clone());
			}
		}

		public void Clear() {
			if (pool.IsEmpty) {
				return;
			}
			
			GCLatencyMode oldMode = GCSettings.LatencyMode;
			GCSettings.LatencyMode = GCLatencyMode.LowLatency;
			
			Interlocked.Exchange(ref pool, new ConcurrentBag<T>());

			GC.Collect();
			GCSettings.LatencyMode = oldMode;
		}
		public void Dispose() {
			GC.Collect();
		}

		//private
		
	}
}
