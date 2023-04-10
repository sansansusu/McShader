using _06_坦克大战.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_坦克大战
{
    class Explosion : GameObject //绘制爆炸动画
    {
        public bool IsNeedDestroy { get; set; }
        private int explosionSpeed = 1;//爆炸速度也就是每张照片停留时间
        private int explosionCount = 0;//计数器,由计数器来决定播放数组里的那一帧
        private int index = 0;
        private Bitmap[] bmpArray = new Bitmap[] {
            Resources.EXP1,
            Resources.EXP2,
            Resources.EXP3,
            Resources.EXP4,
            Resources.EXP5
        };
        public Explosion(int x, int y)
        {
            foreach (Bitmap bmp in bmpArray)
            {
                bmp.MakeTransparent(Color.Black);//使背景透明
            }
            //在中心爆炸
            this.X = x - bmpArray[0].Width / 2;
            this.Y = y - bmpArray[0].Height / 2;
            IsNeedDestroy = false;
        }
        protected override Image GetImage()
        {
            if (index > 4) return bmpArray[4];
            return bmpArray[index];
        }
        public override void Updata()
        {   
            explosionCount++;
            index = (explosionCount-1) / explosionSpeed;
            if (index > 4) IsNeedDestroy = true;
            base.Updata();
        }

        public override void DrawSelf()
        {
            base.DrawSelf();
        }

    }
}
