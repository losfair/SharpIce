using Newtonsoft.Json;

namespace SharpIce {
    public class Response {
        unsafe CoreResponse* inst;
        object instLock = new object();
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

        public unsafe CoreResponse* GetCoreInstance() {
            return inst;
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
                lock(instLock) {
                    Core.ice_core_fire_callback(callInfo, inst);
                    callInfo = null;
                    inst = null;
                }
            }
        }
        public Response SetBody(byte[] body) {
            RequireNotSent();

            unsafe {
                lock(instLock) {
                    Core.ice_glue_response_set_body(inst, body, (uint) body.Length);
                }
            }

            return this;
        }
        public Response SetBody(string body) {
            return SetBody(System.Text.Encoding.UTF8.GetBytes(body));
        }

        public Response SetJson(object data) {
            SetHeader("Content-Type", "application/json");
            return SetBody(JsonConvert.SerializeObject(data));
        }

        public Response SetFile(string path) {
            RequireNotSent();

            unsafe {
                lock(instLock) {
                    Core.ice_glue_response_set_file(inst, path);
                }
            }

            return this;
        }

        public Response AddHeader(string key, string value) {
            RequireNotSent();

            unsafe {
                lock(instLock) {
                    Core.ice_glue_response_add_header(inst, key, value);
                }
            }

            return this;
        }

        public Response SetHeader(string key, string value) {
            return AddHeader(key, value);
        }

        public Response SetCookie(string key, string value) {
            RequireNotSent();

            unsafe {
                lock(instLock) {
                    Core.ice_glue_response_set_cookie(inst, key, value);
                }
            }

            return this;
        }

        public Response RenderTemplate(string name, object data) {
            string dataJson = JsonConvert.SerializeObject(data);
            
            unsafe {
                System.IntPtr ret = req.RenderTemplateToOwned(name, dataJson);
                if(ret == System.IntPtr.Zero) {
                    throw new System.ArgumentException("Unable to render template");
                }
                lock(instLock) {
                    Core.ice_glue_response_consume_rendered_template(inst, ret);
                }
            }

            return this;
        }

        public Response SetStatus(ushort status) {
            RequireNotSent();

            unsafe {
                lock(instLock) {
                    Core.ice_glue_response_set_status(inst, status);
                }
            }

            return this;
        }
        public WriteStream CreateStream() {
            req.RequireResponseNotSent();

            unsafe {
                CoreWriteStream* t;
                lock(instLock) {
                    t = Core.ice_glue_response_create_wstream(
                        inst
                    );
                }
                return new WriteStream(t);
            }
        }
    }
}
