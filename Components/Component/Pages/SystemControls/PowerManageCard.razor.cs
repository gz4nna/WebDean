using System.Diagnostics.CodeAnalysis;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace WebDean.Components.Component.Pages.SystemControls;

public partial class PowerManageCard : ComponentBase
{
    [NotNull]
    private Modal? BackdropModal { get; set; }

    [Inject]
    [NotNull]
    private ToastService? ErrorToast { get; set; }

    /// <summary>
    /// The selected power action to execute.
    /// </summary>
    private string? PowerAction = "";

    /// <summary>
    /// The argument for the power action, delay in seconds.
    /// </summary>
    private string? PowerActionArgument = "0";

#if DEBUG
    /// <summary>
    /// use a unavailable power option for debug mode
    /// </summary>
    private IEnumerable<SelectedItem> PowerOptions { get; set; } = new[]
    {
new SelectedItem("aaa","aaa"),
new SelectedItem("hibernate","hibernate"),
new SelectedItem("suspend","suspend"),
new SelectedItem("reboot","reboot")
};
#elif RELEASE
private IEnumerable<SelectedItem> PowerOptions { get; set; } = new[]
{
new SelectedItem("poweroff","poweroff"),
new SelectedItem("hibernate","hibernate"),
new SelectedItem("suspend","suspend"),
new SelectedItem("reboot","reboot")
};
#endif

    /// <summary>
    /// Validates the user input for power action and delay.
    /// </summary>
    /// <returns></returns>
    private bool IsValid() =>
        !string.IsNullOrEmpty(PowerAction) &&
        int.TryParse(PowerActionArgument, out int delay) &&
        delay >= 0;

    /// <summary>
    /// Handles the execution of the power command when the execute button is clicked.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="delaySeconds"></param>
    /// <returns></returns>
    private async Task<Task> OnExecuteButtonClick(string? command, string? delaySeconds)
    {

        if (!IsValid())
        {
            await ErrorToast.Error("Execute Failed", "Invalid input parameters. Please select a power action and input a valid delay in seconds.");
            return Task.FromException(new ArgumentException("Invalid input parameters."));
        }

        _ = int.TryParse(delaySeconds, out int delaySecondsInt);

        try
        {
            await ExecuteCommand(delaySecondsInt, command!);
        }
        catch (Exception ex)
        {
            await ErrorToast.Error("Execute Failed", $"An error occurred while executing the power command: {ex.Message}");
            return Task.FromException(ex);
        }

        await ErrorToast.Success("Execute Success", $"The power command has been executed after {delaySeconds} seconds.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes the specified power command after an optional delay.
    /// </summary>
    /// <param name="delaySecondsInt"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static async Task<Task> ExecuteCommand(int delaySecondsInt, string command)
    {
        if (delaySecondsInt > 0)
        {
            await Task.Delay(delaySecondsInt * 1000);
        }

        return command switch
        {
            "poweroff" => Task.Run(PowerOff),
            "reboot" => Task.Run(Reboot),
            "suspend" => Task.Run(Suspend),
            "hibernate" => Task.Run(Hibernate),
            _ => throw new InvalidOperationException($"The command '{command}' is not implemented.")
        };
    }

    #region functions to execute power actions
    private static void PowerOff() =>
        System.Diagnostics.Process.Start("systemctl", "poweroff");

    private static void Reboot() =>
        System.Diagnostics.Process.Start("systemctl", "reboot");

    private static void Suspend() =>
        System.Diagnostics.Process.Start("systemctl", "suspend");

    private static void Hibernate() =>
        System.Diagnostics.Process.Start("systemctl", "hibernate");
    #endregion
}
