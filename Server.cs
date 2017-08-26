using System.Collections.Generic;

namespace Ice {
    public class Server {
        public delegate Response EndpointHandler(Request req);
        unsafe CoreServer* inst;
        Dictionary<int, EndpointHandler> handlers;
        Core.AsyncEndpointHandler endpointCallbackInst;

        public Server() {
            unsafe {
                endpointCallbackInst = endpointCallback;
                inst = Core.ice_create_server();
                Core.ice_server_set_async_endpoint_cb(inst, endpointCallbackInst);
            }
            handlers = new Dictionary<int, EndpointHandler>();
            handlers[-1] = defaultHandler;
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

        private Response defaultHandler(Request req) {
            return req.CreateResponse();
        }

        private unsafe void endpointCallback(int id, CoreCallInfo* callInfo) {
            EndpointHandler target = handlers[id];
            Response resp = target(new Request(callInfo));
            resp.Send();
        }
    }
}
