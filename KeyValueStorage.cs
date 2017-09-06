using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SharpIce {
    public class KeyValueStorage {
        protected unsafe CoreKVStorage* handle;
        protected object handleLock = new object();

        protected KeyValueStorage() {}

        ~KeyValueStorage() {
            lock(handleLock) {
                unsafe {
                    if(handle == null) {
                        return;
                    }
                    Core.ice_storage_kv_destroy(handle);
                    handle = null;
                }
            }
        }

        public async Task<string> GetAsync(string key) {
            var tcs = new TaskCompletionSource<string>();

            lock(handleLock) {
                unsafe {
                    if(handle == null) {
                        throw new System.NullReferenceException();
                    }
                    Core.ice_storage_kv_get(handle, key, (_, retValue) => {
                        if(retValue != System.IntPtr.Zero) {
                            tcs.SetResult(Marshal.PtrToStringUTF8(retValue));
                        } else {
                            tcs.SetResult(null);
                        }
                    }, null);
                }
            }

            return await tcs.Task;
        }

        public async Task SetAsync(string key, string value) {
            var tcs = new TaskCompletionSource<bool>();

            lock(handleLock) {
                unsafe {
                    if(handle == null) {
                        throw new System.NullReferenceException();
                    }
                    Core.ice_storage_kv_set(handle, key, value, (_) => {
                        tcs.SetResult(true);
                    }, null);
                }
            }

            await tcs.Task;
        }

        public async Task RemoveAsync(string key) {
            var tcs = new TaskCompletionSource<bool>();

            lock(handleLock) {
                unsafe {
                    if(handle == null) {
                        throw new System.NullReferenceException();
                    }
                    Core.ice_storage_kv_remove(handle, key, (_) => {
                        tcs.SetResult(true);
                    }, null);
                }
            }

            await tcs.Task;
        }
    }

    public class RedisStorage : KeyValueStorage {
        public RedisStorage(string connStr) {
            unsafe {
                handle = Core.ice_storage_kv_create_with_redis_backend(connStr);
            }
        }
    }
}
