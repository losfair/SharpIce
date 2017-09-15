namespace SharpIce {
    public class HttpServerConfig {
        unsafe CoreHttpServerConfig* inst = Core.ice_http_server_config_create();

        public HttpServerConfig() {
        }

        ~HttpServerConfig() {
            unsafe {
                if(inst != null) {
                    Core.ice_http_server_config_destroy(inst);
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

        public unsafe CoreHttpServerConfig* Take() {
            requireValid();

            var currentInst = inst;
            inst = null;
            return currentInst;
        }

        public void SetNumExecutors(int n) {
            requireValid();

            unsafe {
                Core.ice_http_server_config_set_num_executors(
                    inst,
                    (uint) n
                );
            }
        }

        public void SetListenAddress(string addr) {
            requireValid();

            unsafe {
                Core.ice_http_server_config_set_listen_addr(
                    inst,
                    addr
                );
            }
        }
    }
}
