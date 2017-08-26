namespace SharpIce {
    public class Stream {
        unsafe CoreStream* inst;
        bool closed = false;

        public unsafe Stream(CoreStream* _inst) {
            inst = _inst;
        }

        ~Stream() {
            if(!closed) {
                Close();
            }
        }

        public void RequireActive() {
            if(closed) {
                throw new System.InvalidOperationException(
                    "Stream already closed"
                );
            }
        }

        public void Write(byte[] data) {
            RequireActive();
            unsafe {
                Core.ice_core_stream_provider_send_chunk(
                    inst,
                    data,
                    (uint) data.Length
                );
            }
        }

        public void Write(string data) {
            Write(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public void Close() {
            RequireActive();
            closed = true;
            unsafe {
                Core.ice_core_destroy_stream_provider(inst);
                inst = null;
            }
        }
    }
}
