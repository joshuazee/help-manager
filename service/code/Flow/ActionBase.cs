using CMHL.Lawer;
using System.IO;
using System.ServiceModel.Channels;

namespace CMHL.Flow
{
    public partial class Rest : IRest
    {
        /// <summary>
        /// 分发器
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Message Action(Stream stream)
        {
            string method = "";

            int operationUser = 0;
            string token = "";

            if (!Token.ValidToken(operationUser, token))
            {
                //应该有一个通用的结果返回机制，让前台能迅速定位到token过期进行提示，并跳转回登录页面
                return null;
            }

            switch (method)
            {
                
            }
            return null;
        }
    }
}
