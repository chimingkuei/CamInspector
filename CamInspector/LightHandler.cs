using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;

namespace CamInspector
{
    enum LightColor
    {
        Off, Red, Green
    }

    class LightHandler
    {
        public void TwoLights(WrapPanel lightPanel, string machineName, LightColor initialColor)
        {
            // 外框
            var frame = new Border
            {
                Width = 120,
                Height = 250,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(10),
                Background = new SolidColorBrush(Color.FromRgb(85, 85, 85)) // 深灰色
            };
            // 垂直排列內容
            var stack = new WrapPanel
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            // 紅燈
            var redLight = new Ellipse
            {
                Width = 60,
                Height = 60,
                Fill = initialColor == LightColor.Red ? new SolidColorBrush(Color.FromRgb(255, 77, 77)) : new SolidColorBrush(Color.FromRgb(170, 170, 170)), // 紅/灰
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Margin = new Thickness(10)
            };
            // 綠燈
            var greenLight = new Ellipse
            {
                Width = 60,
                Height = 60,
                Fill = initialColor == LightColor.Green ? new SolidColorBrush(Color.FromRgb(77, 255, 77)) : new SolidColorBrush(Color.FromRgb(170, 170, 170)), // 綠/灰
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Margin = new Thickness(10)
            };
            // TextBlock + 圓角 Border
            var textBorder = new Border
            {
                Width = 80,
                Height = 30,
                CornerRadius = new CornerRadius(8),
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black,
                Background = new SolidColorBrush(Color.FromRgb(204, 255, 204)), // 淺綠色
                Margin = new Thickness(10)
            };
            var textBlock = new TextBlock
            {
                Text = machineName,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.Black
            };
            textBorder.Child = textBlock;
            // 加到 stack
            stack.Children.Add(redLight);
            stack.Children.Add(greenLight);
            stack.Children.Add(textBorder);
            // 放進外框
            frame.Child = stack;
            // 儲存紅綠燈，以便之後單獨控制
            frame.Tag = new Tuple<Ellipse, Ellipse>(redLight, greenLight);
            // 加到主容器
            lightPanel.Children.Add(frame);
        }

        public void SetLight(Border frame, LightColor color)
        {
            if (frame.Tag is Tuple<Ellipse, Ellipse> lights)
            {
                var redLight = lights.Item1;
                var greenLight = lights.Item2;
                switch (color)
                {
                    case LightColor.Off:
                        redLight.Fill = Brushes.Gray;
                        greenLight.Fill = Brushes.Gray;
                        break;
                    case LightColor.Red:
                        redLight.Fill = Brushes.Red;
                        greenLight.Fill = Brushes.Gray;
                        break;
                    case LightColor.Green:
                        redLight.Fill = Brushes.Gray;
                        greenLight.Fill = Brushes.Green;
                        break;
                }
            }
        }

        #region 使用範例
        //light.TwoLights(LightPanel, "CCTV1", LightColor.Off);
        //// 控制第一組燈變成綠燈亮
        //var frame1 = LightPanel.Children[0] as Border;
        //light.SetLight(frame1, LightColor.Green);
        #endregion

    }
}
