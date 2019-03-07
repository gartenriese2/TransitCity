namespace CityEditor
{
    using System;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            AppBuilder.CreateApp(() => new MainWindowViewModel());
        }
    }
}
