using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ice {
    public class Request {
        public class SessionView {
            Request req;

            public SessionView(Request _req) {
                req = _req;
            }

            public string Get(string key) {
                req.RequireResponseNotSent();

                string value = null;
                unsafe {
                    System.IntPtr rawValue = Core.ice_glue_request_get_session_item(req.inst, key);
                    if(rawValue != null) {
                        value = Marshal.PtrToStringUTF8(rawValue);
                    }
                }
                return value;
            }

            public void Set(string key, string value) {
                req.RequireResponseNotSent();

                unsafe {
                    Core.ice_glue_request_set_session_item(req.inst, key, value);
                }
            }
        }
        unsafe CoreRequest* inst;
        unsafe CoreCallInfo* callInfo;
        bool responseSent = false;

        public unsafe Request(CoreCallInfo* _callInfo) {
            callInfo = _callInfo;
            inst = Core.ice_core_borrow_request_from_call_info(callInfo);

            Session = new SessionView(this);
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

        private Dictionary<string, string> _cookies = null;
        public Dictionary<string, string> Cookies {
            get {
                RequireResponseNotSent();

                if(_cookies == null) {
                    unsafe {
                        _cookies = StdMap.Deserialize(Core.ice_glue_request_get_cookies(inst));
                    }
                }

                return _cookies;
            }
        }

        private byte[] _body = null;
        public byte[] Body {
            get {
                RequireResponseNotSent();

                if(_body == null) {
                    unsafe {
                        int bodyLen = 0;
                        byte* bodyPtr = Core.ice_glue_request_get_body(inst, ref bodyLen);
                        if(bodyPtr == null || bodyLen == 0) {
                            return null;
                        }
                        _body = new byte [bodyLen];
                        Marshal.Copy((System.IntPtr) bodyPtr, _body, 0, bodyLen);
                    }
                }

                return _body;
            }
        }

        public SessionView Session = null;

        public unsafe CoreContext* Context {
            get {
                RequireResponseNotSent();
                return Core.ice_glue_request_borrow_context(inst);
            }
        }
    }
}
