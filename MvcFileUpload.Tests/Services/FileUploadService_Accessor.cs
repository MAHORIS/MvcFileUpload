using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using MvcFileUpload.Services;

namespace MvcFileUpload.Tests.Services
{
    public class FileUploadService_Accessor : FileUploadService
    {
        //public IEnumerable<HttpPostedFileBase> UploadingFiles_Accessor { get { return base.UploadingFiles; } }
        //public DirectoryInfo UploadingDirectory_Accessor { get { return base.UploadingDirectory; } }
        //public int UploadRetryTimes_Accessor { get { return base.UploadRetryTimes; } }
        //public IEnumerable<string> AllowMimeTypeFilter_Accessor { get { return base.AllowMimeTypeFilter; } }
        //public IEnumerable<string> DenyMimeTypeFilter_Accessor { get { return base.DenyMimeTypeFilter; } }
        //public IEnumerable<string> AllowExtensionFilter_Accessor { get { return base.AllowExtensionFilter; } }
        //public IEnumerable<string> DenyExtensionFilter_Accessor { get { return base.DenyExtensionFilter; } }

        public bool IsMatchingInFilter_Accessor(string data, IEnumerable<string> filter, Func<string, string, bool> predicate)
        {
            return base.IsMatchingInFilter(data, filter, predicate);
        }
        public bool IsMatchingInFilter_Accessor(string data, IEnumerable<string> filter)
        {
            return base.IsMatchingInFilter(data, filter);
        }
        public bool ValidateFilter_Accessor(string pattern, IEnumerable<string> allowFilter, IEnumerable<string> denyFilter)
        {
            return base.ValidateFilter(pattern, allowFilter, denyFilter);
        }
        public string GetRandomFileName_Accessor(string dirName, int retryTimes)
        {
            return base.GetRandomFileName(dirName, retryTimes);
        }
    }
}
