// Originally contributed by Chinajade
//
// LICENSE:
// This work is licensed under the
//     Creative Commons Attribution-NonCommercial-ShareAlike 4.0 Unported License.
// also known as CC-BY-NC-SA.  To view a copy of this license, visit
//      http://creativecommons.org/licenses/by-nc-sa/4.0/
// or send a letter to
//      Creative Commons // 171 Second Street, Suite 300 // San Francisco, California, 94105, USA.
//

#region Usings

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable InconsistentNaming
#endregion


namespace BuddyControlPanel
{
    public class Image_RoundedCorners : Border
    {
        public Image_RoundedCorners(string imageName, int? specificSize = null, Brush borderColor = null)
        {
            // Please maintain invariant:  cornerRadius - borderThickness = cornerMaskRadius
            var borderThickness = new Thickness(1);
            var cornerRadius = new CornerRadius(8);
            var cornerMaskRadius = new CornerRadius(7);
            borderColor = borderColor ?? Brushes.Transparent;

            BorderBrush = borderColor;
            BorderThickness = borderThickness;
            CornerRadius = cornerRadius;

            var grid = new Grid();
            Child = grid;

            // NB. The Mask needs to be a child of the Grid.  This causes the mask to expand to the
            // full content area without having to explicitly specify sizing information in the inner scope.
            var borderMask = new Border()
            {
                Background = Brushes.White,
                Margin = borderThickness,
                CornerRadius = cornerMaskRadius,
            };
            grid.Children.Add(borderMask);

            // NB: If we dump the image straight into the grid, we get annoying 1-pixel-wide artifacts.
            // By placing the image in a container in the grid, the artifacts are properly masked by the grid.
            var stackPanel = new StackPanel()
            {
                OpacityMask = new VisualBrush(borderMask),
            };
            grid.Children.Add(stackPanel);

            var image = new Image()
            {
                OpacityMask = new VisualBrush(borderMask),
                Source = Utility.ToImageSource(imageName, specificSize),
            };
            stackPanel.Children.Add(image);
        }
    }
}
