using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobNotFound.WinUI.MessageKit;


/// <summary>
/// Configuration for MessageBox creation
/// </summary>
public sealed class MessageBoxOptions
{

    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public MessageBoxButtons Buttons { get; set; } = MessageBoxButtons.OK;
    public MessageBoxIcon Icon { get; set; } = MessageBoxIcon.None;
    public MessageBoxSound Sound { get; set; } = Core.DefaultSound;
    public MessageBoxResult DefaultResult { get; set; } = MessageBoxResult.None;
    public TimeSpan? Timeout { get; set; }
    public ElementTheme? Theme { get; set; }
    public double MaxWidth { get; set; } = 600;
    public double MaxHeight { get; set; } = 400;
    public bool IsModal { get; set; } = true;
    public bool ShowInTaskbar { get; set; } = false;
    public string CustomIconPath { get; set; }
    public List<CustomButton> CustomButtons { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
}