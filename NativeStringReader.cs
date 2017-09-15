using System.Runtime.InteropServices;

namespace SharpIce {
    class NativeStringReader {
        static unsafe Core.GetStringInstantCallback writeStringDataCallback = new Core.GetStringInstantCallback(writeStringData);

        static unsafe void writeStringData(System.IntPtr value, CoreResource* data) {
            GCHandle handle = (GCHandle) (System.IntPtr) data;
            StringData target = (StringData) handle.Target;

            if(value == System.IntPtr.Zero) {
                target.Value = null;
            } else {
                target.Value = Marshal.PtrToStringUTF8(value);
            }
        }

        public static unsafe string ConsumeOwned(System.IntPtr s) {
            if(s == System.IntPtr.Zero) {
                return null;
            }

            string value = Marshal.PtrToStringUTF8(s);
            Core.ice_glue_destroy_cstring(s);

            return value;
        }

        StringData stringData = new StringData();
        GCHandle handle;

        public NativeStringReader() {
            handle = GCHandle.Alloc(stringData);
        }

        public string Value {
            get {
                return stringData.Value;
            }
        }

        public Core.GetStringInstantCallback Receiver {
            get {
                return writeStringDataCallback;
            }
        }

        public unsafe CoreResource* NativeTarget {
            get {
                return (CoreResource*) (System.IntPtr) handle;
            }
        }
    }

    internal class StringData {
        public string Value = null;
    }
}
