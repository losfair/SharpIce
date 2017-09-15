namespace SharpIce {
    public class HttpResponse {
        unsafe CoreHttpResponse* inst = null;

        public HttpResponse() {
            unsafe {
                inst = Core.ice_http_response_create();
            }
        }

        ~HttpResponse() {
            unsafe {
                if(inst != null) {
                    Core.ice_http_response_destroy(inst);
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

        public unsafe CoreHttpResponse* Take() {
            requireValid();

            var currentInst = inst;
            inst = null;
            return currentInst;
        }

        public void SetBody(byte[] body) {
            requireValid();

            unsafe {
                Core.ice_http_response_set_body(
                    inst,
                    body,
                    (uint) body.Length
                );
            }
        }
    }
}
