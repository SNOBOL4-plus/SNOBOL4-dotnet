namespace Snobol4W;

public partial class InputForm : Form
{
    public InputForm()
    {
        InitializeComponent();
    }

    public static string? ReadLine()
    {
        var dlg1 = new InputForm();
        dlg1.buttonOK.DialogResult = DialogResult.OK;
        dlg1.buttonEOF.DialogResult = DialogResult.Cancel;
        dlg1.ShowDialog();
        if (dlg1.DialogResult == DialogResult.Cancel)
            return null;
        Console.Error.WriteLine(dlg1.textBox1.Text);
        return dlg1.textBox1.Text;
    }
}

