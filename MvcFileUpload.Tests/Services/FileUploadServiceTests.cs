using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Moq;
using MvcFileUpload.Services;

namespace MvcFileUpload.Tests.Services
{
    [TestClass]
    public class FileUploadServiceTests
    {
        [TestMethod]
        public void DefaultConstructorTest()
        {
            var instance = new FileUploadService();
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void CreateArgumentObjectTest()
        {
            var instance = new FileUploadService();
            Assert.IsNotNull(instance);
            var args = instance.CreateArgumentObject();
            Assert.IsNotNull(args);
            var ufiles = args.UploadingFiles;
            var nfiles = args.NormalizedFiles;
            var dir = args.UploadingDirectory;
            var times = args.UploadRetryTimes;
            var allowMime = args.AllowMimeTypeFilter;
            var denyMime = args.DenyMimeTypeFilter;
        }

        [TestMethod]
        public void SetupTest()
        {
            // アクセッサを作成
            var instance = new FileUploadService_Accessor();
            Assert.IsNotNull(instance);

            // 設定値を準備
            var allow = true;
            var files = new HttpPostedFileBase[] { null, null };
            var fileUploadPath = @".\";
            var dir = new DirectoryInfo(fileUploadPath);
            var uploadRetryTimes = 12345;
            var allowUploadingMimeFilter = new string[] { "abcd", "efgh" };
            var denyUploadingMimeFilter = new string[] { "ijkl", "mnop" };

            // Setup を呼ぶ
            var args = instance.CreateArgumentObject();
            args.AllowFileUpload = allow;
            args.UploadingFiles = files;
            args.UploadingDirectory = dir;
            args.UploadRetryTimes = uploadRetryTimes;
            args.AllowMimeTypeFilter = allowUploadingMimeFilter;
            args.DenyMimeTypeFilter = denyUploadingMimeFilter;
            instance.Setup(args);

            // 設定値が格納されたか確認
            Assert.AreEqual(allow, instance.Argument.AllowFileUpload);
            Assert.AreSame(files, instance.Argument.UploadingFiles);
            Assert.AreSame(dir, instance.Argument.UploadingDirectory);
            Assert.AreEqual(uploadRetryTimes, instance.Argument.UploadRetryTimes);
            Assert.AreSame(allowUploadingMimeFilter, instance.Argument.AllowMimeTypeFilter);
            Assert.AreSame(denyUploadingMimeFilter, instance.Argument.DenyMimeTypeFilter);
        }

        [TestMethod]
        public void ExecuteTest()
        {
            var uploader = new FileUploadService();

            // 配列で情報を与えられるように、HttpPostedFileBase のリストを作ります。
            var files = new List<HttpPostedFileBase>();

            // テスト素材フォルダー内のファイル名配列を取得します。
            var fixturesPath = ConfigurationManager.AppSettings.Get("FixturesPath");
            var names = Directory.GetFiles(fixturesPath);

            // ファイル名を順次アクセスして HttpPostedFileBase モックを作ります。
            var plresult = Parallel.ForEach(names, name =>
            {
                var file = new Mock<HttpPostedFileBase>();
                var fsFrom = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
                file.Setup(f => f.InputStream).Returns(fsFrom);
                file.Setup(f => f.ContentLength).Returns((int)fsFrom.Length);
                file.Setup(f => f.FileName).Returns(fsFrom.Name);
                file.Setup(f => f.ContentType).Returns(MimeMapping.GetMimeMapping(fsFrom.Name));
                file.Setup(f => f.SaveAs(It.IsAny<string>())).Callback((string path) =>
                {
                    using (var fsTo = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        fsFrom.CopyTo(fsTo);
                    }
                });
                files.Add(file.Object);
            });

            // アップロードされたファイルを格納するディレクトリの情報を取得します。
            var sitePath = ConfigurationManager.AppSettings.Get("SitePath");
            var uploadPath = Path.Combine(sitePath, ConfigurationManager.AppSettings.Get("FileUploadPath").Replace("~/", string.Empty));
            var uploadDir = new DirectoryInfo(uploadPath);

            // アップロードを成功させます。

            // パラメータを渡すためのオブジェクトを作ります。
            dynamic args = uploader.CreateArgumentObject();
            args.AllowFileUpload = true;
            args.UploadingFiles = files;
            args.UploadingDirectory = uploadDir;
            args.UploadRetryTimes = 10;
            args.AllowMimeTypeFilter = null; // this.AllowUploadingMimeFilter;
            args.DenyMimeTypeFilter = null; // this.DenyUploadingMimeFilter;

            // 処理します。
            uploader.Setup(args).Execute();

            // 検証します。
            IEnumerable<dynamic> fileResults = uploader.Result.Select(file => file.UploadedFile.Exists);
            Assert.AreEqual(files.Count(), fileResults.Count());

            // アップロードを拒否します。

            // パラメータを渡すためのオブジェクトを作ります。
            args = uploader.CreateArgumentObject();
            args.AllowFileUpload = false;
            args.UploadingFiles = files;
            args.UploadingDirectory = uploadDir;
            args.UploadRetryTimes = 10;
            args.AllowMimeTypeFilter = null; // this.AllowUploadingMimeFilter;
            args.DenyMimeTypeFilter = null; // this.DenyUploadingMimeFilter;

            // 処理します。
            uploader.Setup(args).Execute();

            // 検証します。
            fileResults = uploader.Result.Select(file => file.UploadedFile.Exists);
            Assert.AreEqual(0, fileResults.Count());

            // ファイル配列でなく null を渡します。

            // パラメータを渡すためのオブジェクトを作ります。
            args = uploader.CreateArgumentObject();
            args.AllowFileUpload = true;
            args.UploadingFiles = null;
            args.UploadingDirectory = uploadDir;
            args.UploadRetryTimes = 10;
            args.AllowMimeTypeFilter = null; // this.AllowUploadingMimeFilter;
            args.DenyMimeTypeFilter = null; // this.DenyUploadingMimeFilter;

            // 処理します。
            uploader.Setup(args).Execute();

            // 検証します。
            fileResults = uploader.Result.Select(file => file.UploadedFile.Exists);
            Assert.AreEqual(0, fileResults.Count());

            // 空のファイル配列を渡します。

            // パラメータを渡すためのオブジェクトを作ります。
            args = uploader.CreateArgumentObject();
            args.AllowFileUpload = true;
            args.UploadingFiles = new HttpPostedFileBase[] { };
            args.UploadingDirectory = uploadDir;
            args.UploadRetryTimes = 10;
            args.AllowMimeTypeFilter = null; // this.AllowUploadingMimeFilter;
            args.DenyMimeTypeFilter = null; // this.DenyUploadingMimeFilter;

            // 処理します。
            uploader.Setup(args).Execute();

            // 検証します。
            fileResults = uploader.Result.Select(file => file.UploadedFile.Exists);
            Assert.AreEqual(0, fileResults.Count());

            // 空の状態で送信した状況を確認します。

            // パラメータを渡すためのオブジェクトを作ります。
            args = uploader.CreateArgumentObject();
            args.AllowFileUpload = true;
            args.UploadingFiles = new HttpPostedFileBase[] {null};
            args.UploadingDirectory = uploadDir;
            args.UploadRetryTimes = 10;
            args.AllowMimeTypeFilter = null; // this.AllowUploadingMimeFilter;
            args.DenyMimeTypeFilter = null; // this.DenyUploadingMimeFilter;

            // 処理します。
            uploader.Setup(args).Execute();

            // 検証します。
            fileResults = uploader.Result.Select(file => file.UploadedFile.Exists);
            Assert.AreEqual(0, fileResults.Count());
        }

        [TestMethod]
        public void IsMatchingInFilter_3Test()
        {
            var instance = new FileUploadService_Accessor();
            var predicator = new Func<string, string, bool>((dt, fltr) => dt == fltr);
            Assert.IsNotNull(instance);
            string[] filter;
            string data;
            Func<string, string, bool> predicate;
            bool returnValue;
            Type expectedExceptionType;
            bool caughtExpectedException;

            predicate = predicator;

            filter = null;
            data = null;
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
            Assert.IsFalse(returnValue);

            filter = new string[] { };
            data = null;
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = null;
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = string.Empty;
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = "data";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = "daTa0";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = " data1 ";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = "data2";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
            Assert.IsTrue(returnValue);

            predicate = null;

            expectedExceptionType = typeof(NullReferenceException);
            caughtExpectedException = false;
            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = "data2";
            try
            {
                returnValue = instance.IsMatchingInFilter_Accessor(data, filter, predicate);
                caughtExpectedException = false;
            }
            catch (NullReferenceException ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                caughtExpectedException = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                caughtExpectedException = false;
            }
            Assert.IsTrue(caughtExpectedException);
        }

        [TestMethod]
        public void IsMatchingInFilter_2Test()
        {
            var instance = new FileUploadService_Accessor();
            Assert.IsNotNull(instance);
            string[] filter;
            string data;
            bool returnValue;

            filter = null;
            data = null;
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsFalse(returnValue);

            filter = new string[] { };
            data = null;
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = null;
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = string.Empty;
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = "data";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = "daTa0";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = " data1 ";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsFalse(returnValue);

            filter = new string[] { "data0", "data1", "data2", "data3" };
            data = "data2";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsTrue(returnValue);

            filter = new string[] { "*", "data0", "data1", "data2", "data3" };
            data = "hoge";
            returnValue = instance.IsMatchingInFilter_Accessor(data, filter);
            Assert.IsTrue(returnValue);
        }

        [TestMethod]
        public void ValidateFilterTest()
        {
            var instance = new FileUploadService_Accessor();
            IEnumerable<string> allowFilter, denyFilter;
            bool projValue;

            // 許可と禁止のどちらの設定もないとき、すべてが許可されます。
            allowFilter = null;
            denyFilter = null;
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xlsx"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xls"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.docx"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.doc"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.txt"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);

            // 許可のみ設定されているときは、許可リスト内のファイルのみ許可されます。
            allowFilter = new string[] { MimeMapping.GetMimeMapping("_.xlsx"), MimeMapping.GetMimeMapping("_.xls") };
            denyFilter = null;
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xlsx"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xls"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.docx"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.doc"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.txt"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);

            // 許可にワイルドカード文字（"*"）があったら、すべて許可されます。
            allowFilter = new string[] { "*", MimeMapping.GetMimeMapping("_.xlsx"), MimeMapping.GetMimeMapping("_.xls") };
            denyFilter = null;
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xlsx"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xls"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.docx"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.doc"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.txt"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);

            // 禁止のみ設定されているときは、禁止リスト内のファイル以外は許可されます。
            allowFilter = null;
            denyFilter = new string[] { MimeMapping.GetMimeMapping("_.xlsx"), MimeMapping.GetMimeMapping("_.xls") };
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xlsx"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xls"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.docx"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.doc"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.txt"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);

            // 禁止にワイルドカード文字（"*"）があったら、すべて禁止されます。
            allowFilter = null;
            denyFilter = new string[] { "*", MimeMapping.GetMimeMapping("_.xlsx"), MimeMapping.GetMimeMapping("_.xls") };
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xlsx"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xls"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.docx"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xldocsx"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.txt"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);

            // 許可と禁止どちらも設定されているときは、許可リスト内でかつ禁止リストにないのファイルのみ許可されます。
            allowFilter = new string[] { MimeMapping.GetMimeMapping("_.xlsx"), MimeMapping.GetMimeMapping("_.xls") };
            denyFilter = new string[] { MimeMapping.GetMimeMapping("_.xls"), MimeMapping.GetMimeMapping("_.doc") };
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xlsx"), allowFilter, denyFilter);
            Assert.IsTrue(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.xls"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.docx"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.doc"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
            projValue = instance.ValidateFilter_Accessor(MimeMapping.GetMimeMapping("_.txt"), allowFilter, denyFilter);
            Assert.IsFalse(projValue);
        }

        [TestMethod]
        public void GetRandomFileNameTest()
        {
            string dirName; int retryTimes;
            Type expectedExceptionType;
            bool caughtExpectedException;
            string resultValue;
            FileInfo fileInformation;

            // アクセッサを作成
            var instance = new FileUploadService_Accessor();
            Assert.IsNotNull(instance);

            // ディレクトリ名に null で ArgumentNullException
            dirName = null;
            retryTimes = 1;
            expectedExceptionType = typeof(ArgumentNullException);
            caughtExpectedException = false;
            try
            {
                resultValue = instance.GetRandomFileName_Accessor(dirName, retryTimes);
                caughtExpectedException = false;
            }
            catch (ArgumentNullException ex)
            {
                caughtExpectedException = true;
                System.Diagnostics.Debug.Print(ex.Message);
            }
            catch (Exception ex)
            {
                caughtExpectedException = false;
                System.Diagnostics.Debug.Print(ex.Message);
            }
            Assert.IsTrue(caughtExpectedException);

            // ディレクトリ名に string.Empty で ArgumentException
            dirName = string.Empty;
            retryTimes = 1;
            expectedExceptionType = typeof(ArgumentException);
            caughtExpectedException = false;
            try
            {
                resultValue = instance.GetRandomFileName_Accessor(dirName, retryTimes);
                caughtExpectedException = false;
            }
            catch (ArgumentException ex)
            {
                caughtExpectedException = true;
                System.Diagnostics.Debug.Print(ex.Message);
            }
            catch (Exception ex)
            {
                caughtExpectedException = false;
                System.Diagnostics.Debug.Print(ex.Message);
            }
            Assert.IsTrue(caughtExpectedException);

            // リトライ回数に -1 で ArgumentException
            dirName = @".\";
            retryTimes = -1;
            expectedExceptionType = typeof(ArgumentException);
            caughtExpectedException = false;
            try
            {
                resultValue = instance.GetRandomFileName_Accessor(dirName, retryTimes);
                caughtExpectedException = false;
            }
            catch (ArgumentException ex)
            {
                caughtExpectedException = true;
                System.Diagnostics.Debug.Print(ex.Message);
            }
            catch (Exception ex)
            {
                caughtExpectedException = false;
                System.Diagnostics.Debug.Print(ex.Message);
            }
            Assert.IsTrue(caughtExpectedException);

            // リトライ回数に 0 で存在しないテンポラリ ファイル名を取得成功
            dirName = @".\";
            retryTimes = 0;
            resultValue = instance.GetRandomFileName_Accessor(dirName, retryTimes);
            Console.WriteLine("[{0}]", resultValue);
            fileInformation = new FileInfo(resultValue);
            Assert.IsFalse(fileInformation.Exists);

            // リトライ回数に 1 で存在しないテンポラリ ファイル名を取得成功
            dirName = @".\";
            retryTimes = 1;
            resultValue = instance.GetRandomFileName_Accessor(dirName, retryTimes);
            Console.WriteLine("[{0}]", resultValue);
            fileInformation = new FileInfo(resultValue);
            Assert.IsFalse(fileInformation.Exists);
        }
    }

    //[TestClass]
    //public class FileUploadedFileResultTests
    //{
    //    [TestMethod]
    //    public void OverloadConstructorTest()
    //    {
    //        var instance = new FileUploader.FileResultObject(null);
    //        Assert.IsNotNull(instance);
    //        Assert.Fail();
    //    }

    //    [TestMethod]
    //    public void UpdateResultTest()
    //    {
    //        var instance = new FileUploader.FileResultObject(null);
    //        Assert.IsNotNull(instance);
    //        Assert.Fail();
    //    }
    //}

    //[TestClass]
    //public class FileUploadedResultTests
    //{
    //    [TestMethod]
    //    public void OverloadConstructorTest()
    //    {
    //        var instance = new FileUploader.ResultObject(null);
    //        Assert.IsNotNull(instance);
    //        Assert.Fail();
    //    }

    //    [TestMethod]
    //    public void UpdateResultTest()
    //    {
    //        var instance = new FileUploader.FileResultObject(null);
    //        Assert.IsNotNull(instance);
    //        Assert.Fail();
    //    }
    //}
}
