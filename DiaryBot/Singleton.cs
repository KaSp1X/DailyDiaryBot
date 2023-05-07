using System;

namespace DiaryBot
{
    public abstract class Singleton<T> where T : Singleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get => _instance ??= new Lazy<T>(() => (Activator.CreateInstance(typeof(T), true) as T)!).Value;
            private set => _instance = value;
        }

        public static void ClearInstance() => Instance = null;
    }
}