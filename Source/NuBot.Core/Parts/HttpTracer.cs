using System.ComponentModel.Composition;
using Owin;
using Gate;

namespace NuBot.Parts
{
    [Export(typeof(IPart))]
    public class HttpTracer : Part
    {
        public override string Title
        {
            get { return "HTTP Traffic Tracer"; }
        }

        public override string Name
        {
            get { return "core.trace.http"; }
        }

        public override void AttachToHttpApp(IRobot robo, IAppBuilder app)
        {
            app.UseFunc(next => async environment => {
                var req = new Request(environment);
                TraceRequest(robo, req);
                await next(environment);
                TraceResponse(robo, req, new Response(environment));
            });
        }

        private void TraceRequest(IRobot robo, Request request)
        {
            robo.Log.Trace("<- http {0} {1}", request.Method.PadRight("PATCH".Length), request.Path);
        }

        private void TraceResponse(IRobot robo, Request request, Response response)
        {
            robo.Log.Trace("-> http {0} {1}", response.StatusCode.ToString().PadRight("PATCH".Length), request.Path);
        }
    }
}
