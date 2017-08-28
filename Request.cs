using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace SharpIce {
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
                    System.IntPtr rawValue;
                    lock(req.instLock) {
                        rawValue = Core.ice_glue_request_get_session_item(req.inst, key);
                    }
                    if(rawValue != null) {
                        value = Marshal.PtrToStringUTF8(rawValue);
                    }
                }
                return value;
            }

            public void Set(string key, string value) {
                req.RequireResponseNotSent();

                unsafe {
                    lock(req.instLock) {
                        Core.ice_glue_request_set_session_item(req.inst, key, value);
                    }
                }
            }
        }
        unsafe CoreRequest* inst;
        object instLock = new object();
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

        public unsafe CoreRequest* GetCoreInstance() {
            return inst;
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

        private Dictionary<string, string> _headers = null;
        public Dictionary<string, string> Headers {
            get {
                RequireResponseNotSent();

                if(_headers == null) {
                    unsafe {
                        lock(instLock) {
                            _headers = StdMap.Deserialize(Core.ice_glue_request_get_headers(inst));
                        }
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
                        lock(instLock) {
                            _cookies = StdMap.Deserialize(Core.ice_glue_request_get_cookies(inst));
                        }
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
                        byte* bodyPtr;
                        lock(instLock) {
                            bodyPtr = Core.ice_glue_request_get_body(inst, ref bodyLen);
                        }
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

        public T ParseJsonBody<T>() {
            return JsonConvert.DeserializeObject<T>(
                System.Text.Encoding.UTF8.GetString(Body)
            );
        }

        public Dictionary<string, string> ParseUrlencodedBody() {
            RequireResponseNotSent();
            lock(instLock) {
                unsafe {
                    return StdMap.Deserialize(Core.ice_glue_request_get_body_as_urlencoded(inst));
                }
            }
        }

        private Dictionary<string, string> _query = null;
        public Dictionary<string, string> Query {
            get {
                RequireResponseNotSent();
                if(_query == null) {
                    unsafe {
                        lock(instLock) {
                            _query = StdMap.Deserialize(
                                Core.ice_glue_request_get_query(inst)
                            );
                        }
                    }
                }
                return _query;
            }
        }

        private Dictionary<string, string> _urlParams = null;
        public Dictionary<string, string> UrlParams {
            get {
                RequireResponseNotSent();
                if(_urlParams == null) {
                    unsafe {
                        lock(instLock) {
                            _urlParams = StdMap.Deserialize(
                                Core.ice_glue_request_get_url_params(inst)
                            );
                        }
                    }
                }
                return _urlParams;
            }
        }

        public SessionView Session = null;

        public unsafe CoreContext* Context {
            get {
                RequireResponseNotSent();
                lock(instLock) {
                    return Core.ice_glue_request_borrow_context(inst);
                }
            }
        }

        private string _method = null;
        public string Method {
            get {
                RequireResponseNotSent();

                if(_method == null) {
                    lock(instLock) {
                        unsafe {
                            _method = Marshal.PtrToStringUTF8(
                                Core.ice_glue_request_get_method(inst)
                            );
                        }
                    }
                }
                return _method;
            }
        }

        private string _remoteAddr = null;
        public string RemoteAddr {
            get {
                RequireResponseNotSent();
                if(_remoteAddr == null) {
                    lock(instLock) {
                        unsafe {
                            _remoteAddr = Marshal.PtrToStringUTF8(
                                Core.ice_glue_request_get_remote_addr(inst)
                            );
                        }
                    }
                }
                return _remoteAddr;
            }
        }

        private string _uri = null;
        public string Uri {
            get {
                RequireResponseNotSent();
                if(_uri == null) {
                    lock(instLock) {
                        unsafe {
                            _uri = Marshal.PtrToStringUTF8(
                                Core.ice_glue_request_get_uri(inst)
                            );
                        }
                    }
                }
                return _uri;
            }
        }

        public unsafe System.IntPtr RenderTemplateToOwned(string name, string data) {
            RequireResponseNotSent();
            lock(instLock) {
                return Core.ice_glue_request_render_template_to_owned(inst, name, data);
            }
        }
    }
}
