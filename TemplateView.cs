using System.Threading.Tasks;
using System.Collections.Generic;

namespace SharpIce {
    public class TemplateView : View {
        private string filePath = null;
        private string content = null;

        public TemplateView(string _filePath, string _path, string[] _methods, string[] _flags)
            : base(_path, _methods, _flags) {
                filePath = _filePath;
        }
        public TemplateView(string _filePath, string _path, string[] _methods)
            : base(_path, _methods) {
                filePath = _filePath;
        }
        public TemplateView(string _filePath, string _path)
            : base(_path) {
                filePath = _filePath;
        }

        protected override void Prepare(Server svr) {
            content = System.IO.File.ReadAllText(filePath);
            svr.AddTemplate(filePath, content);
        }

        public override async Task<Response> OnRequest(Request req) {
            if(filePath == null) {
                throw new System.InvalidOperationException("Template file path not set");
            }
            if(content == null) {
                throw new System.InvalidOperationException("Template not prepared");
            }

            object data = await Feed(req);
            return req.CreateResponse().RenderTemplate(filePath, data);
        }

        protected virtual Task<object> Feed(Request req) {
            return Task.FromResult((object) new Dictionary<string, string>());
        }
    }
}
