using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcFileUpload.Helpers;
using MvcFileUpload.Services;

namespace MvcFileUpload.Controllers
{
    public class SubmitController : Controller
    {
        #region *** Constructors and helpers.

        /// <summary>
        /// 新しい SubmitController クラス インスタンスを初期化します。
        /// </summary>
        public SubmitController()
        {
            this.CommonInit();
        }

        /// <summary>
        /// SubmitController クラス インスタンスの共通の初期化処理を実装します。
        /// </summary>
        private void CommonInit()
        {
            this.SettingsHelper = new AppSettingsHelper();
        }

        #endregion

        #region *** Methods for this class.

        /// <summary>
        /// 構成ファイル appSettings セクションからの値の取得のヘルパー オブジェクトを取得または設定します。
        /// </summary>
        private AppSettingsHelper SettingsHelper { get; set; }

        #endregion

        #region *** Actions.

        #region *** Index action.

        // GET: Submit
        /// <summary>
        /// Submit コントローラーの Upload アクションを実行します。
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Result = null;
            return View();
        }

        #endregion

        #region *** Upload action.

        // POST: Sample/Upload
        /// <summary>
        /// Submit コントローラーの Upload アクションを実行します。
        /// </summary>
        /// <param name="files">アップロード ファイルの配列。</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase[] files)
        {
            // 実行クラスのインスタンスを作ります。
            var uploader = new FileUploadService();

            // アップロードされたファイルを格納するディレクトリの情報を取得します。
            var dir = new DirectoryInfo(Server.MapPath(this.FileUploadPath));

            // パラメータを渡すための動的オブジェクトを作ります。
            var args = uploader.CreateArgumentObject();

            // 入力ファイルのクエリを設定します。
            args.UploadingFiles = files;

            // その他のパラメータを設定します。
            args.AllowFileUpload = this.AllowFileUpload;
            args.UploadingDirectory = dir;
            args.UploadRetryTimes = this.UploadRetryTimes;
            args.AllowMimeTypeFilter = this.AllowUploadingMimeFilter;
            args.DenyMimeTypeFilter = this.DenyUploadingMimeFilter;

            // 処理します。
            var result = uploader.Setup(args).Execute().Result;

            // ファイル名を変更します。
            var upfiles = result.Files;
            upfiles.ToList().ForEach(upfile =>
            {
                if (upfile.UploadedFile.Exists)
                {
                    upfile.AdditionalProperties.TemporaryFileName = upfile.UploadedFile.Name;
                    var clfn = Path.GetFileNameWithoutExtension(upfile.PostedFile.FileName);
                    var clext = Path.GetExtension(upfile.PostedFile.FileName);
                    var newName = string.Format("{{{0}}}-{1}{2}", Guid.NewGuid().ToString(), DateTime.Now.Ticks, clext);                    
                    var newPath = System.IO.Path.Combine(dir.FullName, newName);
                    upfile.UploadedFile.Object.MoveTo(newPath);
                }
            });

            // 結果を表示します。
            ViewBag.Argument = uploader.Argument;
            ViewBag.Result = uploader.Result;

            // 通常のビューを返します。
            return View("Index");
        }

        #region *** Properties for Upload action.

        /// <summary>
        ///  構成ファイルから、ファイル アップロードを許可するかどうかを示す値を取得します。
        /// </summary>
        /// <remarks>
        /// 構成ファイルで、キー "AllowFileUpload" に対して設定されている値が評価されます。
        /// 値が "true" のときは true が返されます（すべて小文字である必要があります）。
        /// 値が "false" のときは false が返されます（すべて小文字である必要があります）。
        /// キーが見つからなかったとき、または値が "true" や "false" でなかったときは、ConfigurationErrorsException 例外をスローします。
        /// </remarks>
        private bool AllowFileUpload
        {
            get
            {
                string key = "AllowFileUpload";
                bool returnValue = this.SettingsHelper.GetAsBoolean(key);
                return returnValue;
            }
        }

        /// <summary>
        /// 構成ファイルから、アップロード ファイルを格納するディレクトリ名を示す値を取得します。
        /// </summary>
        /// <remarks>
        /// 構成ファイルで、キー "FileUploadPath" に対して設定されている値が評価されます。
        /// キーが見つからなかったときは null が返されます。
        /// </remarks>
        private string FileUploadPath
        {
            get
            {
                string key = "FileUploadPath";
                var returnValue = this.SettingsHelper.GetAsOptionalString(key);
                return returnValue;
            }
        }

        /// <summary>
        /// 構成ファイルから、アップロード ファイルを格納するリトライ回数を示す値を取得します。
        /// </summary>
        /// <remarks>
        /// 構成ファイルで、キー "UploadRetryTimes" に対して設定されている値が評価されます。
        /// </remarks>
        private int UploadRetryTimes
        {
            get
            {
                string key = "UploadRetryTimes";
                var returnValue = this.SettingsHelper.GetAsInt32(key);
                return returnValue;
            }
        }

        /// <summary>
        /// 構成ファイルからアップロードが許可された MIME タイプ一覧を取得して、文字列の射影にして返します。
        /// </summary>
        /// <remarks>
        /// 構成ファイルで、キー "AllowUploadingMimeFilter" に対して設定されている値が評価されます。
        /// </remarks>
        private IEnumerable<string> AllowUploadingMimeFilter
        {
            get
            {
                string key = "AllowUploadingMimeFilter";
                var returnValue = this.SettingsHelper.GetAsOptionalEnumerableString(key, ';');
                return returnValue;
            }
        }

        /// <summary>
        /// 構成ファイルからアップロードが禁止された MIME タイプ一覧を取得して、文字列の射影にして返します。
        /// </summary>
        /// <remarks>
        /// 構成ファイルで、キー "DenyUploadingMimeFilter" に対して設定されている値が評価されます。
        /// キーが見つからなかったときは空の射影が返されます。
        /// </remarks>
        private IEnumerable<string> DenyUploadingMimeFilter
        {
            get
            {
                string key = "DenyUploadingMimeFilter";
                var returnValue = this.SettingsHelper.GetAsOptionalEnumerableString(key, ';');
                return returnValue;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}

