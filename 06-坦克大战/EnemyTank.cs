using _06_坦克大战.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_坦克大战
{
    class EnemyTank : Movething
    {
        public int changeDirSpeed { get; set; }
        private int changeDirCount = 0;
        public int attackSpeed { get; set; }//坦克自动攻击速度，60帧数一下
        private int attackCount = 0;//攻击计时器
        private  Random r = new Random();
        public EnemyTank(int x, int y, int speed, Bitmap bmpUp, Bitmap bmpDown, Bitmap bmpLift, Bitmap bmpRight)
        {
            this.X = x;
            this.Y = y;
            this.Speed = speed;
            BitmapUp = bmpUp;
            BitmapDown = bmpDown;
            BitmapLeft = bmpLift;
            BitmapRight = bmpRight;
            this.Dir = Direction.Down;//先设置往下左右用那张图片在设置方位
            attackSpeed = 60;
            changeDirSpeed = 100;
        }

        public override void Updata()
        {
            MoveCheck();//移动检测
            Move();
            AttackCheck();
            AutoChangeDirection();

            base.Updata();
        }

        private void MoveCheck()
        {
            #region//判断是否超出窗体碰到东西自动转变方向
            //先判断方向在根据对应的方向判断是否到达这个方向的最大值
            //最大值是450，当前坦克的位置+移动像素大小+坦克高度（不用宽度因为方向朝向那里面向那里就算高度）
            if (Dir == Direction.Up)
            {
                if (Y - Speed < 0)
                {
                    ChangeDirection();return;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Speed + Height > 450)
                {
                    ChangeDirection(); return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X - Speed < 0)
                {
                    ChangeDirection(); return;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Speed + Height > 450)
                {
                    ChangeDirection(); return;
                }
            }
            #endregion

            //判断是否与其他元素有碰撞

            Rectangle rect = GetRectangle();

            switch (Dir)
            {
                case Direction.Up:
                    rect.Y -= Speed;
                    break;
                case Direction.Down:
                    rect.Y += Speed;
                    break;
                case Direction.Left:
                    rect.X -= Speed;
                    break;
                case Direction.Right:
                    rect.X += Speed;
                    break;

            }

            if (GameObjectManager.IsCoolidedWall(rect) != null)
            {
                ChangeDirection(); return;
            }
            if (GameObjectManager.IsCoolidedSteel(rect) != null)
            {
                ChangeDirection(); return;
            }
            if (GameObjectManager.IsCoolidedBoos(rect) != false)
            {
                ChangeDirection(); return;
            }

        }

        private void AutoChangeDirection()
        {
            changeDirCount++;
            if (changeDirCount < changeDirSpeed) return;
                ChangeDirection();
            changeDirCount = 0;
        }

        private void ChangeDirection()//改变朝向
        {
            while (true)//为了获得不同方向
            {
                Direction dir = (Direction)r.Next(0, 4);//0,1,2,3
                if (dir == Dir)
                {
                    continue;
                }
                else 
                {
                    Dir = dir; break;
                }
            }
            MoveCheck();
        }

        private void Move()
        {

            switch (Dir)
            {
                case Direction.Up:
                    Y -= Speed;
                    break;
                case Direction.Down:
                    Y += Speed;
                    break;
                case Direction.Left:
                    X -= Speed;
                    break;
                case Direction.Right:
                    X += Speed;
                    break;

            }

        }

        private void AttackCheck()
        {
            attackCount++;//每一帧加1等帧数超过60就会去调用Attack函数去发射一枚子弹
            if (attackCount < attackSpeed) return;
            Attack();
            attackCount = 0;
        }

        private void Attack()//攻击
        {
            int x = this.X;
            int y = this.Y;
            switch (Dir)
            {
                case Direction.Up:
                    y -= Height;
                    break;
                case Direction.Down:
                    y += Height;
                    break;
                case Direction.Left:
                    x -= Width;
                    break;
                case Direction.Right:
                    x += Width;
                    break;
            }
            GameObjectManager.CreateBullet(x, y, Tag.EnemyTank, Dir);
        }

    }
}
