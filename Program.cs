using System;
using System.Windows.Forms;
using HardwareKiller.Views;
using HardwareKiller.Controllers;


namespace HardwareKiller
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            var controller = new GameController();
            var form = new MainForm(controller);
            Application.Run(form);
        }
    }
}