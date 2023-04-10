using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_坦克大战
{
    enum Direction { 
        Up=0,
        Down=1,
        Left=2,
        Right=3
    }
    class Movething : GameObject//坦克移动是上下左右移动所以要四个
    {

        private Object _lock = new object();
        public Bitmap BitmapUp { get; set; }
        public Bitmap BitmapDown { get; set; }
        public Bitmap BitmapLeft { get; set; }
        public Bitmap BitmapRight { get; set; }

        private Direction dir;
        public int Speed { get; set; }//速度每一帧的移动距离
        public Direction Dir { get { return dir; }
            set {
                dir = value;
                Bitmap bmp = null;

                switch (dir) //现根据dir获取当前图片是哪一个然后根据图片
                {
                    case Direction.Up:
                        bmp = BitmapUp;
                        break;
                    case Direction.Down:
                        bmp = BitmapDown;
                        break;
                    case Direction.Left:
                        bmp = BitmapLeft;
                        break;
                    case Direction.Right:
                        bmp = BitmapRight;
                        break;
                }
                lock (_lock)
                {
                    Width = bmp.Width;
                    Height = bmp.Height;
                }

            }
        }//由这个来确定方向用那张图片

        protected override Image GetImage()
        {//返回上下左右给GameObject
            Bitmap bitmap = null;
            switch (Dir) {
                case Direction.Up:
                    bitmap = BitmapUp;
                    break;
                case Direction.Down:
                    bitmap = BitmapDown;
                    break;
                case Direction.Left:
                    bitmap = BitmapLeft;
                    break;
                case Direction.Right:
                    bitmap = BitmapRight; 
                    break;
            }
            bitmap.MakeTransparent(Color.Black);


            return bitmap;//返回一个透明背景的图片

        }
        public override void DrawSelf()
        {
            lock (_lock)
            {
                base.DrawSelf();
            }
        }

        
    }
}
