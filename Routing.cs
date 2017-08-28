using System;

namespace SharpIce {
    public class Routing {
        [AttributeUsage(AttributeTargets.Class)]
        public class Endpoint : Attribute {
            string _path;
            public Endpoint(string path) {
                _path = path;
            }
            public string Path {
                get {
                    return _path;
                }
            }
        }
    }
}
