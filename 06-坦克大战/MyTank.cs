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
    
    class MyTank:Movething
    {
        public int HP { get; set; }
        public bool IsMoving { get; set; }
        private int originalX;
        private int originalY;
        private Direction originalDir;
        public MyTank(int x, int y, int speed)
        {
            IsMoving = false;
            this.X = x;
            this.Y = y;
            originalX = X;
            originalY = Y;
            this.Speed = speed;
            BitmapUp = Resources.MyTankUp;
            BitmapDown = Resources.MyTankDown;
            BitmapLeft = Resources.MyTankLeft;
            BitmapRight = Resources.MyTankRight;
            this.Dir = Direction.Up;//先设置往下左右用那张图片在设置方位
            originalDir = Direction.Up;
            HP = 3;//血量


        }

        public override void Updata()
        {
            MoveCheck();//移动检测
            Move();
            base.Updata();
        }

        private void MoveCheck()
        {
            #region//判断是否超出窗体
            //先判断方向在根据对应的方向判断是否到达这个方向的最大值
            //最大值是450，当前坦克的位置+移动像素大小+坦克高度（不用宽度因为方向朝向那里面向那里就算高度）
            if (Dir == Direction.Up)
            {
                if (Y - Speed < 0)
                {
                    IsMoving = false;
                    return;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Speed + Height > 450)
                {
                    IsMoving = false;
                    return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X - Speed < 0)
                {
                    IsMoving = false;
                    return;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Speed + Height > 450)
                {
                    IsMoving = false;
                    return;
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
                IsMoving = false;
            }
            else if (GameObjectManager.IsCoolidedSteel(rect) != null)
            {
                IsMoving = false;
            }
            else if (GameObjectManager.IsCoolidedBoos(rect) != false)
            {
                IsMoving = false;
            }

        }

        private void Move()
        {
            if (IsMoving == false) return;
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

        public  void KeyDown(KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.W:
                    Dir = Direction.Up;
                    IsMoving = true;
                    break;
                case Keys.S:
                    Dir = Direction.Down;
                    IsMoving = true;
                    break;
                case Keys.A:
                    Dir = Direction.Left;
                    IsMoving = true;
                    break;
                case Keys.D:
                    Dir = Direction.Right;
                    IsMoving = true;
                    break;
                case Keys.Space:
                    Attack();
                    break;
            }
        }
        private void Attack()//攻击
        {
            SoundMananger.PlayFire();
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
            GameObjectManager.CreateBullet(x, y, Tag.MyTank, Dir);
        }

        public  void KeyUp(KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.W:
                    IsMoving = false;
                    break;
                case Keys.S:
                    IsMoving = false;
                    break;
                case Keys.A:
                    IsMoving = false;
                    break;
                case Keys.D:
                    IsMoving = false;
                    break;

            }
        }
        public void TankDamage()//收到了攻击
        {
            HP--;
            if (HP <= 0)//再原点重生
            {
                X = originalX;
                Y = originalY;
                Dir = originalDir;
                HP = 3;
            }
        }

    }
}
