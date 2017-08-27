using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpIce {
    class HandlerInfo {
        Dictionary<string, Server.EndpointHandler> targets;
        public HandlerInfo() {
            targets = new Dictionary<string, Server.EndpointHandler>();
        }
        public HandlerInfo(Server.EndpointHandler defaultTarget) {
            targets = new Dictionary<string, Server.EndpointHandler>();
            targets[""] = defaultTarget;
        }
        public Server.EndpointHandler GetTarget(Request req) {
            Server.EndpointHandler ret;
            if(targets.TryGetValue(req.Method, out ret)) {
                return ret;
            }
            if(targets.TryGetValue("", out ret)) {
                return ret;
            }
            return null;
        }
        public void AddTargetForMethod(string method, Server.EndpointHandler target) {
            targets[method] = target;
        }
    }

    public class Server {
        public delegate Task<Response> EndpointHandler(Request req);
        unsafe CoreServer* inst;
        Dictionary<int, HandlerInfo> handlers;
        Core.AsyncEndpointHandler endpointCallbackInst;

        public Server() {
            unsafe {
                endpointCallbackInst = endpointCallback;
                inst = Core.ice_create_server();
                Core.ice_server_set_async_endpoint_cb(inst, endpointCallbackInst);
            }
            handlers = new Dictionary<int, HandlerInfo>();
            handlers[-1] = new HandlerInfo(defaultHandler);
        }

        public void Listen(string addr) {
            unsafe {
                Core.ice_server_listen(inst, addr);
            }
        }

        public void DisableRequestLogging() {
            unsafe {
                Core.ice_server_disable_request_logging(inst);
            }
        }

        public uint MaxRequestBodySize {
            set {
                unsafe {
                    Core.ice_server_set_max_request_body_size(inst, value);
                }
            }
        }

        public ulong EndpointTimeoutMs {
            set {
                unsafe {
                    Core.ice_server_set_endpoint_timeout_ms(inst, value);
                }
            }
        }

        public string SessionCookieName {
            set {
                unsafe {
                    Core.ice_server_set_session_cookie_name(inst, value);
                }
            }
        }

        public ulong SessionTimeoutMs {
            set {
                unsafe {
                    Core.ice_server_set_session_timeout_ms(inst, value);
                }
            }
        }

        public void Route(string[] methods, string path, EndpointHandler cb, string[] flags) {
            unsafe {
                CoreEndpoint* ep = Core.ice_server_router_add_endpoint(inst, path);
                foreach(string flag in flags) {
                    Core.ice_core_endpoint_set_flag(ep, flag, true);
                }
                int epId = Core.ice_core_endpoint_get_id(ep);

                HandlerInfo target;
                if(!handlers.TryGetValue(epId, out target)) {
                    target = new HandlerInfo();
                    handlers[epId] = target;
                }
                foreach(string m in methods) {
                    target.AddTargetForMethod(m, cb);
                }
            }
        }

        public void Route(string[] methods, string path, EndpointHandler cb) {
            Route(methods, path, cb, new string[] {});
        }

        public void Route(string method, string path, EndpointHandler cb) {
            Route(new string[] { method }, path, cb);
        }

        public void Route(string path, EndpointHandler cb) {
            Route("", path, cb);
        }

        public void AddTemplate(string name, string content) {
            unsafe {
                bool ret = Core.ice_server_add_template(inst, name, content);
                if(!ret) {
                    throw new System.ArgumentException("Invalid template");
                }
            }
        }

        public void LoadModule(string name, byte[] bitcode) {
            unsafe {
                if(!Core.ice_core_cervus_enabled()) {
                    throw new System.NotImplementedException("Cervus engine not enabled");
                }
                bool ret = Core.ice_server_cervus_load_bitcode(
                    inst,
                    name,
                    bitcode,
                    (uint) bitcode.Length
                );
                if(!ret) {
                    throw new System.ArgumentException("Invalid bitcode");
                }
            }
        }

        private Task<Response> defaultHandler(Request req) {
            return Task.FromResult(req.CreateResponse().SetStatus(404).SetBody("Not found"));
        }

        private unsafe void endpointCallback(int id, CoreCallInfo* callInfo) {
            Task t = realEndpointCallback(id, new Request(callInfo));
        }

        private async Task realEndpointCallback(int id, Request req) {
            try {
                HandlerInfo info = handlers[id];
                EndpointHandler target = info.GetTarget(req);
                if(target == null) {
                    target = handlers[-1].GetTarget(req);
                }
                Response resp;
                
                try {
                    resp = await target(req);
                } catch(System.Exception e) {
                    resp = req.CreateResponse()
                        .SetStatus(500)
                        .SetBody(e.ToString());
                }
                resp.Send();
            } catch(System.Exception e) {
                System.Console.WriteLine(e);
            }
        }
    }
}
