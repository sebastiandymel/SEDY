using System;
using System.IO;
using System.Net;
using Autofac;
using Nancy;
using Nancy.Hosting.Self;


namespace FakeVerifit
{
    public class HttpServer : IServer
    {
        private NancyHost server;
        private ILifetimeScope scope;

        public HttpServer(ILifetimeScope scope)
        {
            this.scope = scope;
        }

        public void Run(IPAddress ipAddress)
        {
            var url = $"http://{ipAddress}:80";
            HostConfiguration config = new HostConfiguration();
            config.UrlReservations.CreateAutomatically = true;
            this.server = new NancyHost(new NancyBootstraper(this.scope), config, new Uri(url));

            this.server.Start();
        }


        public void RequestStop()
        {
            this.server.Stop();
            this.server.Dispose();
        }
    }

    // ReSharper disable once UnusedMember.Global
    public class HomeModule : NancyModule
    {
        private IUiBridge bridge;
        public HomeModule(IUiBridge bridge)
        {
            this.bridge = bridge;

            Func<dynamic, object> responseIndexHtml = _ => Response.AsFile("Data/Web/index.html", "text/html");
            Get(@"/", responseIndexHtml);
            Get(@"/index.html", responseIndexHtml);
            Get(@"/banner.png", _ => Response.AsFile(this.bridge.LogoPath, "image/png"));
            Get(@"/services", _ => Response.AsText(GenerateServices(), "text/xml"));
            Get("/screen", _ => Response.FromByteArray(this.bridge.WindowScreenShot, "image/jpeg"));
        }
        private string GenerateServices()
        {
            var xmlFormat = @"<?xml version=""1.0"" encoding=""utf-8"" ?> 
<AudioscanServer model=""{0}"" serial=""{1}"" version=""4.10.2"">
  <Service symbol=""clfs"" name=""Audioscan Closed-Loop Fitting System"" port=""8040"" avail=""1"" enabled=""1""></Service>
</AudioscanServer>";
            return string.Format(xmlFormat, this.bridge.SelectedVerifitModel, "PSSD1");
        }
    }

   

    public class NancyBootstraper : AutofacNancyBootstrapper
    {
        private ILifetimeScope scope;
        public NancyBootstraper(ILifetimeScope scope)
        {
            this.scope = scope;
            var bridge = this.scope.Resolve<IUiBridge>();
        }

        protected override ILifetimeScope GetApplicationContainer()
        {
            return this.scope;
        }

    }

    public class ByteArrayResponse : Response
    {
        /// <summary>
        /// Byte array response
        /// </summary>
        /// <param name="body">Byte array to be the body of the response</param>
        /// <param name="contentType">Content type to use</param>
        public ByteArrayResponse(byte[] body, string contentType = null)
        {
            ContentType = contentType ?? "application/octet-stream";

            Contents = stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(body);
                }
            };
        }
    }

    public static class Extensions
    {
        public static Response FromByteArray(this IResponseFormatter formatter, byte[] body, string contentType = null)
        {
            return new ByteArrayResponse(body, contentType);
        }
    }
}
