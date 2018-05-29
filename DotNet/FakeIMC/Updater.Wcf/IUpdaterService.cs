using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Updater.Wcf
{
    [ServiceContract]
    public interface IUpdaterService
    {
        [OperationContract]
        UpdateInfo CheckNewVersion(Version currentVersion);
    }

    [DataContract]
    public class UpdateInfo
    {
        [DataMember]
        public Version NewVersion { get; set; }

        [DataMember]
        public string RemotePathToInstaller { get; set; } 
       
    }
}
