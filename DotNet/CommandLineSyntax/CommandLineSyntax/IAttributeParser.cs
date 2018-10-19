using System;

namespace CommandLineSyntax
{
    public interface IAttributeParser
    {
        T Parse<T>(params string[] arguments) where T : new();
        void RegisterCustomConverter<T>(Func<string, T> converter) where T : class;
    }
}