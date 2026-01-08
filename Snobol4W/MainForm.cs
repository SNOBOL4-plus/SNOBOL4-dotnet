using Snobol4.Common;

namespace Snobol4W;

internal partial class MainForm : Form
{
    #region Members

    public static bool Finished;
    private TextBoxStreamWriter? _streamTextBoxWriter;
    private TextBoxStreamWriter? _streamErrorWriter;

    #endregion

    #region Methods

    internal MainForm()
    {
        InitializeComponent();
        Executive.ReadLineDelegate = InputForm.ReadLine;
    }

    public void WriteLine(string message)
    {
        textConsoleRTF.AppendText(message + Environment.NewLine);
    }

    public void StartForm()
    {
        Application.Run(new Splash());
    }

    private void btnRun_Click(object sender, EventArgs e)
    {
        // Get array of commands and source files
        var commands = new List<string>(
            textCommands.Text.Split(" ",
                StringSplitOptions.RemoveEmptyEntries));

        var files = new List<string>(
            textSelectedFiles.Text.Split(Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries));

        var args = commands.Concat(files);
        Builder builder = new();
        textConsoleRTF.Clear();
        textErrorsRTF.Clear();

        if (files.Count > 0 && string.Equals(Path.GetExtension(files[0]), ".dll", StringComparison.OrdinalIgnoreCase))
        {
            builder.RunDll(Path.GetFullPath(files[0]));
            return;
        }

        // Run compiler
        builder.BuildMain();
    }

    #endregion

    #region Event Handlers

    private void btnSelectFiles_Click(object sender, EventArgs e)
    {
        OpenFileDialog ofn = new()
        {
            Multiselect = true
        };

        if (ofn.ShowDialog() != DialogResult.OK)
            return;

        textSelectedFiles.Lines = textSelectedFiles.Lines.Concat(ofn.FileNames).ToArray();
    }

    internal void MainForm_Load(object sender, EventArgs e)
    {
        _streamTextBoxWriter = new(textConsoleRTF);
        Console.SetOut(_streamTextBoxWriter);
        _streamErrorWriter = new(textErrorsRTF);
        Console.SetError(_streamErrorWriter);
        BringToFront();
        Show();
        var t = new Thread(StartForm);
        t.Start();

        // Force loading of CSharp Compiler assembly
        lock (Program.FinishLock)
        {
            Finished = true;
        }
    }

    private void MainForm_Layout(object sender, LayoutEventArgs e)
    {
        var control = (Control)sender;
        tabControl1.Size = new Size(control.Size.Width - (tabControl1.Location.X + 25),
            control.Size.Height - (tabControl1.Location.Y + 50));
    }

    #endregion

    private void btnClearFiles_Click(object sender, EventArgs e)
    {
        textSelectedFiles.Text = "";
    }

    private void tabControl1_MouseDown(object sender, MouseEventArgs e)
    {
    }
}