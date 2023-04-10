using _06_坦克大战.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_坦克大战
{
    enum Tag
    {
        MyTank,
        EnemyTank
    }

    class Bullet : Movething
    {
        public Tag Tag { get; set; }

        public bool IsDestory { get; set; }

        public Bullet(int x, int y, int speed, Direction dir, Tag tag)
        {
            IsDestory = false;
            this.X = x;
            this.Y = y;
            this.Speed = speed;
            BitmapUp = Resources.BulletUp;
            BitmapDown = Resources.BulletDown;
            BitmapLeft = Resources.BulletLeft;
            BitmapRight = Resources.BulletRight;
            this.Dir = dir;//子弹初始位置
            this.Tag = tag;//谁的子弹

            //this.X -= ;
            //this.Y += Height /2;

        }

        public override void DrawSelf()
        {
            base.DrawSelf();
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
                if (Y + Height / 2 + 3 < 0)
                {
                    IsDestory=true; return;
                }
            }
            else if (Dir == Direction.Down)
            {
                if (Y + Height / 2 - 3 > 450)//下边界
                {
                    IsDestory = true; return;
                }
            }
            else if (Dir == Direction.Left)
            {
                if (X - Width / 2 + 3 < 0)
                {
                    IsDestory = true; return;
                }
            }
            else if (Dir == Direction.Right)
            {
                if (X + Width / 2 - 3 > 450)
                {
                    IsDestory = true; return;
                }
            }

            #endregion

            //判断是否与其他元素有碰撞

            Rectangle rect = GetRectangle();

            //子弹的左上角
            rect.X = X + Width / 2 - 3;
            rect.Y = Y + Height / 2 - 3;
            rect.Height = 3;
            rect.Width = 3;

            //检测和墙，钢墙，不同的坦克
            //MyTank的只需要检测墙和敌人，EnemyTank检测墙和我的坦克

            //爆炸的中心点
            int xExplosion = this.X + Width / 2;
            int yExplosion = this.Y + Height / 2;

            NotMovething wall = null;
            if ((wall = GameObjectManager.IsCoolidedWall(rect)) != null)
            {
                IsDestory = true;
                GameObjectManager.DestroyWall(wall);
                GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                SoundMananger.PlayBlast();
                return;
            }

            if (GameObjectManager.IsCoolidedSteel(rect) != null)
            {
                IsDestory = true;
                GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                SoundMananger.PlayBlast();
                return;
            }
            if (Tag == Tag.MyTank)
            {
                EnemyTank tank = null;
                if((tank = GameObjectManager.IsCollidedEnemyTank(rect)) != null)
                {
                    IsDestory = true;
                    GameObjectManager.DestroyTank(tank);
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                    SoundMananger.PlayHit();
                    return;
                }
            }
            else if(Tag == Tag.EnemyTank)
            {
                MyTank mytank = null;
                if ((mytank = GameObjectManager.IsCollidedMyTank(rect)) != null)
                {
                    IsDestory = true;
                    GameObjectManager.CreateExplosion(xExplosion, yExplosion);
                    SoundMananger.PlayBlast();
                    mytank.TankDamage();


                    return;
                }
            }
            if (GameObjectManager.IsCoolidedBoos(rect) != false)
            {
                GameFramework.ChangeToGameOver(); return;
            }
        }
            private void ChangeDirection() { }


        
        private void Move()//子弹移动
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
    }
}
