using System;

namespace SharpIce {
    public class HttpMethod {
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
        public class Base : Attribute {
            public virtual string Name {
                get {
                    return null;
                }
            }
        }

        public class Custom : Base {
            string _name;
            public Custom(string name) {
                _name = name.ToUpper();
            }
            public override string Name {
                get {
                    return _name;
                }
            }
        }

        public class Get : Base {
            public override string Name {
                get {
                    return "GET";
                }
            }
        }

        public class Post : Base {
            public override string Name {
                get {
                    return "POST";
                }
            }
        }

        public class Put : Base {
            public override string Name {
                get {
                    return "PUT";
                }
            }
        }

        public class Delete : Base {
            public override string Name {
                get {
                    return "DELETE";
                }
            }
        }

        public class Options : Base {
            public override string Name {
                get {
                    return "OPTIONS";
                }
            }
        }

        public class Head : Base {
            public override string Name {
                get {
                    return "HEAD";
                }
            }
        }

        public class Trace : Base {
            public override string Name {
                get {
                    return "TRACE";
                }
            }
        }
    }
}