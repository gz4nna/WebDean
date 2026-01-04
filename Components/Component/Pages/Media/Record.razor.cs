using System.Diagnostics;

namespace WebDean.Components.Component.Pages.Media;

public partial class Record
{
    // 录音持续时间
    private int recordDuration = 10;

    // 录音状态标记
    private bool isRecording = false;

    // 保存路径
    private string? folderPath;

    // 录音进程
    private Process? _currentRecordingProcess;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        folderPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "records"
        );

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        // 清理历史文件
        CleanOldRecords();
    }

    private async Task OnRecordButtonClick()
    {
        if (!isRecording) await StartRecording();
        else FinishRecording();
    }

    /// <summary>
    /// 开始录音
    /// </summary>
    private async Task StartRecording()
    {
        // 如果已经有录音进程在运行，则不启动新的录音
        if (_currentRecordingProcess != null && !_currentRecordingProcess.HasExited) return;

        // 开启录音
        isRecording = true;

        // 新建录音文件名
        var fileName = $"rec_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
        var absolutePath = Path.Combine(folderPath!, fileName);

        // 设置命令参数
        var startInfo = new ProcessStartInfo
        {
            FileName = "arecord",
            Arguments = $"-d {recordDuration} -f S16_LE -r 16000 -c 1 -t wav \"{absolutePath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _currentRecordingProcess = Process.Start(startInfo);

        // 设定的录制时间结束后重置状态
        await Task.Delay(TimeSpan.FromSeconds(recordDuration));
        isRecording = false;
    }

    /// <summary>
    /// 终止录音
    /// </summary>
    private void FinishRecording()
    {
        // 如果录音进程已经退出了则不需要执行
        if (_currentRecordingProcess == null || _currentRecordingProcess.HasExited) return;

        // 杀死录音进程
        Process.Start("kill", $"-2 {_currentRecordingProcess.Id}")?.WaitForExit();

        // 如果3秒还没退就强杀
        if (!_currentRecordingProcess.WaitForExit(3000)) _currentRecordingProcess.Kill();

        _currentRecordingProcess.Dispose();
        _currentRecordingProcess = null;

        // 重置状态
        isRecording = false;
    }

    /// <summary>
    /// 删除一星期前的文件
    /// </summary>
    public void CleanOldRecords()
    {
        var dir = new DirectoryInfo("wwwroot/records");
        if (!dir.Exists) return;

        foreach (var file in dir.GetFiles("*.wav"))
        {
            if (file.CreationTime < DateTime.Now.AddDays(-7))
            {
                file.Delete();
            }
        }
    }
}