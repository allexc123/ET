using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class TurntableAwakeSystemq : AwakeSystem<Turntable, GameObject>
    {
        public override void Awake(Turntable self, GameObject gameObject)
        {
            self.Awake(gameObject);
        }
    }
    [ObjectSystem]
    public class TurntableUpdateSystem : UpdateSystem<Turntable>
    {
        public override void Update(Turntable self)
        {
            self.Update();
        }
    }

    //[HideInHierarchy]
    public class Turntable : Entity
    {

        private float acc = 0f; //加速度
        private float maxSpeed = 0f; //最大速度
        private float gearNum = 12;
        private float decRoll = 0;  // 减速旋转两圈

        private float spinTime = 0; //减速前旋转时间

        private int wheelState = 0; //转盘的状态
        private float curSpeed = 0; //当前速度

        //private int targetIndex = 0;

        public float _endAngle;
        public float endAngle
        {
            get
            {
                return _endAngle;
            }
            set
            {
                _endAngle = Mathf.Abs(value);
                _endAngle = _endAngle % 360; //将角度限定在[0, 360]这个区间
                _endAngle = -_endAngle - 360 * this.decRoll; //多N圈并取反，圈数能使减速阶段变得更长，显示更自然，逼真
            }
        }

        public float tempAngle = 0;

        public void Awake(GameObject gameObject)
        {

            this.GameObject = gameObject;

        }

        public void begin(int id, int targetIndex)
        {
            DiskConfig diskConfig = (DiskConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(DiskConfig), id);

            this.acc = diskConfig.acc;
            this.maxSpeed = diskConfig.maxSpeed;
            this.decRoll = diskConfig.decRoll;

            this.wheelState = 1;

           //this.targetIndex = 1;

            this.endAngle = 360 / this.gearNum * targetIndex;
        }

        public void Update()
        {
            //Log.Debug("update");

            //this.big.transform.Rotate(Vector3.back, -50 * Time.deltaTime, Space.World);
            if (this.wheelState == 0)
            {
                return;
            }
            if (this.wheelState == 1)
            {

                this.GameObject.transform.Rotate(Vector3.back, this.curSpeed * Time.deltaTime);

                this.curSpeed += acc * Time.deltaTime;

                if (this.curSpeed >= this.maxSpeed)
                {
                    this.curSpeed = this.maxSpeed;
                    spinTime += Time.deltaTime;
                }

                if (spinTime > 5)
                {
                    this.wheelState = 2;

                    this.tempAngle = (-1) * (360 - this.GameObject.transform.eulerAngles.z) % 360;

                    //this.endAngle = (360 - this.big.transform.eulerAngles.z % 360) + (360 / this.gearNum) * this.targetIndex + 360 * this.decRoll;

                }


            }
            if (this.wheelState == 2)
            {
                //通过差值运算实现精准地旋转到指定角度（球型插值无法实现大于360°的计算）
                float k = 0.5f; //如果嫌减速太慢，可以加个系数修正一下
                tempAngle = Mathf.Lerp(tempAngle, endAngle, Time.deltaTime * k);

                //这里只存在一个方向的旋转，所以不存在欧拉角万向节的问题，所以使用欧拉角和四元数直接赋值都是可以的
                this.GameObject.transform.rotation = Quaternion.Euler(0, 0, tempAngle);

                if (Mathf.Abs(tempAngle - endAngle) <= 1)
                {
                    //this.big.transform.rotation = Quaternion.Euler(0,0, (360 / this.gearNum) * this.targetIndex);
                    this.wheelState = 0;
                }

            }


        }
    }
}
