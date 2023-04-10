using _06_坦克大战.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _06_坦克大战
{

    class GameObjectManager
    {
        private static List<NotMovething> wallList = new List<NotMovething>();
        private static List<NotMovething> steelList = new List<NotMovething>();
        private static List<NotMovething> wallHalfbrickList = new List<NotMovething>();
        private static List<NotMovething> steelHalfbrickList = new List<NotMovething>();

        private static List<EnemyTank> tankList = new List<EnemyTank>();

        private static List<Bullet> bulletList = new List<Bullet>();//子弹
        private static Object _bulletList = new Object();

        private static NotMovething boos = null;
        private static MyTank myTank;

        private static List<Explosion> expList = new List<Explosion>();

        private static Point[] points = new Point[3];//敌人生成地点位置
        private static int enemyBornSpeed = 120;//多少帧生成一个敌人
        private static int enemyBornCount = 60;//用来控制敌人数量计数器

        public static void Start()
        {
            //三个敌人生成的位置
            points[0].X = 0; points[0].Y = 0;
            points[1].X = 7*30; points[1].Y = 0;
            points[2].X = 14*30; points[2].Y = 0;
        }

        public static void Updata()
        {
            foreach (NotMovething nm in wallList)
            {
                nm.Updata();
            }

            foreach (NotMovething nm in steelList)
            {
                nm.Updata();
            }

            foreach (NotMovething nm in wallHalfbrickList)
            {
                nm.Updata();
            }

            foreach (NotMovething nm in steelHalfbrickList)
            {
                nm.Updata();
            }

            foreach (EnemyTank tank in tankList)
            {
                tank.Updata();
            }

            CheckAndDestoryBullet();

            for(int i=0; i < bulletList.Count; i++) 
            {

                bulletList[i].Updata();

            }

            foreach (Explosion exp in expList) 
            {
                exp.Updata();
            }
            CheckAndDestoryExplosion();

            boos.DrawSelf();

            myTank.Updata();

            EnemyBorn();
        }

        private static void CheckAndDestoryBullet()
        {
            List<Bullet> needToDistroy = new List<Bullet>();
            foreach (Bullet bullet in bulletList)
            {
                if (bullet.IsDestory == true)
                {
                    needToDistroy.Add(bullet);//把要销毁的子弹放到新集合里集中销毁，为了不调用子弹集合时同时要销毁子弹集合中的东西冲突
                }
            }
            foreach (Bullet bullet in needToDistroy)
            {
                bulletList.Remove(bullet);
            }
        }
        private static void CheckAndDestoryExplosion()//销毁爆炸
        {
            List<Explosion> needToDestroy = new List<Explosion>();
            foreach (Explosion exp in expList)
            {
                if (exp.IsNeedDestroy == true)
                {
                    needToDestroy.Add(exp);
                }
            }
            foreach (Explosion exp in needToDestroy)
            {
                expList.Remove(exp);
            }
        }

        public static void CreateExplosion(int x, int y)
        {
            Explosion exp = new Explosion(x, y);
            expList.Add(exp);
        }


        public static void CreateBullet(int x, int y, Tag tag, Direction dir)
        {
            Bullet bullet = new Bullet(x, y, 8, dir, tag);
                  bulletList.Add(bullet);
            
        }

        public static void DestroyWall(NotMovething wall)
        {
            wallList.Remove(wall);
            wallHalfbrickList.Remove(wall);
        }

        public static void DestroyTank(EnemyTank tank)
        {
            tankList.Remove(tank);
        }

        public static bool IsCollidedBoss(Rectangle rt)
        {
            return boos.GetRectangle().IntersectsWith(rt);
        }

        public static MyTank IsCollidedMyTank(Rectangle rt)
        {
            if (myTank.GetRectangle().IntersectsWith(rt))
                return myTank;
            else return null;
        }

        public static EnemyTank IsCollidedEnemyTank(Rectangle rt)
        {
            foreach (EnemyTank tank in tankList) 
            {
                if (tank.GetRectangle().IntersectsWith(rt))
                {
                    return tank;
                }
            }
            return null;
        }

        private static void EnemyBorn() 
        {
            enemyBornCount++;
            
            if (enemyBornCount < enemyBornSpeed) return;

            SoundMananger.PlayAdd();
            //随机生成敌人0-2
            Random rd = new Random();
            int index = rd.Next(0, 3);
            Point postion = points[index];
            int enemyType = rd.Next(1, 5);//Next不包含最大值
            switch (enemyType)
            {
                case 1:
                    CreatEnemyTank1(postion.X,postion.Y);
                    break;
                case 2:
                    CreatEnemyTank2(postion.X, postion.Y);
                    break;
                case 3:
                    CreatEnemyTank3(postion.X, postion.Y);
                    break;
                case 4:
                    CreatEnemyTank4(postion.X, postion.Y);
                    break;
            }
            
            enemyBornCount = 0;
        }

        private static void CreatEnemyTank1(int x, int y) 
        {
            EnemyTank tank = new EnemyTank(x, y, 2, Resources.GrayUp, Resources.GrayDown, Resources.GrayLeft, Resources.GrayRight);
            tankList.Add(tank);
        }
        private static void CreatEnemyTank2(int x, int y)
        {
            EnemyTank tank = new EnemyTank(x, y, 2, Resources.GreenUp, Resources.GreenDown, Resources.GreenLeft, Resources.GreenRight);
            tankList.Add(tank);
        }
        private static void CreatEnemyTank3(int x, int y)
        {
            EnemyTank tank = new EnemyTank(x, y, 4, Resources.QuickUp, Resources.QuickDown, Resources.QuickLeft, Resources.QuickRight);
            tankList.Add(tank);
        }
        private static void CreatEnemyTank4(int x, int y)
        {
            EnemyTank tank = new EnemyTank(x, y, 1, Resources.SlowUp, Resources.SlowDown, Resources.SlowLeft, Resources.SlowRight);
            tankList.Add(tank);
        }

        public static NotMovething IsCoolidedWall(Rectangle rt)
        {
            
            foreach (NotMovething wall in wallList)
            {
                if (wall.GetRectangle().IntersectsWith(rt))
                {
                    return wall;
                }
            }
            
            foreach (NotMovething wall in wallHalfbrickList)
            {
                if (wall.GetRectangle().IntersectsWith(rt))
                {
                    return wall;
                }
            }

            return null;

        }
        public static NotMovething IsCoolidedSteel(Rectangle st)
        { 
            foreach (NotMovething steel in steelList)
            {
                if (steel.GetRectangle().IntersectsWith(st))
                {
                    return steel;
                }
            }
            foreach (NotMovething steel in steelHalfbrickList)
            {
                if (steel.GetRectangle().IntersectsWith(st))
                {
                    return steel;
                }
            }

            return null;
        }

        public static bool IsCoolidedBoos(Rectangle rt)
        {
            return boos.GetRectangle().IntersectsWith(rt);
        }

        /* public static void DrawMap() //绘制出来
         {
             foreach(NotMovething nm in wallList)
             {
                 nm.DrawSelf();
             }
             foreach (NotMovething nm in steelList)
             {
                 nm.DrawSelf();
             }
             foreach (NotMovething nm in halfbrickList)
             {
                 nm.DrawSelf();
             }
             boos.DrawSelf();

         }

         public static void DrawMyTank()
         {
             myTank.DrawSelf();
         }*/

        public static void CreatyTank()
        {
            int x = 11 * 15;
            int y = 14 * 30;

            myTank = new MyTank(x, y, 4);

        }
        
        public static void CreateMap() 
        {
            CreateWall(1, 1, 5, Resources.wall, wallList);
            CreateWall(3, 1, 5, Resources.wall, wallList);
            CreateWall(5, 1, 4, Resources.wall, wallList);
            CreateWall(7, 1, 2, Resources.wall, wallList);
            CreateWall(9, 1, 4, Resources.wall, wallList);
            CreateWall(11, 1, 5, Resources.wall, wallList);
            CreateWall(13, 1, 5, Resources.wall, wallList);

            CreateWall(7, 3, 1, Resources.steel, steelList);
            CreateWall(7, 4, 1, Resources.wall, wallList);

            CreateWall(5, 6, 1, Resources.wall, wallList);
            CreateWall(7, 6, 1, Resources.wall, wallList);
            CreateWall(9, 6, 1, Resources.wall, wallList);

            CreateWall(2, 7, 1, Resources.wall, wallList);
            CreateWall(3, 7, 1, Resources.wall, wallList);
            CreateWall(11, 7, 1, Resources.wall, wallList);
            CreateWall(12, 7, 1, Resources.wall, wallList);

            CreateWall(6, 8, 4, Resources.wall, wallList);
            CreateWall(8, 8, 4, Resources.wall, wallList);
            CreateWall(7, 9, 1, Resources.wall, wallList);

            CreateWall(1, 9, 5, Resources.wall, wallList);
            CreateWall(3, 9, 5, Resources.wall, wallList);
            CreateWall(11, 9, 5, Resources.wall, wallList);
            CreateWall(13, 9, 5, Resources.wall, wallList);

            CreateBoos(7, 14, Resources.Boss);

            CreateVerticalHalfBrick(13, 27, 3, Resources.wall, wallHalfbrickList);
            CreateHorizontalHalfBrick(14, 27, 3, Resources.wall, wallHalfbrickList);
            CreateVerticalHalfBrick(16, 28, 2, Resources.wall, wallHalfbrickList);

            CreateHorizontalHalfBrick(0, 15, 2, Resources.steel, steelHalfbrickList);
            CreateHorizontalHalfBrick(28, 15, 2, Resources.steel, steelHalfbrickList);


        }
        public static void CreateWall(int x, int y, int count, Image img, List<NotMovething> wallList)//创建墙
        {

            int xPosition = x * 30;
            int yPosition = y * 30;
            for (int i = yPosition; i < yPosition + count * 30; i += 15)
            {
                NotMovething wall1 = new NotMovething(xPosition, i, img);
                NotMovething wall2 = new NotMovething(xPosition+15, i, img);
                wallList.Add(wall1);
                wallList.Add(wall2);
            }

        }

        public static void CreateBoos(int x, int y, Image img)
        {
            int xPosition = x * 30;
            int yPosition = y * 30;
            boos = new NotMovething(xPosition, yPosition, img);
            
        }
        public static void CreateVerticalHalfBrick(int x, int y,int count, Image img, List<NotMovething> halfbrickList)
        {
            int xPosition = x * 15;
            int yPosition = y * 15;

            for (int i = yPosition; i < yPosition + count * 15; i += 15)
            {
                NotMovething halfbrick = new NotMovething(xPosition, i, img);
                halfbrickList.Add(halfbrick);
            }
        }
        public static void CreateHorizontalHalfBrick(int x, int y, int count, Image img, List<NotMovething> halfbrickList)
        {
            int xPosition = x * 15;
            int yPosition = y * 15;

            for (int i = xPosition; i < xPosition + count * 15; i += 15)
            {
                NotMovething halfbrick = new NotMovething(i, yPosition, img);
                halfbrickList.Add(halfbrick);
            }
        }

        public static void KeyDown(KeyEventArgs args)
        {
            myTank.KeyDown(args);
        }
        public static void KeyUp(KeyEventArgs args)
        {
            myTank.KeyUp(args);
        }
    }
}
