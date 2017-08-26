namespace Ice {
    public class Request {
        unsafe CoreRequest* inst;
        unsafe CoreCallInfo* callInfo;
        bool responseSent = false;

        public unsafe Request(CoreCallInfo* _callInfo) {
            callInfo = _callInfo;
            inst = Core.ice_core_borrow_request_from_call_info(callInfo);
        }

        ~Request() {
            if(!responseSent) {
                CreateResponse().Send();
            }
        }

        public Response CreateResponse() {
            RequireResponseNotSent();

            unsafe {
                return new Response(this, callInfo);
            }
        }

        public void SetResponseSent() {
            RequireResponseNotSent();
            responseSent = true;

            unsafe {
                inst = null;
                callInfo = null;
            }
        }

        public void RequireResponseNotSent() {
            if(responseSent) {
                throw new System.InvalidOperationException("Response already sent");
            }
        }

        public bool ResponseIsSent() {
            return responseSent;
        }
    }
}
