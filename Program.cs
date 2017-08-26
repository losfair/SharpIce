﻿using System.Threading.Tasks;

namespace Ice
{
    class Program
    {
        static void Main(string[] args)
        {
            Server svr = new Server();
            svr.DisableRequestLogging();

            svr.Route("GET", "/hello_world", (req) => {
                return Task.FromResult(req.CreateResponse().SetBody("Hello world!"));
            });
            svr.Route(new string[] { "GET" }, "/info", (req) => {
                var headers = req.Headers;
                string t = "";
                foreach(var p in headers) {
                    t += p.Key;
                    t += ": ";
                    t += p.Value;
                    t += "\n";
                }
                return Task.FromResult(req.CreateResponse().SetBody(t));
            });
            svr.Route(new string[] { "POST" }, "/echo", (req) => {
                return Task.FromResult(req.CreateResponse().SetBody(req.Body));
            }, new string[] { "read_body" });

            svr.Listen("127.0.0.1:1218");
            while(true) {
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
