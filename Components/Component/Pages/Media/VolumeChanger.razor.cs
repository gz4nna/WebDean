using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WebDean.Components.Component.Pages.Media;

public partial class VolumeChanger
{
    #region Properties
    private string DisplayText { get; set; } = "Volume";

    private double MaxValue { get; set; } = 100;

    private double MinValue { get; set; } = 0;

    private double CurrentValue { get; set; } = 0;

    private double Step { get; set; } = 1;

    private bool ShowLabel { get; set; } = true;

    private double PreviousValue { get; set; } = 0;
    #endregion

    protected override void OnInitialized()
    {
        CurrentValue = GetCurrentVolume();
    }

    /// <summary>
    /// silence or restore volume
    /// </summary>
    private void Silence()
    {
        if (PreviousValue == 0)
        {
            SetVolume(0);
            PreviousValue = CurrentValue;
        }
        else
        {
            SetVolume((int)PreviousValue);
            CurrentValue = PreviousValue;
            PreviousValue = 0;
        }
    }

    /// <summary>
    /// get current volume
    /// </summary>
    /// <returns></returns>
    private static int GetCurrentVolume()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "amixer",
                Arguments = "get Master",
                RedirectStandardOutput = true, // redirect output
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);

            if (process == null) return 0;

            using var reader = process.StandardOutput;
            string output = reader.ReadToEnd();
            process.WaitForExit();

            var match = Regex.Match(output, @"\[(\d+)%\]");

            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
        }

        return 0;
    }

    private async Task OnValueChanged(double value)
    {
        CurrentValue = value;
        SetVolume((int)CurrentValue);
        // when volume is changed manually, reset PreviousValue as not muted
        PreviousValue = 0;
    }

    /// <summary>
    /// set volume
    /// </summary>
    /// <param name="percent"></param>
    public void SetVolume(int percent)
    {
        string args = $"set Master {percent}%";
        Process.Start("amixer", args);
    }
}