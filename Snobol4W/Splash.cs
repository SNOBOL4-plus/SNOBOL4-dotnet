namespace Snobol4W;

public partial class Splash : Form
{
    public Splash()
    {
        InitializeComponent();
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        lock (Program.FinishLock)
        {
            Thread.Sleep(3000);
            if (MainForm.Finished)
                Close();
        }
    }

    private void Splash_Load(object sender, EventArgs e)
    {

    }

    private void label1_Click(object sender, EventArgs e)
    {

    }
}