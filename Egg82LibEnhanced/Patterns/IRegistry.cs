using System;

namespace Egg82LibEnhanced.Patterns {
	public interface IRegistry<K> {
		//functions
		void SetRegister(K key, dynamic data);
        dynamic RemoveRegister(K key);
        T RemoveRegister<T>(K key);

        dynamic GetRegister(K key);
        T GetRegister<T>(K key);
        K GetKey(dynamic data);
        Type GetRegisterType(K key);

        Type GetKeyClass();

        bool HasRegister(K key);
        bool HasValue(dynamic data);

        void Clear();
        K[] GetKeys();
	}
}
