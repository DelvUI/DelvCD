﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DelvCD.Helpers
{
    public interface IPluginDisposable { 
        void Dispose();
    }

    public static class Singletons
    {
        private static readonly ConcurrentDictionary<Type, object> ActiveInstances = new ConcurrentDictionary<Type, object>();

        public static T Get<T>()
        {
            if (ActiveInstances.TryGetValue(typeof(T), out object? o) && o != null) {
                return (T)o;
            }

            throw new Exception($"Singleton not initialized '{typeof(T).FullName}'.");
        }

        public static bool IsRegistered<T>()
        {
            return ActiveInstances.ContainsKey(typeof(T));
        }

        public static void Register<T>(T newSingleton)
        {
            if (newSingleton == null) { return; }

            if (!ActiveInstances.TryAdd(typeof(T), newSingleton))
            {
                throw new Exception($"Failed to register new singleton for type {newSingleton.GetType()}");
            }
        }

        public static void Dispose()
        {
            foreach (object singleton in ActiveInstances.Values)
            {
                // Only dispose the disposable objects that we own
                if (singleton is IPluginDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            ActiveInstances.Clear();
        }
    }
}