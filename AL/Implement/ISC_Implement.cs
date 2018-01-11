using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AL.Implement
{
    class ISC_Implement : AL_Service.IAL_ServiceCallback
    {
        public MainWindow mw;

        public ISC_Implement(MainWindow _mw)
        {
            mw = _mw;
        }

        /// <summary>
        /// 上线提醒
        /// </summary>
        /// <param name="_login"></param>
        public void Login_Prompt(string _login)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 消息
        /// </summary>
        /// <param name="_sm"></param>
        public void Message(AL_Service.DTO_SMess _sm)
        {
            mw.m.AddMessage(_sm);
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 下线提醒
        /// </summary>
        /// <param name="_num"></param>
        public void LogoutMessa(string _num)
        {
            //throw new NotImplementedException();
        }
    }
}
