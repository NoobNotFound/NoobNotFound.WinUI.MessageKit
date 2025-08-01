using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobNotFound.WinUI.MessageKit;


/// <summary>
/// Custom button configuration
/// </summary>
public sealed class CustomButton
{
    public string Text { get; set; }
    public MessageBoxResult Result { get; set; }
    public bool IsDefault { get; set; }
    public bool IsCancel { get; set; }
    public string ToolTip { get; set; }
    public bool IsEnabled { get; set; } = true;
    public Func<Task<bool>> OnClickAsync { get; set; }
}