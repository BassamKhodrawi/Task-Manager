using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Task_Manager
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<ProcessInfo> Processes { get; set; } = new ObservableCollection<ProcessInfo>();
        private readonly DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadProcesses();
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (s, e) => LoadProcesses();
            timer.Start();
        }

        private void LoadProcesses()
        {
            var currentProcesses = Process.GetProcesses().Select(p =>
            {
                try
                {
                    return new ProcessInfo
                    {
                        Id = p.Id,
                        Name = p.ProcessName,
                        MemoryUsage = p.PrivateMemorySize64 / 1024 / 1024 + " MB"
                    };
                }
                catch
                {
                    return null;
                }
            }).Where(p => p != null).ToList();

            Processes.Clear();
            foreach (var proc in currentProcesses)
            {
                Processes.Add(proc);
            }
        }

        private void KillProcess(object sender, RoutedEventArgs e)
        {
            if (ProcessList.SelectedItem is ProcessInfo selectedProcess)
            {
                try
                {
                    Process.GetProcessById(selectedProcess.Id).Kill();
                    LoadProcesses();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fehler beim Beenden des Prozesses: " + ex.Message);
                }
            }
        }
    }

    public class ProcessInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MemoryUsage { get; set; }
    }
}