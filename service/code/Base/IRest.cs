using CMHL.Lawer;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace CMHL.Base
{
    [ServiceContract(Namespace = "http://www.cmhlkj.com/")]
    /// <summary>
    /// 权限相关接口集合  用户  角色  部门  权限设置以及登录
    /// </summary>
    public interface IRest
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "action/{method}", ResponseFormat = WebMessageFormat.Json)]
        Message Action(string method, Stream stream);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "upload", ResponseFormat = WebMessageFormat.Json)]
        GeneralResult<string> upload(Stream stream);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "upload2", ResponseFormat = WebMessageFormat.Json)]
        GeneralResult<string[]> upload2(Stream stream);
    }
}
