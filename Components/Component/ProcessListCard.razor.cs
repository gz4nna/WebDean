using System.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace WebDean.Components.Component;

partial class ProcessListCard : ComponentBase
{
    public class ProcessInfo
    {
        public string ProcessName { get; set; } = string.Empty;
        public int Id { get; set; }
        public long WorkingSet64 { get; set; }
        public DateTime StartTime { get; set; }
    }

    protected override void OnInitialized()
    {
        #region get process list
        GetProcessList();
        #endregion
    }
    private static List<ProcessInfo> GetProcessList() => [.. Process.GetProcesses().Select(
        p => new ProcessInfo{
            ProcessName = p.ProcessName,
            Id = p.Id,
            WorkingSet64 = p.WorkingSet64,
            StartTime = p.StartTime
        }
    )];

}