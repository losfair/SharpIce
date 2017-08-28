using System.Threading.Tasks;

namespace SharpIce {
    public class StaticDirectoryView : View {
        string staticPath = null;
        string urlPrefix = null;
        public StaticDirectoryView(string _path) {
            staticPath = System.IO.Path.GetFullPath(_path);
        }

        public override Task<Response> OnRequestFallback(Request req) {
            if(urlPrefix == null) {
                urlPrefix = FullPath;
            }

            string targetPath = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(
                    staticPath,
                    req.Url.Substring(urlPrefix.Length).Trim('/')
                )
            );
            
            if(
                !targetPath.StartsWith(staticPath + "/", System.StringComparison.Ordinal)
                && !targetPath.StartsWith(staticPath + "\\", System.StringComparison.Ordinal)
            ) {
                return Task.FromResult(req.CreateResponse().SetStatus(403));
            }

            return Task.FromResult(
                req.CreateResponse().SetFile(targetPath)
            );
        }
    }
}