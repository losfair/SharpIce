using System.Runtime.InteropServices;

namespace SharpIce {
    public unsafe struct CoreResource {};
    public unsafe struct CoreServer {};
    public unsafe struct CoreEndpoint {};
    public unsafe struct CoreCallInfo {};
    public unsafe struct CoreRequest {};
    public unsafe struct CoreResponse {};
    public unsafe struct CoreMap {};
    public unsafe struct CoreContext {};
    public unsafe struct CoreCustomProperties {};
    public unsafe struct CoreStream {};
    public unsafe struct CoreKVStorage {};
    public unsafe struct CoreHashMapExt {};

    public class Core {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void AsyncEndpointHandler(int id, CoreCallInfo* call_info);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void GetSessionItemCallback(CoreResource* data, System.IntPtr value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void SetSessionItemCallback(CoreResource* data);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void KVStorageGetItemCallback(CoreResource* data, System.IntPtr value);
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void KVStorageSetItemCallback(CoreResource* data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public unsafe delegate void KVStorageRemoveItemCallback(CoreResource* data);

        [DllImport("libice_core")]
        public static extern unsafe CoreServer* ice_create_server();
        [DllImport("libice_core")]
        public static extern unsafe CoreResource* ice_server_listen(
            CoreServer* server,
            string addr
        );
        [DllImport("libice_core")]
        public static extern unsafe CoreEndpoint* ice_server_router_add_endpoint(
            CoreServer* server,
            string path
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_server_set_session_cookie_name(
            CoreServer* server,
            string name
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_server_set_session_timeout_ms(
            CoreServer* server,
            ulong ms
        );
        [DllImport("libice_core")]
        public static extern unsafe bool ice_server_add_template(
            CoreServer* server,
            string name,
            string content
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_server_set_max_request_body_size(
            CoreServer* server,
            uint size
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_server_disable_request_logging(CoreServer* server);
        [DllImport("libice_core")]
        public static extern unsafe void ice_server_set_async_endpoint_cb(
            CoreServer* server,
            AsyncEndpointHandler cb
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_server_set_endpoint_timeout_ms(CoreServer* server, ulong t);
        [DllImport("libice_core")]
        public static extern unsafe void ice_server_set_custom_app_data(CoreServer* server, CoreResource* data);
        [DllImport("libice_core")]
        public static extern unsafe bool ice_server_cervus_load_bitcode(CoreServer* server, string name, byte[] data, uint len);
        [DllImport("libice_core")]
        public static extern unsafe void ice_server_use_redis_session_storage(CoreServer* server, string conn_str);
        [DllImport("libice_core")]
        public static extern unsafe void ice_context_set_custom_app_data(CoreServer* server, CoreResource* data);
        [DllImport("libice_core")]
        public static extern unsafe System.IntPtr ice_glue_request_get_remote_addr(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe System.IntPtr ice_glue_request_get_method(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe System.IntPtr ice_glue_request_get_uri(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe System.IntPtr ice_glue_request_get_session_id(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe System.IntPtr ice_glue_request_get_session_item(
            CoreRequest* req,
            string key
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_request_get_session_item_async(
            CoreRequest* req,
            string key,
            GetSessionItemCallback cb,
            CoreResource* call_with
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_request_set_session_item(
            CoreRequest* req,
            string key,
            string value
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_request_set_session_item_async(
            CoreRequest* req,
            string key,
            string value,
            SetSessionItemCallback cb,
            CoreResource* call_with
        );
        [DllImport("libice_core")]
        public static extern unsafe System.IntPtr ice_glue_request_get_header(
            CoreRequest* req,
            string key
        );
        [DllImport("libice_core")]
        public static extern unsafe CoreMap* ice_glue_request_get_headers(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe System.IntPtr ice_glue_request_get_cookie(
            CoreRequest* req,
            string key
        );
        [DllImport("libice_core")]
        public static extern unsafe CoreMap* ice_glue_request_get_cookies(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe CoreMap* ice_glue_request_get_query(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe byte* ice_glue_request_get_body(
            CoreRequest* req,
            ref int len
        );
        [DllImport("libice_core")]
        public static extern unsafe CoreMap* ice_glue_request_get_body_as_urlencoded(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe System.IntPtr ice_glue_request_render_template_to_owned(
            CoreRequest* req,
            string name,
            string data
        );
        [DllImport("libice_core")]
        public static extern unsafe CoreContext* ice_glue_request_borrow_context(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe CoreCustomProperties* ice_glue_request_borrow_custom_properties(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe CoreMap* ice_glue_request_get_url_params(CoreRequest* req);
        [DllImport("libice_core")]
        public static extern unsafe CoreResponse* ice_glue_create_response();
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_destroy_response(CoreResponse* resp);
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_response_set_body(
            CoreResponse* resp,
            byte[] body,
            uint len
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_response_set_file(
            CoreResponse* resp,
            string path
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_response_set_status(
            CoreResponse* resp,
            ushort status
        );
        [DllImport("libice_core")]
        public static extern unsafe bool ice_glue_response_consume_rendered_template(
            CoreResponse* resp,
            System.IntPtr data
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_response_add_header(
            CoreResponse* resp,
            string key,
            string value
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_glue_response_set_cookie(
            CoreResponse* resp,
            string key,
            string value
        );
        [DllImport("libice_core")]
        public static extern unsafe CoreStream* ice_glue_response_stream(
            CoreResponse* resp,
            CoreContext* ctx
        );
        [DllImport("libice_core")]
        public static extern unsafe bool ice_core_fire_callback(
            CoreCallInfo* call_info,
            CoreResponse* resp
        );
        [DllImport("libice_core")]
        public static extern unsafe CoreRequest* ice_core_borrow_request_from_call_info(
            CoreCallInfo* call_info
        );
        [DllImport("libice_core")]
        public static extern unsafe int ice_core_endpoint_get_id(CoreEndpoint* ep);
        [DllImport("libice_core")]
        public static extern unsafe void ice_core_endpoint_set_flag(
            CoreEndpoint* ep,
            string name,
            bool value
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_core_stream_provider_send_chunk(
            CoreStream* stream,
            byte[] data,
            uint len
        );
        [DllImport("libice_core")]
        public static extern unsafe void ice_core_destroy_stream_provider(
            CoreStream* stream
        );
        [DllImport("libice_core")]
        public static extern unsafe bool ice_core_cervus_enabled();

        [DllImport("libice_core")]
        public static extern unsafe CoreKVStorage* ice_storage_kv_create_with_redis_backend(
            string conn_str
        );

        [DllImport("libice_core")]
        public static extern unsafe void ice_storage_kv_destroy(CoreKVStorage* handle);

        [DllImport("libice_core")]
        public static extern unsafe void ice_storage_kv_get(
            CoreKVStorage* handle,
            string k,
            KVStorageGetItemCallback cb,
            CoreResource* call_with
        );

        [DllImport("libice_core")]
        public static extern unsafe void ice_storage_kv_set(
            CoreKVStorage* handle,
            string k,
            string v,
            KVStorageSetItemCallback cb,
            CoreResource* call_with
        );

        [DllImport("libice_core")]
        public static extern unsafe void ice_storage_kv_remove(
            CoreKVStorage* handle,
            string k,
            KVStorageRemoveItemCallback cb,
            CoreResource* call_with
        );

        [DllImport("libice_core")]
        public static extern unsafe void ice_storage_kv_expire_sec(
            CoreKVStorage* handle,
            string k,
            uint t,
            KVStorageRemoveItemCallback cb,
            CoreResource* call_with
        );

        [DllImport("libice_core")]
        public static extern unsafe CoreHashMapExt* ice_storage_kv_get_hash_map_ext(
            CoreKVStorage* handle
        );

        [DllImport("libice_core")]
        public static extern unsafe void ice_storage_kv_hash_map_ext_get(
            CoreHashMapExt* hm,
            string k,
            string map_key,
            KVStorageGetItemCallback callback,
            CoreResource* call_with
        );

        [DllImport("libice_core")]
        public static extern unsafe void ice_storage_kv_hash_map_ext_set(
            CoreHashMapExt* hm,
            string k,
            string map_key,
            string v,
            KVStorageSetItemCallback callback,
            CoreResource* call_with
        );

        [DllImport("libice_core")]
        public static extern unsafe void ice_storage_kv_hash_map_ext_remove(
            CoreHashMapExt* hm,
            string k,
            string map_key,
            KVStorageRemoveItemCallback callback,
            CoreResource* call_with
        );
    }
}
