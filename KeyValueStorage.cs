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

        public async Task ExpireSecAsync(string key, int t) {
            var tcs = new TaskCompletionSource<bool>();

            lock(handleLock) {
                unsafe {
                    if(handle == null) {
                        throw new System.NullReferenceException();
                    }
                    Core.ice_storage_kv_expire_sec(handle, key, (uint) t, (_) => {
                        tcs.SetResult(true);
                    }, null);
                }
            }

            await tcs.Task;
        }
    }

    public class KeyValueStorageHashMapExt {
        KeyValueStorage owner; // Prevent the storage instance from being destroyed
        unsafe CoreHashMapExt* handle;
        object handleLock = new object();

        public unsafe KeyValueStorageHashMapExt(KeyValueStorage _owner, CoreHashMapExt* hm) {
            owner = _owner;
            handle = hm;
        }

        public async Task<string> GetAsync(string key, string mapKey) {
            var tcs = new TaskCompletionSource<string>();

            lock(handleLock) {
                unsafe {
                    if(handle == null) {
                        throw new System.NullReferenceException();
                    }
                    Core.ice_storage_kv_hash_map_ext_get(handle, key, mapKey, (_, retValue) => {
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

        public async Task SetAsync(string key, string mapKey, string value) {
            var tcs = new TaskCompletionSource<bool>();

            lock(handleLock) {
                unsafe {
                    if(handle == null) {
                        throw new System.NullReferenceException();
                    }
                    Core.ice_storage_kv_hash_map_ext_set(handle, key, mapKey, value, (_) => {
                        tcs.SetResult(true);
                    }, null);
                }
            }

            await tcs.Task;
        }

        public async Task RemoveAsync(string key, string mapKey) {
            var tcs = new TaskCompletionSource<bool>();

            lock(handleLock) {
                unsafe {
                    if(handle == null) {
                        throw new System.NullReferenceException();
                    }
                    Core.ice_storage_kv_hash_map_ext_remove(handle, key, mapKey, (_) => {
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

        public KeyValueStorageHashMapExt GetHashMapExt() {
            unsafe {
                CoreHashMapExt* hm = Core.ice_storage_kv_get_hash_map_ext(handle);
                if(hm == null) {
                    throw new System.NullReferenceException();
                }
                return new KeyValueStorageHashMapExt(this, hm);
            }
        }
    }
}
