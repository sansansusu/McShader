using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _06_坦克大战
{
    public partial class Form1 : Form
    {
        private Thread t;//线程变量
        private static Bitmap tempBmp; //临时的图片用来覆盖上一帧
        private static Graphics windowG;//画布变量
        public Form1()
        {
            InitializeComponent();
            
            this.StartPosition = FormStartPosition.CenterScreen;//设置窗口位置为中间

            windowG = this.CreateGraphics();//取得画布

            tempBmp = new Bitmap(450, 450);//创建一个图片450，450的
            Graphics bmpG = Graphics.FromImage(tempBmp);//然后把图片画到画布上
            GameFramework.g = bmpG;//最后再把完整的一帧的画面涂到窗体上
            
            t = new Thread(new ThreadStart(GameMainThread));//启动一个线程GameMainThread
            t.Start();
        
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private static void GameMainThread() { //新线程

            //GameFramwork 游戏框架
            GameFramework.Start();

            int sleepTime = 1000 / 60;

            while (true)
            {
                GameFramework.g.Clear(Color.Black);//Clear是用来清空画布按照某个颜色

                GameFramework.Updata();//Updata是对游戏每一帧的绘制

                windowG.DrawImage(tempBmp, 0, 0);//从0，0开始加载tempBmp图片

                Thread.Sleep(sleepTime);//每过sleepTime+运行代码时间休息SleepTime这么久
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            t.Abort();//中止
        }

        //事件 消息 
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            GameObjectManager.KeyDown(e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            GameObjectManager.KeyUp(e);
        }
    }
}
