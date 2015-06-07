using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MvcFileUpload.Services
{
    public class FileUploadService
    {
        #region *** Inner classes.

        #region *** FileInfoWrapper

        public class FileInfoWrapper
        {
            #region *** Constructors.

            private FileInfoWrapper()
            {
                this._Object = null;
            }
            public FileInfoWrapper(string fileName)
            {
                this._Object = new FileInfo(fileName);
            }
            static FileInfoWrapper()
            {
                _Empty = new FileInfoWrapper();
            }

            #endregion

            #region *** Fields & Properites.

            protected FileInfo _Object;
            public FileInfo Object { get { return this._Object; } }

            private static FileInfoWrapper _Empty;
            public static FileInfoWrapper Empty { get { return _Empty; } }

            public string Name { get { return this.Object == null ? string.Empty : this.Object.Name; } }
            public bool Exists { get { return this.Object == null ? false : this.Object.Exists; } }

            #endregion
        }

        #endregion

        #region *** ArgumentObject class.

        /// <summary>
        /// ファイル アップロード処理の状況を設定するためのクラスです。
        /// </summary>
        public class ArgumentObject
        {
            public ArgumentObject()
            {
                this.AdditionalProperties = new ExpandoObject();
            }
            public IEnumerable<HttpPostedFileBase> UploadingFiles { get; set; }
            public IEnumerable<HttpPostedFileBase> NormalizedFiles { get; set; }
            public bool AllowFileUpload { get; set; }
            public DirectoryInfo UploadingDirectory { get; set; }
            public int UploadRetryTimes { get; set; }
            public IEnumerable<string> AllowMimeTypeFilter { get; set; }
            public IEnumerable<string> DenyMimeTypeFilter { get; set; }
            public dynamic AdditionalProperties { get; private set; }
            internal ArgumentObject Setup()
            {
                var files = new List<HttpPostedFileBase>();
                if (this.UploadingFiles != null)
                {
                    this.UploadingFiles.ToList().ForEach(file =>
                    {
                        if (file != null)
                        {
                            files.Add(file);
                        }
                    });
                }
                this.NormalizedFiles = files;
                return this;
            }
        }

        #endregion

        #region *** ResultObject class.

        /// <summary>
        /// ファイル アップロード処理の結果を示すためのクラスです。
        /// </summary>
        public class ResultObject
        {
            public ResultObject(IEnumerable<ResultFileObject> files)
            {
                this.Files = files;
                this.AdditionalProperties = new ExpandoObject();
            }
            public bool AllowFileUpload { get; private set; }
            public DirectoryInfo UploadingDirectory { get; private set; }
            public IEnumerable<ResultFileObject> Files { get; private set; }
            public dynamic AdditionalProperties { get; private set; }
            internal virtual ResultObject UpdateResult(bool allowFileUpload, DirectoryInfo uploadingDirectory)
            {
                this.AllowFileUpload = allowFileUpload;
                this.UploadingDirectory = uploadingDirectory;
                return this;
            }
            public virtual IEnumerable<ResultFileObject> Select(Func<ResultFileObject, bool> predicate)
            {
                var files = new List<ResultFileObject>();
                this.Files.ToList().ForEach(file =>
                {
                    if (predicate(file)) files.Add(file);
                });
                return files;
            }
        }

        #endregion

        #region *** ResultFileObject class.

        /// <summary>
        /// ファイル アップロード処理のファイルごとの結果を示すためのクラスです。
        /// </summary>
        public class ResultFileObject
        {
            public ResultFileObject(HttpPostedFileBase file)
            {
                this.PostedFile = file;
                this.UploadedFile = FileInfoWrapper.Empty;
                this.AdditionalProperties = new ExpandoObject();
            }
            public HttpPostedFileBase PostedFile { get; private set; }
            internal virtual ResultFileObject UpdateResult(bool isValidMime, FileInfoWrapper fiWrapper)
            {
                this.IsValidMimeType = isValidMime;
                this.UploadedFile = fiWrapper;
                return this;
            }
            public bool IsValidMimeType { get; private set; }
            public FileInfoWrapper UploadedFile { get; set; }
            public dynamic AdditionalProperties { get; private set; }
        }

        #endregion

        #endregion

        #region *** Constructor.

        /// <summary>
        /// 新しい FileUploadService クラス インスタンスを初期化します。
        /// </summary>
        public FileUploadService()
        {
        }

        #endregion

        #region *** Properties.

        /// <summary>
        /// アップロードしようとする操作の状況設定を取得します。
        /// </summary>
        public virtual ArgumentObject Argument { get; protected set; }

        /// <summary>
        /// アップロードしようとした操作の結果を取得します。
        /// </summary>
        public virtual ResultObject Result { get; protected set; }

        #endregion

        #region *** Public methods (Facade).

        /// <summary>
        /// ファイル アップロード操作の状況を設定するためのオブジェクトを取得します。
        /// </summary>
        /// <returns></returns>
        public virtual ArgumentObject CreateArgumentObject()
        {
            return new ArgumentObject();
        }

        /// <summary>
        /// 指定されたファイル アップロード操作の状況を設定します。
        /// </summary>
        /// <param name="args">ファイル アップロード操作の状況が設定されたオブジェクト。</param>
        /// <returns></returns>
        public virtual FileUploadService Setup(ArgumentObject args)
        {
            this.Argument = args;
            this.Argument.Setup();
            return this;
        }

        /// <summary>
        /// ファイル アップロード処理を行います。
        /// </summary>
        /// <returns></returns>
        public virtual FileUploadService Execute()
        {
            var sourceFiles = this.Argument.NormalizedFiles;
            var resFiles = new List<ResultFileObject>();
            sourceFiles.ToList().ForEach(file => resFiles.Add(new ResultFileObject(file)));
            this.Result = new ResultObject(resFiles);
            bool allowFileUpload = this.Argument.AllowFileUpload;
            bool isDirectoryExists = this.Argument.UploadingDirectory.Exists;
            if (allowFileUpload && isDirectoryExists)
            {
                var plresult = Parallel.ForEach(resFiles, file =>
                {
                    FileInfoWrapper fiWrapper = FileInfoWrapper.Empty;
                    var tmppath = string.Empty;
                    var isValidMime = this.ValidateFilter(file.PostedFile.ContentType, this.Argument.AllowMimeTypeFilter, this.Argument.DenyMimeTypeFilter);
                    if (isValidMime)
                    {
                        tmppath = this.GetRandomFileName(this.Argument.UploadingDirectory.FullName, this.Argument.UploadRetryTimes);
                        if (!string.IsNullOrEmpty(tmppath))
                        {
                            file.PostedFile.SaveAs(tmppath);
                            fiWrapper = new FileInfoWrapper(tmppath);
                        }
                    }
                    file.UpdateResult(isValidMime, fiWrapper);
                });
            }
            this.Result.UpdateResult(allowFileUpload, this.Argument.UploadingDirectory);
            return this;
        }

        #endregion

        #region *** Protected methods.

        // ***
        protected virtual bool IsMatchingInFilter(string data, IEnumerable<string> filter, Func<string, string, bool> predicate)
        {
            var returnValue = false;
            if (filter != null)
            {
                foreach (var fltr in filter)
                {
                    returnValue = predicate(data, fltr);
                    if (returnValue) break;
                }
            }
            return returnValue;
        }

        // ***
        protected virtual bool IsMatchingInFilter(string data, IEnumerable<string> filter)
        {
            var returnValue = this.IsMatchingInFilter(data, filter, (dt, fltr) => (fltr == "*") || (fltr == dt));
            return returnValue;
        }

        // ***
        protected virtual bool ValidateFilter(string pattern, IEnumerable<string> allowFilter, IEnumerable<string> denyFilter)
        {
            var returnValue = false;
            returnValue = allowFilter == null; // 許可フィルター設定かなければすべて許可
            if (!returnValue)
            {
                returnValue = this.IsMatchingInFilter(pattern, allowFilter);
            }
            if (!returnValue) return returnValue; // 許可フィルターがあり、かつマッチしない場合はすぐに拒否
            if (denyFilter != null) // 許可フィルターにマッチして拒否フィルター
            {
                returnValue = !this.IsMatchingInFilter(pattern, denyFilter);
            }
            return returnValue;
        }

        // ***
        protected virtual string GetRandomFileName(string dirName, int retryTimes)
        {
            if (retryTimes < 0) throw new ArgumentException(string.Empty, "retryTimes");
            var dir = System.IO.Path.GetFullPath(dirName);
            for (var i = 0; i <= retryTimes; i++)
            {
                var tmpfn = System.IO.Path.GetRandomFileName();
                var tmppath = System.IO.Path.Combine(dir, tmpfn);
                if (!(new System.IO.FileInfo(tmppath).Exists))
                {
                    return (tmppath);
                }
            }
            return string.Empty;
        }

        #endregion
    }
}

