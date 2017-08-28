using System;

namespace SharpIce
{
    public class CoreFlag {
        [AttributeUsage(AttributeTargets.Class)]
        public class Base : Attribute {
            public virtual string Name {
                get {
                    return null;
                }
            }
        }

        public class InitSession : Base {
            public override string Name {
                get {
                    return "init_session";
                }
            }
        }

        public class ReadBody : Base {
            public override string Name {
                get {
                    return "read_body";
                }
            }
        }
    }
}
