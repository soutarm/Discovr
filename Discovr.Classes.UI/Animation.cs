using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Discovr.Classes.UI
{
    public class Animation
    {
        public bool SettingsOpen;
        public bool LayersOpen;

        public Animation(bool settingsOpen, bool layersOpen)
        {
            SettingsOpen = settingsOpen;
            LayersOpen = layersOpen;
        }


        public void SlidePanel(Grid grid, bool isSettings)
        {
            var storyboard = new Storyboard();

            grid.RenderTransform = new CompositeTransform();

            double from;
            double to;

            if (isSettings)
            {
                to = SettingsOpen ? -400.0 : 0.0;
                from = SettingsOpen ? 0.0 : -400.0;
            }
            else
            {
                to = LayersOpen ? -400.0 : -800.0;
                from = LayersOpen ? -800.0 : -400.0;
            }

            var easing = new ExponentialEase { EasingMode = EasingMode.EaseInOut };

            // create the timeline
            var animation = new DoubleAnimationUsingKeyFrames();
            animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.Zero, Value = from });
            animation.KeyFrames.Add(new EasingDoubleKeyFrame { KeyTime = TimeSpan.FromMilliseconds(500), Value = to, EasingFunction = easing });
            animation.AutoReverse = false;
            animation.RepeatBehavior = new RepeatBehavior(1);

            // notice the first parameter takes a timeline object not the storyboard itself
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateX)"));
            Storyboard.SetTarget(animation, grid);

            storyboard.Children.Add(animation);

            // start the animation
            storyboard.Begin();

            if (isSettings)
            {
                SettingsOpen = !SettingsOpen;
            }
            else
            {
                LayersOpen = !LayersOpen;
            }
        }

    }
}
