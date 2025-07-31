using System;
using System.Windows.Forms;

namespace AplikasiPencatatanWarga
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var db = new DatabaseManager(); // Ini akan otomatis membuat DB jika belum ada
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
