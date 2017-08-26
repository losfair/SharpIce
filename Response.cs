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

        public void RequireNotSent() {
            if(sent) {
                throw new System.InvalidOperationException("Already sent");
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
        public Response SetBody(byte[] body) {
            RequireNotSent();

            unsafe {
                Core.ice_glue_response_set_body(inst, body, (uint) body.Length);
            }

            return this;
        }
        public Response SetBody(string body) {
            return SetBody(System.Text.Encoding.UTF8.GetBytes(body));
        }
        public Response SetStatus(ushort status) {
            RequireNotSent();

            unsafe {
                Core.ice_glue_response_set_status(inst, status);
            }

            return this;
        }
    }
}
