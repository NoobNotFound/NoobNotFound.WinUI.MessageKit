using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobNotFound.WinUI.MessageKit;


/// <summary>
/// Result of MessageBox operation with additional metadata
/// </summary>
public sealed class MessageBoxResponse
{
    public MessageBoxResult Result { get; set; }
    public bool TimedOut { get; set; }
    public TimeSpan ResponseTime { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public Exception Error { get; set; }
}