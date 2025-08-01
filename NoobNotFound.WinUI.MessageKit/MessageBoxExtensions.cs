using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobNotFound.WinUI.MessageKit;
/// <summary>
/// Extension methods for easier MessageBox usage
/// </summary>
public static class MessageBoxExtensions
{
    internal static string Localize(this string text)
    {
        return Core.Localize(text);
    }

    public static MessageBoxOptions WithTimeout(this MessageBoxOptions options, TimeSpan timeout)
    {
        options.Timeout = timeout;
        return options;
    }

    public static MessageBoxOptions WithIcon(this MessageBoxOptions options, MessageBoxIcon icon)
    {
        options.Icon = icon;
        return options;
    }

    public static MessageBoxOptions WithSound(this MessageBoxOptions options, MessageBoxSound sound)
    {
        options.Sound = sound;
        return options;
    }

    public static MessageBoxOptions WithMetadata(this MessageBoxOptions options, string key, object value)
    {
        options.Metadata[key] = value;
        return options;
    }
}