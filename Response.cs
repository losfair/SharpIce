namespace Ice {
    public class Response {
        unsafe CoreResponse* inst;
        unsafe CoreCallInfo* callInfo;
        Request req;
        bool sent = false;
        public unsafe Response(Request _req, CoreCallInfo* _callInfo) {
            req = _req;
            callInfo = _callInfo;
            inst = Core.ice_glue_create_response();
        }

        ~Response() {
            if(!sent) {
                unsafe {
                    Core.ice_glue_destroy_response(inst);
                }
            }
        }

        public void Send() {
            req.RequireResponseNotSent();
            req.SetResponseSent();
            sent = true;

            unsafe {
                Core.ice_core_fire_callback(callInfo, inst);
                callInfo = null;
                inst = null;
            }
        }
        public void SetBody(byte[] body) {
            unsafe {
                Core.ice_glue_response_set_body(inst, body, (uint) body.Length);
            }
        }
    }
}
