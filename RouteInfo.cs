using System.Threading.Tasks;

namespace SharpIce {
    public class RouteInfo {
        unsafe CoreRouteInfo* inst = null;
        Core.RouteCallback cbHandle = null;
        public delegate Task<HttpResponse> EndpointCallback(HttpRequest req);
        EndpointCallback localCallback;

        public RouteInfo(string path, EndpointCallback _localCallback) {
            localCallback = _localCallback;

            unsafe {
                cbHandle = new Core.RouteCallback(endpointCallback);
                inst = Core.ice_http_server_route_create(
                    path,
                    cbHandle,
                    null
                );
            }
        }

        ~RouteInfo() {
            unsafe {
                if(inst != null) {
                    Core.ice_http_server_route_destroy(inst);
                }
            }
        }

        void requireValid() {
            unsafe {
                if(inst == null) {
                    throw new System.NullReferenceException();
                }
            }
        }

        public unsafe CoreRouteInfo* Take() {
            requireValid();

            CoreRouteInfo* currentInst = inst;
            inst = null;
            return currentInst;
        }

        public unsafe void endpointCallback(
            CoreEndpointContext* rawCtx,
            CoreHttpRequest* rawReq,
            CoreResource* call_with
        ) {
            EndpointContext ctx = new EndpointContext(rawCtx);
            HttpRequest req = new HttpRequest(rawReq);

            realEndpointCallback(ctx, req);
        }

        async void realEndpointCallback(
            EndpointContext context,
            HttpRequest req
        ) {
            HttpResponse resp = await localCallback(req);
            context.End(resp);
        }
    }

    internal class EndpointContext {
        unsafe CoreEndpointContext* inst = null;

        public unsafe EndpointContext(CoreEndpointContext* _inst) {
            inst = _inst;
        }

        void requireValid() {
            unsafe {
                if(inst == null) {
                    throw new System.NullReferenceException();
                }
            }
        }

        public void End(HttpResponse resp) {
            requireValid();
            unsafe {
                Core.ice_http_server_endpoint_context_end_with_response(
                    inst,
                    resp.Take()
                );
                inst = null;
            }
        }
    }
}
