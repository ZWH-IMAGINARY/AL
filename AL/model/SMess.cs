using AL.AL_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL.model
{
    [Serializable]
    public class SMess
    {
        /// <summary>
        /// 发送人
        /// </summary>
        public UserInfo Despatcher { get; set; }

        /// <summary>
        /// 发送到
        /// </summary>
        public DTO_Friend SendToNum { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 消息类型 0：普通消息  1：文件
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendDate { get; set; }

        /// <summary>
        /// 阅读状态 1：未阅读   0：已阅读
        /// </summary>
        public int ReadType { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        private string Furl;
        public string Furl1
        {
            get { return Furl; }
            set
            {
                if (type == 1)
                    Furl = value;
                else
                    Furl = "";
            }
        }
    }
}
