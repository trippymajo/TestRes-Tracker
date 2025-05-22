using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrtShared.ServiceCommunication
{
    public class S3AwsSettings
    {
        public string BucketName { get; set; } = string.Empty;
        public string Region {  get; set; } = string.Empty;
        public bool UseEncryption { get; set; } = false;
        public string? EncryptionKeyId { get; set; }
    }
}
