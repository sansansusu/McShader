using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _06_坦克大战
{
    enum GameState
    {
        Running,
        GameOver
    }
    class GameFramework
    {
        public static Graphics g;//每一帧图片都是根据GameFramwork里的g进行绘制
        public static GameState gameState = GameState.Running;
        public static void Start() //Start方法,在游戏开始时被调用一次。
        {

            SoundMananger.InitSound();
            GameObjectManager.Start();
            GameObjectManager.CreateMap();
            GameObjectManager.CreatyTank();
            SoundMananger.PlayStart();
        }

        public static void Updata()
        { 
            //Updata方法，因为游戏会不断刷新变换，所以Updata会被不断调用调用次数也就是FPS。
            //FPS
            /*GameObjectManager.DrawMap();
            GameObjectManager.DrawMyTank();*/


            if (gameState == GameState.Running)
            {
                GameObjectManager.Updata();
            }
            else if (gameState == GameState.GameOver)
            {
                GameOverUpdata();
            }
        }
        public static void GameOverUpdata() 
        {
            int x = 450 / 2 - Properties.Resources.GameOver.Width / 2;
            int y = 450 / 2 - Properties.Resources.GameOver.Height / 2;
            g.DrawImage(Properties.Resources.GameOver, x, y);
        }
        public static void ChangeToGameOver()
        {
            gameState = GameState.GameOver;
        }  
        
        
    }
}
