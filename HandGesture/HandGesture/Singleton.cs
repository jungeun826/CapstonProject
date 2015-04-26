using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public interface ISingleTon
    {
        void Init();
    }

    public class Singletone<T> where T : class, ISingleTon, new()
    {
        private static T _instance = null;
        public static T Instance
        {
            get
            {
                lock (typeof(Singletone<T>))
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                        _instance.Init();
                    }
                    return _instance;
                }
            }
        }
    }
}
