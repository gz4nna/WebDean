using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace WebDean.Components.Component.Pages.SystemControls;

partial class ProcessListCard : ComponentBase
{
    [NotNull]
    // Modal reference
    private Modal? BackdropModal { get; set; }

    protected override void OnInitialized()
    {
        #region get process list
        GetProcessList();
        #endregion
    }

    /// <summary>
    /// get process list
    /// </summary>
    /// <returns></returns>
    private static List<ProcessInfo> GetProcessList() => [.. Process.GetProcesses().Select(
        p => new ProcessInfo{
            ProcessName = p.ProcessName,
            Id = p.Id,
            WorkingSet64 = p.WorkingSet64,
            StartTime = p.StartTime
        }
    )];
}

/// <summary>
/// process info
/// </summary>
public class ProcessInfo
{
    public string ProcessName { get; set; } = string.Empty;
    public int Id { get; set; }
    public long WorkingSet64 { get; set; }
    public DateTime StartTime { get; set; }
}