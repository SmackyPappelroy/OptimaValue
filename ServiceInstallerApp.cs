using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Windows.Forms;

namespace OptimaValue;

public partial class ServiceInstallerAppForm : Form
{
    public static readonly string _serviceName = "OptimaValue.Service";
    public static string serviceText => ServiceExists() ? $"Service {_serviceName} finns" : $"Service {_serviceName} finns ej";
    public ServiceInstallerAppForm()
    {
        InitializeComponent();
        Load += ServiceInstallerAppForm_Load;
    }

    private void ServiceInstallerAppForm_Load(object sender, EventArgs e)
    {
        txtServiceExist.Text= ServiceExists() ? "Service finns" : "Service finns ej";
    }

    private void buttonInstallService_Click(object sender, EventArgs e)
    {
        string serviceExePath = Application.ExecutablePath; // Hittar sökvägen till den körande applikationen

        if (!ServiceExists())
        {
            InstallService(serviceExePath, _serviceName);
        }
        else
        {
            MessageBox.Show("Tjänsten är redan installerad.");
        }
    }

    private void buttonUninstallService_Click(object sender, EventArgs e)
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
                MessageBox.Show("Tjänsten installerades framgångsrikt.");
                txtServiceExist.Text = serviceText;
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
                MessageBox.Show("Tjänsten togs bort framgångsrikt.");
                txtServiceExist.Text = serviceText;
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
