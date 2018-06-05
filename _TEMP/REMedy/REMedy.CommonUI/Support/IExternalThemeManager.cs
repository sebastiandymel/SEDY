using System;
using System.Collections.Generic;
using Remedy.Core;

namespace FakeVerifit.UI
{
    public interface IExternalThemeManager
    {
        event EventHandler ThemesChanged;
        IEnumerable<ExternalThemeData> ThemeData { get; }
    }
}