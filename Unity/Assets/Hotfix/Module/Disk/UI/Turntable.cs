

using System.Collections;
using System.Collections.Generic;

using ETModel;
using UnityEngine;
//using UnityEngine.UI;

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

        //private float acc = 0f; //加速度
        //private float maxSpeed = 0f; //最大速度
        //private float gearNum = 12;
        //private float decRoll = 0;  // 减速旋转两圈

        //private float spinTime = 0; //减速前旋转时间

        //private int wheelState = 0; //转盘的状态
        //private float curSpeed = 0; //当前速度

        ////private int targetIndex = 0;

        //public float _endAngle;
        //public float endAngle
        //{
        //    get
        //    {
        //        return _endAngle;
        //    }
        //    set
        //    {
        //        _endAngle = Mathf.Abs(value);
        //        _endAngle = _endAngle % 360; //将角度限定在[0, 360]这个区间
        //        _endAngle = -_endAngle - 360 * this.decRoll; //多N圈并取反，圈数能使减速阶段变得更长，显示更自然，逼真
        //    }
        //}

        //public float tempAngle = 0;

        //public void Awake(GameObject gameObject)
        //{

        //    this.GameObject = gameObject;

        //}

        //public void begin(int id, int targetIndex)
        //{
        //    DiskConfig diskConfig = (DiskConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(DiskConfig), id);

        //    this.acc = diskConfig.acc;
        //    this.maxSpeed = diskConfig.maxSpeed;
        //    this.decRoll = diskConfig.decRoll;

        //    this.wheelState = 1;

        //   //this.targetIndex = 1;

        //    this.endAngle = 360 / this.gearNum * targetIndex;
        //}

        //public void Update()
        //{
        //    //Log.Debug("update");

        //    //this.big.transform.Rotate(Vector3.back, -50 * Time.deltaTime, Space.World);
        //    if (this.wheelState == 0)
        //    {
        //        return;
        //    }
        //    if (this.wheelState == 1)
        //    {

        //        this.GameObject.transform.Rotate(Vector3.back, this.curSpeed * Time.deltaTime);

        //        this.curSpeed += acc * Time.deltaTime;

        //        if (this.curSpeed >= this.maxSpeed)
        //        {
        //            this.curSpeed = this.maxSpeed;
        //            spinTime += Time.deltaTime;
        //        }

        //        if (spinTime > 5)
        //        {
        //            this.wheelState = 2;

        //            this.tempAngle = (-1) * (360 - this.GameObject.transform.eulerAngles.z) % 360;

        //            //this.endAngle = (360 - this.big.transform.eulerAngles.z % 360) + (360 / this.gearNum) * this.targetIndex + 360 * this.decRoll;

        //        }


        //    }
        //    if (this.wheelState == 2)
        //    {
        //        //通过差值运算实现精准地旋转到指定角度（球型插值无法实现大于360°的计算）
        //        float k = 0.5f; //如果嫌减速太慢，可以加个系数修正一下
        //        tempAngle = Mathf.Lerp(tempAngle, endAngle, Time.deltaTime * k);

        //        //这里只存在一个方向的旋转，所以不存在欧拉角万向节的问题，所以使用欧拉角和四元数直接赋值都是可以的
        //        this.GameObject.transform.rotation = Quaternion.Euler(0, 0, tempAngle);

        //        if (Mathf.Abs(tempAngle - endAngle) <= 1)
        //        {
        //            //this.big.transform.rotation = Quaternion.Euler(0,0, (360 / this.gearNum) * this.targetIndex);
        //            this.wheelState = 0;
        //        }

        //    }


        //}

        public List<AnimationCurve> animationCurves; //动画曲线列表 

        private bool spinning;  //是否在旋转中
        private float anglePerItem;  //每个item角度(360/item个数)
        private int randomTime;  //旋转时间
        private int itemNumber;  //item个数

        private bool rotateCommand = false; //旋转命令
        private int targetItemIndex; //目标item索引(从0开始)
        private bool CW = true; //是否顺时针
        private System.Action EndCallBack; //旋转结束回调

        public void Awake(GameObject gameObject)
        {

            this.GameObject = gameObject;

            spinning = false;
            //避免没有预设曲线报错(这里建一条先慢再快再慢的动画曲线)
            if (animationCurves == null)
            {
                Keyframe[] ks = new Keyframe[3];
                ks[0] = new Keyframe(0, 0);
                ks[0].inTangent = 0;
                ks[0].outTangent = 0;
                ks[1] = new Keyframe(0.5f, 0.5f);
                ks[1].inTangent = 1;
                ks[1].outTangent = 1;
                ks[2] = new Keyframe(1, 1);
                ks[2].inTangent = 0;
                ks[2].outTangent = 0;
                AnimationCurve animationCurve = new AnimationCurve(ks);
                animationCurves.Add(animationCurve);
            }

        }

        /// <summary>
        /// 开启旋转调用(外部调用)
        /// </summary>
        /// <param name="itemNum">item总个数</param>
        /// <param name="itemIndex">目标item索引，从0开始</param>
        /// <param name="cw">是否顺时针</param>
        /// <param name="callback">结束回调</param>
        public void RotateUp(int itemNum, int itemIndex, bool cw, System.Action callback)
        {
            itemNumber = itemNum;
            anglePerItem = 360 / itemNumber;
            targetItemIndex = itemIndex;
            CW = cw;
            EndCallBack = callback;
            rotateCommand = true;
        }

        public void Update()
        {
            if (rotateCommand && !spinning)
            {

                randomTime = Random.Range(6, 8);  //随机获取旋转全角的次数 

                float maxAngle = 360 * randomTime + (targetItemIndex * anglePerItem);  //需要旋转的角度
                rotateCommand = false;

                //StartCoroutine(SpinTheWheel(randomTime, maxAngle));
               

            }
        }

        IEnumerator SpinTheWheel(float time, float maxAngle)
        {
            spinning = true;

            float timer = 0.0f;
            float startAngle = this.GameObject.transform.eulerAngles.z;
            //减去相对于0位置的偏移角度
            maxAngle = maxAngle - GetFitAngle(startAngle);
            //根据顺时针逆时针不同，不同处理
            int cw_value = 1;
            if (CW)
            {
                cw_value = -1;
            }
            int animationCurveNumber = Random.Range(0, animationCurves.Count);  //获取一个随机索引

            while (timer < time)
            {
                //计算旋转,动画曲线的Evaluate函数返回了给定时间下曲线上的值：从0到1逐渐变化，速度又每个位置的切线斜率决定。
                float angle = maxAngle * animationCurves[animationCurveNumber].Evaluate(timer / time);
                //得到的angle从0到最大角度逐渐变化 速度可变,让给加到旋转物角度上实现逐渐旋转 速度可变
                this.GameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, cw_value * angle + startAngle);
                timer += Time.deltaTime;
                yield return 0;
            }

            //避免旋转有误，最终确保其在该在的位置
            this.GameObject.transform.eulerAngles = new Vector3(0.0f, 0.0f, cw_value * maxAngle + startAngle);
            //执行回调 
            if (EndCallBack != null)
            {
                EndCallBack();
                EndCallBack = null;
            }
            spinning = false;
        }

        //获取相对角度
        private float GetFitAngle(float angle)
        {
            if (angle > 0)
            {
                if (angle - 360 > 0)
                {
                    return GetFitAngle(angle - 360);
                }
                else
                {
                    return angle;
                }
            }
            else
            {
                if (angle + 360 < 0)
                {
                    return GetFitAngle(angle + 360);
                }
                else
                {
                    return angle;
                }
            }
        }
    }
}
