using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Autofac;
using Autofac.log4net;
using log4net.Config;

namespace FakeVerifit
{
    public static class Bootstraper
    {
        public static IEnumerable<IServer> CreateServers(IContainer container)
        {
            return container.Resolve<IEnumerable<IServer>>();
        }

        public static IContainer CreateContainer(params Module[] modules)
        {
            XmlConfigurator.Configure();
            var builder = new ContainerBuilder();
            builder.RegisterModule<FakeVerifitModule>();
            builder.RegisterModule<Log4NetModule>();
            
            foreach (var module in modules)
            {
                builder.RegisterModule(module);
            }
            var container = builder.Build();
            return container;
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}