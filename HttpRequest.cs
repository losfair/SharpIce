using System.Runtime.InteropServices;

namespace SharpIce {
    public class HttpRequest {
        unsafe CoreHttpRequest* inst;

        public unsafe HttpRequest(CoreHttpRequest* _inst) {
            inst = _inst;
        }

        public unsafe void MakeInvalid() {
            inst = null;
        }

        void requireValid() {
            unsafe {
                if(inst == null) {
                    throw new System.NullReferenceException();
                }
            }
        }

        public string GetUri() {
            requireValid();

            unsafe {
                return NativeStringReader.ConsumeOwned(
                    Core.ice_http_request_get_uri_to_owned(inst)
                );
            }
        }

        public string GetMethod() {
            requireValid();

            unsafe {
                return NativeStringReader.ConsumeOwned(
                    Core.ice_http_request_get_method_to_owned(inst)
                );
            }
        }

        public string GetRemoteAddress() {
            requireValid();

            unsafe {
                return NativeStringReader.ConsumeOwned(
                    Core.ice_http_request_get_remote_addr_to_owned(inst)
                );
            }
        }
    }
}
