using System.Diagnostics.CodeAnalysis;

namespace WebDean.Components.Component.Pages.Media;

public partial class RecordListView
{
    #region 列表用参数
    [NotNull]
    private string? Value { get; set; }

    [NotNull]
    private List<string>? Items { get; set; }
    #endregion

    // 文件系统监视器
    private FileSystemWatcher? recordFileWatcher;

    // 录音文件存放文件夹位置
    private string? folderPath;

    #region 播放器用参数
    // 播放器资源位置
    private string? _currentUrl;

    // 播放录音文件名称
    private string? _currentFileName;
    #endregion

    protected override void OnInitialized()
    {
        base.OnInitialized();
        folderPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "wwwroot",
        "records"
        );

        // 文件发生变动时更新列表
        // 这里未生效，需要再看一下
        recordFileWatcher = new FileSystemWatcher(folderPath);
        recordFileWatcher.Changed += OnRecordFileChanged;

        // 初始化时手动触发一次
        OnRecordFileChanged(this, new EventArgs());
    }

    /// <summary>
    /// 文件变动时更新列表
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    private void OnRecordFileChanged(object obj, EventArgs args)
    {
        var dir = new DirectoryInfo("wwwroot/records");
        if (!dir.Exists) return;

        // 获取全部录音文件的文件名
        Items = dir.GetFiles("*.wav")
        .Select(x => x.Name)
        .ToList();
    }

    /// <summary>
    /// 点击列表项时播放录音
    /// </summary>
    /// <param name="selectRecordFile"></param>
    /// <returns></returns>
    private Task OnClickItem(string selectRecordFile)
    {
        // 设定好播放器参数后等待自动播放
        _currentFileName = selectRecordFile;
        _currentUrl = $"/records/{_currentFileName}?t={DateTime.Now.Ticks}";

        return Task.CompletedTask;
    }
}