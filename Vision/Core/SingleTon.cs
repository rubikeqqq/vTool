using System;

namespace Vision.Core
{
    /// <summary>
    ///     单例,支持多线程创建
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleTon<T> where T : class
    {
        private static readonly Lazy<T> Lazy = new Lazy<T>(Activator.CreateInstance<T>);
        public static T Instance
        {
            get { return Lazy.Value; }
        }
    }
}
