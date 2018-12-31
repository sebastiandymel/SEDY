using System;

namespace PhoenixStyleBrowser
{
    internal interface IGroupBuilder
    {
        void Add(Type type, string key, object res);
        bool IsEmpty();
        ResourceGroup Get();
    }
}