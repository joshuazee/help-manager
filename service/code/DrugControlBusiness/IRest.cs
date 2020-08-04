using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace DrugControlBusiness
{
    [ServiceContract(Namespace = "http://www.cmhlkj.com/")]
    public interface IRest
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "action/{method}", ResponseFormat = WebMessageFormat.Json)]
        Message Action(string method, Stream stream);

        //[OperationContract]
        //[WebInvoke(Method = "POST", UriTemplate = "video_upload?title={title}&uploader={uploader}", ResponseFormat = WebMessageFormat.Json)]
        //bool VideoUpload(string title, string uploader, Stream stream);
    }
}