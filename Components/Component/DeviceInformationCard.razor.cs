using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;
using System.Diagnostics.CodeAnalysis;

namespace WebDean.Components.Component;

public partial class DeviceInformationCard : ComponentBase
{
    // last updated time
    private string? lastUpdatedTime = null;
    private string? name = null;

    /// <summary>
    /// 
    /// </summary>
    protected override void OnInitialized()
    {
        #region initialize last updated time from sqlite database
        InitializeLastUpdatedTime();
        #endregion

        #region get name as userName@machineName
        name = Environment.UserName + "@" + Environment.MachineName;
        #endregion
    }

    /// <summary>
    /// Initialize last updated time from sqlite database
    /// </summary>
    private void InitializeLastUpdatedTime()
    {
        using var connection = new SqliteConnection("Data Source=system_monitor.db");
        connection.Open();

        // if table last_updated exists, read the last updated time
        var command = connection.CreateCommand();
        command.CommandText = @"SELECT name FROM sqlite_master WHERE type='table' AND name='last_updated';";
        var result = command.ExecuteScalar();
        // table last_updated exists
        if (result != null)
        {
            command.CommandText = @"SELECT updated_time FROM last_updated WHERE name='device_infomation' LIMIT 1;";
            var timeResult = command.ExecuteScalar();
            if (timeResult != null)
            {
                lastUpdatedTime = timeResult.ToString();
            }
        }
        // if table last_updated does not exist, create it
        else
        {
            command.CommandText = @"CREATE TABLE last_updated (id INTEGER PRIMARY KEY AUTOINCREMENT, updated_time TEXT NOT NULL, name TEXT NOT NULL);";
            command.ExecuteNonQuery();
            command.CommandText = @"INSERT INTO last_updated (updated_time, name) VALUES ($updated_time, $name);";
            command.Parameters.AddWithValue("$updated_time", "Never");
            command.Parameters.AddWithValue("$name", "device_infomation");
            command.ExecuteNonQuery();
            lastUpdatedTime = "Never";
        }
        connection.Close();
    }

    /// <summary>
    /// Get machine information by running "fastfetch" command
    /// </summary>
    /// <returns></returns>
    public List<string> GetMachineInfo()
    {
        #region run command and get output
        // run shell command "fastfetch" and get output
        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "fastfetch";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        #endregion

        #region process output
        List<string> MachineInfoList = [.. output.Split("[31C")];

        // first line is ascii art, last two lines are color reset codes, remove them
        MachineInfoList.RemoveAt(0);
        MachineInfoList.RemoveAt(MachineInfoList.Count - 1);
        MachineInfoList.RemoveAt(MachineInfoList.Count - 1);

        // remove each line's utf-16 code "\n\u001b" and trim whitespace
        MachineInfoList = [.. MachineInfoList.Select(item => item.Replace("\n\u001b", "").Trim())];
        MachineInfoList.RemoveAll(string.IsNullOrWhiteSpace);
        #endregion

        #region update last updated time in sqlite database
        if (lastUpdatedTime != null)
        {
            using var connection = new SqliteConnection("Data Source=system_monitor.db");

            // make sure the time format is same as sqlite datetime format
            lastUpdatedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // update database
            connection.Open();
            var command = connection.CreateCommand();

            command.CommandText = @"UPDATE last_updated SET updated_time = $updated_time WHERE name = 'device_infomation';";
            command.Parameters.AddWithValue("$updated_time", lastUpdatedTime);
            command.ExecuteNonQuery();

            connection.Close();
        }
        #endregion

        return MachineInfoList;
    }
}
