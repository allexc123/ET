using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    public class UIBase : Component
    {
        public virtual void OnInit(object param)
        {

        }

        public virtual void OnShow(object param)
        {

        }

        public virtual void OnEnable()
        {

        }

        /// <summary>
        /// 界面当前节点被设置隐藏时调用
        /// </summary>
        public virtual void OnDisable()
        {

        }
        /// <summary>
        /// 界面销毁时
        /// </summary>
        public virtual void OnDestroy()
        {


        }
    }
}
