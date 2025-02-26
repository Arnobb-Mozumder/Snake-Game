using System.Windows.Media;
using System.Windows.Media.Imaging;
using System;

namespace Snakee
{
    public static class Images
    {
        public static ImageSource Empty = LoadImage("Assets/Empty.png");
        public static ImageSource Body = LoadImage("Assets/Body.png");
        public static ImageSource Food = LoadImage("Assets/Food.png");
        public static ImageSource Head = LoadImage("Assets/Head.png");
        public static ImageSource DeadBody = LoadImage("Assets/DeadBody.png");
        public static ImageSource DeadHead = LoadImage("Assets/DeadHead.png");



        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }
    }
}
