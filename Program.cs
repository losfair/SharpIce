using System;

namespace Ice
{
    class Program
    {
        static void Main(string[] args)
        {
            Server svr = new Server();
            svr.DisableRequestLogging();
            svr.Listen("127.0.0.1:1218");
            while(true) {
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
