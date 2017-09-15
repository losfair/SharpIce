using System.Collections.Generic;

namespace SharpIce {
    public class HttpServer {
        static List<HttpServer> servers = new List<HttpServer>();

        unsafe CoreHttpServer* inst = null;
        bool started = false;
        List<RouteInfo> routeHandles = new List<RouteInfo>();

        public HttpServer(HttpServerConfig cfg) {
            unsafe {
                inst = Core.ice_http_server_create(cfg.Take());
            }
            servers.Add(this);
        }

        ~HttpServer() {
            throw new System.InvalidOperationException("HttpServer should never be destroyed");
        }

        void requireValid() {
            unsafe {
                if(inst == null) {
                    throw new System.NullReferenceException();
                }
            }
        }

        void requireNotStarted() {
            if(started) {
                throw new System.InvalidOperationException("Server already started");
            }
        }

        void requireStarted() {
            if(!started) {
                throw new System.InvalidOperationException("Server not started");
            }
        }

        public void Start() {
            requireValid();
            requireNotStarted();

            unsafe {
                Core.ice_http_server_start(inst);
            }

            started = true;
        }

        public void AddRoute(RouteInfo rt) {
            requireValid();

            unsafe {
                Core.ice_http_server_add_route(
                    inst,
                    rt.Take()
                );
            }

            routeHandles.Add(rt);
        }
    }
}
