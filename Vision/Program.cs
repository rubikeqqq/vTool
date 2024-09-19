using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Vision
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Mutex mutex = new Mutex( true , Process.GetCurrentProcess().ProcessName , out var b );

            if( !b )
            {
                MessageBox.Show( "程序已经打开，即将关闭此界面！" );
                Environment.Exit( 0 );
            }

            Application.Run(new FormMain());
        }
    }
}
