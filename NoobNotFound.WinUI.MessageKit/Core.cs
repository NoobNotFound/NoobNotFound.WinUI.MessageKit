using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobNotFound.WinUI.MessageKit;

public static class Core
{
    /// <summary>
    /// The default sound played by message boxes.
    /// </summary>
    public static MessageBoxSound DefaultSound { get; set; } = MessageBoxSound.Default;

    /// <summary>
    /// Delegate to provide the current XamlRoot (usually from the main window or frame).
    /// </summary>
    public static Func<XamlRoot?>? XamlRootProvider { get; set; }

    /// <summary>
    /// Delegate to localize message text globally.
    /// </summary>
    public static Func<string, string>? LocalizationProvider { get; set; }

    /// <summary>
    /// Delegate to provide the current application theme.
    /// </summary>
    public static Func<ElementTheme>? ThemeProvider { get; set; }



    /// <summary>
    /// Gets the current XamlRoot if available.
    /// </summary>
    internal static XamlRoot? GetXamlRoot()
    {
        return XamlRootProvider?.Invoke();
    }

    /// <summary>
    /// Localizes the given text using the configured provider, or returns the original string.
    /// </summary>
    internal static string Localize(string text)
    {
        return LocalizationProvider?.Invoke(text) ?? text;
    }
    /// <summary>
    /// Gets the current theme if available.
    /// </summary>
    internal static ElementTheme GetCurrentTheme()
    {
        return ThemeProvider?.Invoke() ?? ElementTheme.Default;
    }
}