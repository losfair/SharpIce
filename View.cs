using System.Threading.Tasks;
using System.Collections.Generic;

namespace SharpIce {
    public class View {
        View parent = null;
        List<View> children = new List<View>();
        protected string path = null;
        protected string[] methods = new string[] { "GET" };
        protected string[] flags = new string[] {};

        public View(string _path, string[] _methods, string[] _flags) {
            path = _path;
            methods = _methods;
            flags = _flags;
        }

        public View(string _path, string[] _methods) {
            path = _path;
            methods = _methods;
        }

        public View(string _path) {
            path = _path;
        }

        public void AddToServer(Server svr) {
            Prepare(svr);

            svr.Route(methods, FullPath, OnRequest, flags);
            svr.RouteFallback(methods, FullPath, OnRequestFallback);

            foreach(View v in children) {
                v.AddToServer(svr);
            }
        }

        protected virtual void Prepare(Server svr) {}

        public virtual Task<Response> OnRequest(Request req) {
            return Task.FromResult(
                req.CreateResponse().SetStatus(500).SetBody("Not implemented")
            );
        }

        public virtual Task<Response> OnRequestFallback(Request req) {
            return Task.FromResult(
                req.CreateResponse().SetStatus(404).SetBody("View not found")
            );
        }

        public void AttachTo(View _parent) {
            parent = _parent;
            parent.children.Add(this);
        }

        public void Attach(View child) {
            child.AttachTo(this);
        }

        public string FullPath {
            get {
                string ret = rawFullPath;
                if(ret.Length == 0) {
                    return "/";
                }
                return ret;
            }
        }

        private string rawFullPath {
            get {
                string ret = "";
                if(parent != null) ret = parent.rawFullPath;
                string nlp = normalizedLocalPath;
                if(nlp.Length != 0) {
                    ret += "/";
                    ret += nlp;
                }
                return ret;
            }
        }

        private string normalizedLocalPath {
            get {
                return path.Trim('/');
            }
        }
    }
}
