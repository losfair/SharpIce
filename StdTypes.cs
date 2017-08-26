using System.Collections.Generic;

namespace Ice {
    public class StdMap {
        public unsafe static Dictionary<string, string> Deserialize(CoreMap* _begin) {
            Dictionary<string, string> ret = new Dictionary<string, string>();

            if(_begin == null) {
                throw new System.InvalidOperationException("Invalid input");
            }

            byte* begin = (byte*) _begin;

            ushort typeLen = * (ushort*) begin;
            begin += 2;

            string dataType = System.Text.Encoding.UTF8.GetString(begin, (int) typeLen);
            begin += typeLen;

            if(dataType != "map") {
                throw new System.InvalidOperationException("Data is not a StdMap");
            }

            uint itemCount = * (uint*) begin;
            begin += 4;

            for(uint i = 0; i < itemCount; i++) {
                uint kLen = * (uint*) begin;
                begin += 4;

                string key = System.Text.Encoding.UTF8.GetString(begin, (int) kLen);
                begin += kLen + 1;

                uint vLen = * (uint*) begin;
                begin += 4;

                string value = System.Text.Encoding.UTF8.GetString(begin, (int) vLen);
                begin += vLen + 1;

                ret[key] = value;
            }

            return ret;
        }
    }
}
