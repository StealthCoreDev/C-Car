namespace StopCarApp
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // برای فعال‌سازی استایل‌های جدید ویندوز
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
