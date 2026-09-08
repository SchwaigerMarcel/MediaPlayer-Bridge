using System.Diagnostics;

namespace MediaPlayerBridge;

internal sealed class SetupForm : Form
{
    public SetupForm()
    {
        Text = "MediaPlayerBridge Setup";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(390, 280);

        var heading = new Label
        {
            Text = "MediaPlayerBridge",
            Font = new Font(Font, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(24, 22)
        };

        var description = new Label
        {
            Text = "Set up piXel VISUALIZER, startup, and the background bridge.",
            AutoSize = true,
            Location = new Point(24, 52)
        };

        var setupAll = CreateButton("Set up everything", 24, 88, (_, _) => RunSetupAll());
        var repairPixel = CreateButton("Install or repair piXel support", 24, 126, (_, _) => RunAction(RainmeterInstaller.InstallPixel, "piXel support is ready."));
        var enableStartup = CreateButton("Enable startup", 24, 164, (_, _) => RunAction(Startup.Install, "Startup is enabled."));
        var disableStartup = CreateButton("Disable startup", 24, 202, (_, _) => RunAction(Startup.Remove, "Startup is disabled."));
        var uninstall = CreateButton("Uninstall", 210, 202, (_, _) => Uninstall());

        Controls.AddRange([heading, description, setupAll, repairPixel, enableStartup, disableStartup, uninstall]);
    }

    private Button CreateButton(string text, int x, int y, EventHandler onClick) => new()
    {
        Text = text,
        Location = new Point(x, y),
        Size = new Size(165, 30),
        UseVisualStyleBackColor = true,
        TabStop = true
    }.Also(button =>
    {
        button.Click += onClick;
    });

    private void RunSetupAll()
    {
        try
        {
            RainmeterInstaller.InstallPixel();
            Startup.Install();
            StartBridge();
            MessageBox.Show("Setup is complete. Refresh the piXel skin in Rainmeter.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private void RunAction(Action action, string successMessage)
    {
        try
        {
            action();
            MessageBox.Show(successMessage, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            ShowError(exception);
        }
    }

    private void Uninstall()
    {
        var choice = MessageBox.Show(
            "Remove piXel support, startup, and local bridge data?",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (choice != DialogResult.Yes)
        {
            return;
        }

        RunAction(Installation.Uninstall, "Uninstall is complete. You can now delete this EXE.");
    }

    private static void StartBridge()
    {
        var executable = Environment.ProcessPath
            ?? throw new InvalidOperationException("The executable path could not be determined.");

        Process.Start(new ProcessStartInfo(executable, "--background")
        {
            UseShellExecute = true
        });
    }

    private void ShowError(Exception exception) => MessageBox.Show(
        exception.Message,
        Text,
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
}

internal static class ButtonExtensions
{
    public static T Also<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }
}
