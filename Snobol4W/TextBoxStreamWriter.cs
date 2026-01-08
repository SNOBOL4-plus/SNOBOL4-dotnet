namespace Snobol4W;

/// <summary>
/// CLass to allow Console.Error.WriteLine to update a text box
/// https://nmarkou.blogspot.com/2011/12/redirect-console-output-to-textbox.html
/// </summary>
internal class TextBoxStreamWriter : StringWriter
{
    private readonly RichTextBox _textBoxOutput;
    internal StreamWriter Writer;
    internal MemoryStream Mem;

    internal TextBoxStreamWriter(RichTextBox output)
    {
        _textBoxOutput = output;
        Mem = new();
        Writer = new(Mem) { AutoFlush = true };
    }

    public override void Write(char value)
    {
        base.Write(value);
        _textBoxOutput.AppendText(value.ToString());
        Writer.Write(value);
    }

    public override void Write(string? value)
    {
        base.Write(value);
        _textBoxOutput.AppendText(value);
        Writer.Write(value);
    }

    public override void WriteLine(string? value)
    {
        base.Write(value);
        _textBoxOutput.AppendText(value + Environment.NewLine);
        Writer.Write(value);
    }
}
