using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace BulletTime
{
    public class MyObject
    {
        #region Property
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public object Texture { get; set; }
        public Color thisClolor { get; set; }
        #endregion

        #region Event
        public bool canCheck { get; set; }
        public bool canBreak { get; set; }
        public bool canAttack { get; set; }
        public bool canShoot { get; set; }

        public event Func<object, bool> CheckEvent;
        public event Action<object> BreakEvent;
        public event Func<object, object, bool> AttackEvent;
        public event Action<object> ShootEvent;
        public event Action<object, object> showHandler;


        public bool OnShoot(object obj)
        {
            if (ShootEvent != null)
            {
                ShootEvent(obj);
                return true;
            }
            return false;
        }

        public bool OnBreak()
        {
            if (BreakEvent != null)
            {
                BreakEvent(this);
                return true;
            }
            return false;
        }

        public bool OnCheck()
        {
            if (CheckEvent != null)
            {
                return CheckEvent(this);
            }
            return false;
        }

        public bool OnAttack(object obj)
        {
            if (AttackEvent != null)
            {
                return AttackEvent(this, obj);
            }
            return false;
        }

        public void Show()
        {
            showHandler.Invoke(Texture, this);
        }

        #endregion

        #region Create

        public MyObject(object tt)
        {
            Texture = tt;
        }

        #endregion

        #region Method

        public virtual void Update(float elapsedTime)
        {

        }

        #endregion
    }

    public class Launchpad : MyObject
    {
        #region Private

        private Vector2 _circle_center;
        private int _circle_radius;
        private int _circle_radiusMax;
        private double _speed;
        private const double _speedMax = 50;
        private double _degree;
        private bool canMove;
        private bool canWait;
        private int _shootCount;
        private const int _shootMax = 3;
        private double _distDegree;
        private double _splitDegree;
        private const int _splitNumber = 30;
        private Random rand;
        private float _timecount;
        private float _timeMax;
        private const int _vector = 12;
        private const int _shootDeg = 360;
        private Action<object, object> _action;
        private double _shootSpeed;
        #endregion

        #region Property

        public double Degree
        {
            get { return _degree; }
            set
            {
                if (_degree != value)
                {
                    _degree = value;
                    X = (float)(_circle_radius * Math.Cos(radians(_degree))) + _circle_center.X - Width/2;
                    Y = (float)(_circle_radius * Math.Sin(radians(_degree))) + _circle_center.Y - Height/2;
                }
            }
        }

        #endregion

        #region Create

        public Launchpad(object tt, Vector2 cc, int cr, int size, Action<object,object> act, int seed) : base(tt)
        {
            rand = new Random(seed);
            _circle_center = cc;
            _circle_radius = cr;
            _circle_radiusMax = cr;
            Width = size;
            Height = size;
            _speed = _speedMax;
            thisClolor = Color.White;
            _action = act;

//            _shootSpeed = Math.Pow(rand.Next(2, 5) * 4,2);
            _shootSpeed = 10;
            _timeMax = 2f;
            _splitDegree = 360 / _splitNumber;
            _distDegree = rand.Next(0, 359);
            Degree = rand.Next(0,359);
            canMove = true;

            canCheck = false;
            canBreak = false;
            canAttack = false;
            canShoot = true;
        }

        #endregion

        #region Method

        private double radians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public override void Update(float elapsedTime)
        {
            if(canMove == true)
            {
                if(_distDegree > Degree)
                {
                    if ((_distDegree - 180) > Degree)
                    {
                        Degree -= _speed * elapsedTime;
                    }
                    else
                    {
                        if (_distDegree > Degree)
                            Degree += _speed * elapsedTime;
                        else
                            Degree -= _speed * elapsedTime;
                    }
                }
                else if (_distDegree < Degree)
                {
                    if ((_distDegree + 180) < Degree)
                    {
                        Degree += _speed * elapsedTime;
                    }
                    else
                    {
                        if (_distDegree > Degree)
                            Degree += _speed * elapsedTime;
                        else
                            Degree -= _speed * elapsedTime;
                    }
                }

                Degree = CalDeg(Degree);
            }
            if (Math.Abs(_distDegree - Degree) <= (_speed))
            {
                _speed *= 0.9;
                _speed = _speed < 10 ? 10 : _speed;
            }
            if ( Math.Abs(_distDegree - Degree) <= 0.1)
            {
                canMove = false;
                Degree = _distDegree;
                _distDegree = _splitDegree * rand.Next(1, _splitNumber);// * (rand.Next()%2==0?-1:1);
                canShoot = true;
            }

            if(canMove == false)
            {
                if(canWait== true)
                {
                    _timecount += elapsedTime;
                    if(_timecount >= _timeMax)
                    {
                        _timecount = 0;
                        canWait = false;
                    }
                }

                if(canShoot == true && canWait == false)
                {
                    ShootBullet();
                    if (_shootCount >= _shootMax)
                    {
                        _shootCount = 0;
                        canShoot = false;
                        canMove = true;
                        _speed = _speedMax;
                       // _circle_radius = rand.Next(_circle_radiusMax / 2, _circle_radiusMax);
                    }
                }
            }
        }

        private void ShootBullet()
        {

            double tempDeg = CalDeg(Degree - 180);
            int dir = 0;
            if (_shootCount % 2 == 0) dir = 1;
            else dir = -1;
            for (int jj = 0; jj < _vector; jj++)
            {
                Bullet temp = new Bullet(Texture, X + Width / 2, Y + Height / 2, Width / 2, (tempDeg + (jj - 1) * (_shootDeg / _vector - 1)), _shootSpeed, dir);
                temp.showHandler += _action;
                OnShoot(temp);
            }
            _shootCount++;
            canWait = true;
        }

        private double CalDeg( double deg)
        {
            double temp;
            if (deg >= 0) temp =  deg;
            else temp =  360 + deg;

            return temp % 360;
        }

        #endregion
    }

    public class Bullet : MyObject
    {
        #region Private

        private double vx;
        private double vy;
        private double _degree;
        private double _speed;
        private double _radius;
        private int _direct;
        #endregion

        #region Property

        public double Degree
        {
            get { return _degree; }
            set
            {
                if (_degree != value)
                {
                    _degree = value;
                    _degree %= 360;
                    vx = Speed * Math.Cos(radians(_degree));
                    vy = Speed * Math.Sin(radians(_degree));
                }
            }
        }

        public double Speed
        {
            get { return _speed; }
            set
            {
                if(_speed != value)
                {
                    _speed = value;
                    vx = Speed * Math.Cos(radians(_degree));
                    vy = Speed * Math.Sin(radians(_degree));
                }
            }
        }

        #endregion


        #region Create
        public Bullet(object tt, float x, float y, int size, double deg, double sp, int dir) : base(tt)
        {
            Width = size;
            Height = size;
            X = x - size / 2;
            Y = y - size / 2;
            thisClolor = Color.Green;
            Degree = deg;
            Speed = sp;

            _direct = dir;

            canCheck = false;
            canBreak = true;
            canAttack = true;
            canShoot = false;
        }
        #endregion

        #region Method
        public override void Update(float elapsedTime)
        {
            Degree += 20 * elapsedTime * _direct;
            _radius += 1 * elapsedTime;
            X += (float)((vx * elapsedTime)* _radius);
            Y += (float)((vy * elapsedTime)* _radius);
            if (X + Width < 0 || X>Game1.window_width ||
                Y + Height <0 || Y>Game1.window_height)
            {
                OnBreak();
            }
        }

        private double radians(double deg)
        {
            return deg * (Math.PI / 180);
        }
        #endregion
    }

    public class MyBall : MyObject
    {
        private float _speed;
        public float Speed
        {
            get { return _speed; }
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    vx = Speed * Math.Cos(radians(_degree));
                    vy = Speed * Math.Sin(radians(_degree));
                }
            }
        }
        private double _degree;
        public double Degree
        {
            get { return _degree; }
            set
            {
                if (_degree != value)
                {
                    _degree = value;
                    vx = Speed * Math.Cos(radians(_degree));
                    vy = Speed * Math.Sin(radians(_degree));
                }
            }
        }

        private double radians(double deg)
        {
            return deg * (Math.PI / 180);
        }

        private double vx;
        private double vy;
        public MyBall(int x, int y, int size, float sp, Color c, object tt) : base(tt)
        {
            X = x;
            Y = y;
            Width = size;
            Height = size;
            thisClolor = c;
            Speed = sp;

            Degree = 45;

            canCheck = true;
            canBreak = false;
            canAttack = false;
        }

        public void Attack(int val)
        {
            switch (val)
            {
                case 1:
                    Degree = -Degree;
                    break;
                case 2:
                    Degree = 180 - Degree;
                    break;
                default:
                    break;
            }
        }

        public void Update(float elapsedTime)
        {
            X += (float)(vx * elapsedTime);
            Y += (float)(vy * elapsedTime);
            OnCheck();
        }
    }

    public class Player : MyObject
    {
        private int BoundL { get; set; }
        private int BoundR { get; set; }
        private int BoundU { get; set; }
        private int BoundD { get; set; }
        private float Speed { get; set; }
        public Player(object tt, int x, int y, int w, int h, Color c, float sp) : base(tt)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            thisClolor = c;
            this.Speed = sp;
            this.BoundL = 0;
            this.BoundR = Game1.window_width;
            this.BoundU = 0;
            this.BoundD = Game1.window_height;

            canCheck = true;
            canBreak = false;
            canAttack = false;
            canShoot = false;
        }

        public void MoveLeft()
        {
            this.X -= (int)this.Speed;
            if (X < BoundL) X = BoundL;
 //           OnCheck();
        }

        public void MoveRight()
        {
            this.X += (int)this.Speed;
            if ((X + Width) > BoundR) X = BoundR - Width;
//            OnCheck();
        }

        public void MoveUP()
        {
            this.Y -= (int)this.Speed;
            if (Y < BoundU) Y = BoundU;
//            OnCheck();
        }

        public void MoveDown()
        {
            this.Y += (int)this.Speed;
            if ((Y + Height) > BoundD) Y = BoundD - Height;
//            OnCheck();
        }
    }

    public class MyWall : MyObject
    {
        public MyWall(int x, int y, int w, int h, Color c, object tt) : base(tt)
        {
            this.X = x;
            Y = y;
            Width = w;
            Height = h;
            thisClolor = c;

            canCheck = false;
            canBreak = false;
            canAttack = true;
        }
    }

    public class MyFont : MyObject
    {
        public String ss { get; set; }
        public readonly int sX;
        public readonly int sY;
        public readonly Color sC;
        public MyFont(int x, int y, string s, Color c, int offset, object tt) : base(tt)
        {
            X = x;
            sX = x + offset;
            Y = y;
            sY = y + offset;
            thisClolor = c;
            sC = new Color((byte)(thisClolor.R * 0.5), (byte)(thisClolor.G * 0.5), (byte)(thisClolor.B * 0.5), (byte)(thisClolor.A * 0.5));
            ss = s;

            canCheck = false;
            canBreak = false;
            canAttack = false;
        }
    }

    public class MyBrick : MyObject
    {
        public MyBrick(int x, int y, int w, int h, Color c, object tt) : base(tt)
        {
            this.X = x;
            Y = y;
            Width = w;
            Height = h;
            thisClolor = c;

            canCheck = false;
            canBreak = true;
            canAttack = true;
        }
    }

    public class MyObjList : ObservableCollection<MyObject>
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    MyObject temp = item as MyObject;
                    if (temp == null) continue;

                    if (temp.canBreak == true)
                    {
                        temp.BreakEvent += Temp_BreakEvent;
                    }
                    if (temp.canCheck == true)
                    {
                        temp.CheckEvent += Temp_CheckEvent;
                    }
                    if (temp.canAttack == true)
                    {
                        temp.AttackEvent += Temp_AttackEvent;
                    }
                    if (temp.canShoot == true)
                    {
                        temp.ShootEvent += Temp_ShootEvent;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    MyObject temp = item as MyObject;
                    if (temp == null) continue;

                    if (temp.canBreak == true)
                    {
                        temp.BreakEvent -= Temp_BreakEvent;
                    }
                    if (temp.canCheck == true)
                    {
                        temp.CheckEvent -= Temp_CheckEvent;
                    }
                    if (temp.canAttack == true)
                    {
                        temp.AttackEvent -= Temp_AttackEvent;
                    }
                    if (temp.canShoot == true)
                    {
                        temp.ShootEvent -= Temp_ShootEvent;
                    }
                }
            }
        }

        private void Temp_ShootEvent(object obj)
        {
            this.Add((MyObject)obj);
        }

        private bool Temp_AttackEvent(object src, object dist)
        {

            Player player = dist as Player;
            MyObject trig = src as MyObject;
            if (player == null || trig == null) return false;

            Vector2 p1_1, p1_2, p2_1, p2_2;
            p1_1.X = player.X;
            p1_1.Y = player.Y;
            p1_2.X = player.X + player.Width;
            p1_2.Y = player.Y + player.Height;

            p2_1.X = trig.X;
            p2_1.Y = trig.Y;
            p2_2.X = trig.X + trig.Width;
            p2_2.Y = trig.Y + trig.Height;

            return !((p1_2.X < p2_1.X) || (p1_1.X > p2_2.X) ||
                (p1_2.Y < p2_1.Y) || (p1_1.Y > p2_2.Y));

            //if ((pointL.X > trig.X) && (pointL.X <= trig.X + trig.Width) &&
            //    (pointL.Y > trig.Y) && (pointL.Y <= trig.Y + trig.Height))
            //{
            //    //               player.X += (trig.X + trig.Width) - (int)pointL.X;
            //    return true;
            //}
            //else if ((pointR.X > trig.X) && (pointR.X <= trig.X + trig.Width) &&
            //    (pointR.Y > trig.Y) && (pointR.Y <= trig.Y + trig.Height))
            //{
            //    //                player.X += (trig.X) - (int)pointR.X;
            //    return true;
            //}
            //else if ((pointT.X > trig.X) && (pointT.X <= trig.X + trig.Width) &&
            //    (pointT.Y > trig.Y) && (pointT.Y <= trig.Y + trig.Height))
            //{
            //    //                player.Y += (trig.Y + trig.Height) - (int)pointT.Y;
            //    return true;
            //}
            //else if ((pointB.X > trig.X) && (pointB.X <= trig.X + trig.Width) &&
            //    (pointB.Y > trig.Y) && (pointB.Y <= trig.Y + trig.Height))
            //{
            //    //                player.Y += (trig.Y) - (int)pointB.Y;
            //    return true;
            //}
            //return false;

        }

        private bool Temp_CheckEvent(object obj)
        {
            Player player = obj as Player; //球
            if (player == null) return false;
            //檢查碰撞對象
            foreach (var item in this)
            {
                //檢查能否反射球
                bool tempBool = item.OnAttack(player);

                //如果有碰撞發生
                if (tempBool == true)
                {
                    int a = 0;
                    //檢查能否消失
                    if (item.OnBreak() == true)
                    {
                        //                       player.Speed += 5;
                        return true;
                    }
                }
            }
            return false;
        }

        private void Temp_BreakEvent(object obj)
        {
            this.Remove((MyObject)obj);
        }
    }
}
