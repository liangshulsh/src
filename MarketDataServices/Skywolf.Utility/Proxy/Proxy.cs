using System;
using System.ServiceModel;

namespace Skywolf.Utility.Proxy
{
    public static class Proxy<T>
        where T : ICommunicationObject, new()
    {
        public static void Call(Action<T> action)
        {
            var proxy = new T();

            try
            {
                action(proxy);
                proxy.Close();
            }
            catch
            {
                proxy.Abort();
                throw;
            }
        }

        public static V Call<V>(Func<T, V> func)
        {
            V v = default(V);

            Call(client => { v = func(client); });

            return v;
        }
    }
}
