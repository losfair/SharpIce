namespace SharpIce {
    public class WriteStream {
        unsafe CoreWriteStream* inst;
        bool closed = false;

        public unsafe WriteStream(CoreWriteStream* _inst) {
            inst = _inst;
        }

        ~WriteStream() {
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
                Core.ice_stream_wstream_write(
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
                Core.ice_stream_wstream_destroy(inst);
                inst = null;
            }
        }
    }
}
