using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace WebDean.Components.Component.Pages.SystemControls;

public partial class PowerManageCard : ComponentBase
{
    private bool IsValid() =>
        !string.IsNullOrEmpty(PowerAction) &&
        int.TryParse(PowerActionArgument, out int delay) &&
        delay >= 0;

    private async Task ExecuteCommand(string? command, string? delaySeconds)
    {
        if (!IsValid())
        {
            await ErrorToast.Error("Execute Failed", "Invalid input parameters. Please select a power action and input a valid delay in seconds.");
            return;
        }

        _ = int.TryParse(delaySeconds, out int delaySecondsInt);

        switch (command)
        {
            case "powerOff":
                PowerOff(delaySecondsInt);
                break;
            case "reboot":
                Reboot(delaySecondsInt);
                break;
            case "suspend":
                Suspend(delaySecondsInt);
                break;
            case "hibernate":
                Hibernate(delaySecondsInt);
                break;
        }
    }
    private static void PowerOff(int delaySeconds = 0)
    {
        System.Diagnostics.Process.Start("systemctl", "poweroff");

    }

    private static void Reboot(int delaySeconds = 0)
    {
        System.Diagnostics.Process.Start("systemctl", "reboot");

    }

    private static void Suspend(int delaySeconds = 0)
    {
        System.Diagnostics.Process.Start("systemctl", "suspend");
    }

    private static void Hibernate(int delaySeconds = 0)
    {
        System.Diagnostics.Process.Start("systemctl", "hibernate");
    }
}
