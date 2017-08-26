using System.Collections.Generic;
using System.Runtime.InteropServices;

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

        public string Method {
            get {
                RequireResponseNotSent();
                unsafe {
                    return Marshal.PtrToStringUTF8(Core.ice_glue_request_get_method(inst));
                }
            }
        }

        private Dictionary<string, string> _headers = null;
        public Dictionary<string, string> Headers {
            get {
                RequireResponseNotSent();

                if(_headers == null) {
                    unsafe {
                        _headers = StdMap.Deserialize(Core.ice_glue_request_get_headers(inst));
                    }
                }

                return _headers;
            }
        }
    }
}
