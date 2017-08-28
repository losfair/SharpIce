using System.Threading.Tasks;

namespace SharpIce {
    public class JsonView : View {
        public sealed override async Task<Response> OnRequest(Request req) {
            object data = await Feed(req);
            return req.CreateResponse().SetJson(data);
        }

        protected virtual Task<object> Feed(Request req) {
            throw new System.NotImplementedException();
        }
    }
}
