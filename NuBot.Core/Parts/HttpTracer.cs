using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Gate;

namespace NuBot.Parts
{
    [Export(typeof(IPart))]
    public class HttpTracer : Part
    {
        public override string Name
        {
            get { return "HTTP Traffic Tracer"; }
        }

        public override void AttachToHttpApp(IRobot robo, IAppBuilder app)
        {
            app.UseFunc(next => async (IDictionary<string, object> environment) => {
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
