using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace OptimaValue;

public partial class ServiceInstallerAppForm : Form
{
    public static readonly string _serviceName = "OptimaValue.Service";
    public static string serviceText => ServiceExists() ? $"{_serviceName} är installerad" : $"{_serviceName} EJ installerad";
    private string buttonInstallServiceText => ServiceExists() ? "Avinstallera service" : "Installera service";
    public ServiceInstallerAppForm()
    {
        InitializeComponent();
        Load += ServiceInstallerAppForm_Load;
        buttonInstallService.Click += buttonInstallService_Click;
        UpdateServiceStatus();
    }

    // Metod för att uppdatera statusikonen
    private void UpdateServiceStatus()
    {
        txtServiceExist.Text = serviceText;
        buttonInstallService.Text = buttonInstallServiceText;

        if (ServiceExists())
        {
            statusPictureBox.Image = DrawGreenCircle();
        }
        else
        {
            statusPictureBox.Image = DrawRedCross();
        }
    }

    // Rita en grön cirkel
    private Image DrawGreenCircle()
    {
        int size = 20;
        Bitmap bmp = new Bitmap(size, size);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Transparent);
            using (SolidBrush brush = new SolidBrush(Color.Green))
            {
                g.FillEllipse(brush, 0, 0, size, size);
            }
        }
        return bmp;
    }

    // Rita ett rött kryss
    private Image DrawRedCross()
    {
        int size = 20;
        Bitmap bmp = new Bitmap(size, size);
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.Transparent);
            using (Pen pen = new Pen(Color.Red, 3))
            {
                g.DrawLine(pen, 0, 0, size, size);
                g.DrawLine(pen, 0, size, size, 0);
            }
        }
        return bmp;
    }

    private void buttonInstallService_Click(object sender, EventArgs e)
    {
        if(ServiceExists())
        {
            UninstallServiceClick(this, EventArgs.Empty);
        }
        else
        {
            InstallServiceClick(this,EventArgs.Empty);
        }
    }

    private void ServiceInstallerAppForm_Load(object sender, EventArgs e)
    {
        txtServiceExist.Text = serviceText;
    }

    private void InstallServiceClick(object sender, EventArgs e)
    {
        string serviceExePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "OptimaValue.Service.exe");

        if (!ServiceExists())
        {
            InstallService(serviceExePath, _serviceName);
        }
        else
        {
            MessageBox.Show("Tjänsten är redan installerad.");
        }
    }

    private void UninstallServiceClick(object sender, EventArgs e)
    {

        if (ServiceExists())
        {
            UninstallService(_serviceName);
        }
        else
        {
            MessageBox.Show("Tjänsten finns inte.");
        }
    }

    private void InstallService(string servicePath, string serviceName)
    {
        try
        {
            // Använd sc.exe för att skapa tjänsten
            Process process = new Process();
            process.StartInfo.FileName = "sc.exe";
            process.StartInfo.Arguments = $"create \"{serviceName}\" binPath= \"{servicePath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                // Sätt beskrivningen för tjänsten
                SetServiceDescription(serviceName, "Cyklisk loggning från Siemens-PLC och OPC-server");
                UpdateServiceStatus();
            }
            else
            {
                MessageBox.Show($"Fel vid installation av tjänst: {error}");
                txtServiceExist.Text = serviceText;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ett fel uppstod: {ex.Message}");
            txtServiceExist.Text = serviceText;
        }
    }

    private void SetServiceDescription(string serviceName, string description)
    {
        try
        {
            Process process = new Process();
            process.StartInfo.FileName = "sc.exe";
            process.StartInfo.Arguments = $"description \"{serviceName}\" \"{description}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                MessageBox.Show($"Fel vid inställning av beskrivning: {error}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ett fel uppstod vid inställning av beskrivning: {ex.Message}");
        }
    }

    private void UninstallService(string serviceName)
    {
        try
        {
            // Använd sc.exe för att ta bort tjänsten
            Process process = new Process();
            process.StartInfo.FileName = "sc.exe";
            process.StartInfo.Arguments = $"delete \"{serviceName}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                txtServiceExist.Text = serviceText;
                UpdateServiceStatus();
            }
            else
            {
                MessageBox.Show($"Fel vid borttagning av tjänst: {error}");
                txtServiceExist.Text = serviceText;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ett fel uppstod: {ex.Message}");
            txtServiceExist.Text = serviceText;
        }
    }

    public static bool ServiceExists()
    {
        ServiceController[] services = ServiceController.GetServices();
        foreach (ServiceController service in services)
        {
            if (service.ServiceName.Equals(_serviceName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }
}
