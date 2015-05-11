using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace Discovr.Classes.UI
{
    public static class Extensions
    {
        public static string LayerName(this ToggleSwitch toggleSwitch)
        {
            var switchName = toggleSwitch.Content.ToString();
            return string.Format("Layer_{0}", switchName);
        }


        public static ProgressBar FindProgressBarByName(this StackPanel stackPanel, string switchName)
        {
            var nameToFind = string.Format("Loading{0}", switchName);
            return stackPanel.Children.OfType<ProgressBar>().FirstOrDefault(l => l.Name == nameToFind);
        }

        public static SolidColorBrush GetRandomColor()
        {
            var random = new Random();
            var newColor = Color.FromArgb(255, (byte)random.Next(70, 150), (byte)random.Next(70, 150), (byte)random.Next(70, 150));
            return new SolidColorBrush(newColor);
        }

    }
}
