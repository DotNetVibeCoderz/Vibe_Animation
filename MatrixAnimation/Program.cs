namespace MatrixAnimation;

static class Program
{
    /// <summary>
    /// Main entry point. Sets high-DPI mode, enables visual styles, then shows
    /// the main window. The application will run as a normal desktop app.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Enable per-monitor DPI awareness for crisp rendering on HiDPI screens.
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) =>
        {
            MessageBox.Show("Unexpected error: " + e.Exception.Message, "Matrix Animation",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        };
        Application.Run(new MainForm());
    }
}
