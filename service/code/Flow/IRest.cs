using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace CMHL.Flow
{
    [ServiceContract(Namespace = "http://www.cmhlkj.com/")]
    public interface IRest
    {
        [OperationContract]
        [WebInvoke(Method = "Post", UriTemplate = "action", ResponseFormat = WebMessageFormat.Json)]
        Message Action(Stream stream);
    }
}
