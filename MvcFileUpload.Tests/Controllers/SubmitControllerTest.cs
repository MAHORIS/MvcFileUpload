using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using MvcFileUpload.Helpers;
using MvcFileUpload.Controllers;
using MvcFileUpload.Tests.Helpers;

namespace MvcFileUpload.Tests.Controllers
{
    [TestClass]
    public class SubmitControllerTest
    {
        #region *** Cupsuled members.

        /// <summary>
        /// 指定されたふたつの文字列の射影を比較します。
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        private bool CompareStrings(IEnumerable<string> s1, IEnumerable<string> s2)
        {
            if (s1.Count() != s2.Count()) return false;
            for (var i = 0; i < s1.Count(); i++)
            {
                if (s1.ElementAt(i) != s2.ElementAt(i)) return false;
            }
            return true;
        }

        #endregion

        #region *** Test methods.

        [TestMethod]
        public void DefaultConstructorTest()
        {
            // コントローラーを作成します。
            var controller = new SubmitController();
            Assert.IsNotNull(controller);
        }

        [TestMethod]
        public void CommonInitTest()
        {
            // コントローラーを作成します。
            var controller = new SubmitController();
            Assert.IsNotNull(controller);
            PrivateObject po = new PrivateObject(controller);
            var helper = po.GetProperty("SettingsHelper");
            Assert.IsNotNull(helper);
        }

        [TestMethod]
        public void AllowFileUploadTest()
        {
            // プロジェクトとテストの比較のための変数です。
            string memberName = "AllowFileUpload";
            string helperName = "SettingsHelper";
            bool projValue, testValue;
            string settingValue = string.Empty;
            Type expectedExceptionType = null;
            bool caughtExpectedException;

            // コントローラーとプライベート オブジェクトを作成します。
            var controller = new SubmitController();
            PrivateObject po = new PrivateObject(controller);

            // settingValue を返すようにカスタマイズした AppSettingsHelper_Accessor を作成します。
            var helper = new AppSettingsHelper_Accessor();
            helper.SetGetter(key => settingValue);

            // コントローラーの SettingsHelper をカスタマイズした AppSettingsHelper_Accessor に設定します。
            po.SetProperty(helperName, helper);

            // 構成ファイル想定値で "true" を取得させて true を確認します。
            testValue = true;
            settingValue = testValue.ToString().ToLower();
            projValue = (bool)po.GetProperty(memberName);
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で "false" を取得させて false を確認します。
            testValue = false;
            settingValue = testValue.ToString().ToLower();
            projValue = (bool)po.GetProperty(memberName);
            Assert.AreEqual(projValue, testValue);

            // null でなくて true でも false でもなければ ConfigurationErrorsException 例外をスローします。
            testValue = true;
            settingValue = " " + testValue.ToString().ToLower() + " ";
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = (bool)po.GetProperty(memberName);
                caughtExpectedException = false;
            }
            catch (ConfigurationErrorsException ex)
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

            // キーがない場合を確認します。
            settingValue = null;
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = (bool)po.GetProperty(memberName);
                caughtExpectedException = false;
            }
            catch (ConfigurationErrorsException ex)
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

            // Get() を加工しない状態で動作を確認します。
            controller = new SubmitController();
            po = new PrivateObject(controller);
            projValue = (bool)po.GetProperty(memberName);
            var tmpValue = ConfigurationManager.AppSettings.Get("AllowFileUpload");
            tmpValue = (tmpValue ?? string.Empty).ToLower();
            var isTrue = tmpValue == "true".ToLower();
            var isFalse = tmpValue.ToLower() == "false".ToLower();
            Assert.AreNotEqual(isTrue, isFalse);
            testValue = isTrue;
            Assert.AreEqual(projValue, testValue);
        }

        [TestMethod]
        public void FileUploadPathTest()
        {
            // プロジェクトとテストの比較のための変数です。
            string memberName = "FileUploadPath";
            string helperName = "SettingsHelper";
            string projValue = string.Empty, testValue = string.Empty;
            string settingValue = string.Empty;

            // コントローラーを作成します。
            var controller = new SubmitController();

            // プライベート オブジェクトを作成します。
            PrivateObject po = new PrivateObject(controller);

            // settingValue を返すようにカスタマイズした AppSettingsHelper_Accessor を作成します。
            var helper = new AppSettingsHelper_Accessor();
            helper.SetGetter(key => settingValue);

            // コントローラーの SettingsHelper をカスタマイズした AppSettingsHelper_Accessor に設定します。
            po.SetProperty(helperName, helper);

            // ランダムな値を設定値に見立てて、取得する値が正しいことを確認します。
            testValue = System.Guid.NewGuid().ToString();
            settingValue = testValue;
            projValue = po.GetProperty(memberName) as string;
            Assert.AreSame(projValue, testValue);

            // Get() を加工しない状態で動作を確認します。
            controller = new SubmitController();
            po = new PrivateObject(controller);
            projValue = po.GetProperty(memberName) as string;
            testValue = ConfigurationManager.AppSettings.Get("FileUploadPath");
            Assert.AreSame(projValue, testValue);
        }

        [TestMethod]
        public void UploadRetryTimesTest()
        {
            // プロジェクトとテストの比較のための変数です。
            string memberName = "UploadRetryTimes";
            string helperName = "SettingsHelper";
            int projValue = 0, testValue = 0;
            string settingValue = string.Empty;

            // コントローラーを作成します。
            var controller = new SubmitController();

            // プライベート オブジェクトを作成します。
            PrivateObject po = new PrivateObject(controller);

            // settingValue を返すようにカスタマイズした AppSettingsHelper_Accessor を作成します。
            var helper = new AppSettingsHelper_Accessor();
            helper.SetGetter(key => settingValue);

            // コントローラーの SettingsHelper をカスタマイズした AppSettingsHelper_Accessor に設定します。
            po.SetProperty(helperName, helper);

            // ランダムな値を設定値に見立てて、取得する値が正しいことを確認します。
            testValue = new Random().Next();
            settingValue = testValue.ToString();
            projValue = (int)po.GetProperty(memberName);
            Assert.AreEqual(projValue, testValue);

            // Get() を加工しない状態で動作を確認します。
            controller = new SubmitController();
            po = new PrivateObject(controller);
            projValue = (int)po.GetProperty(memberName);
            testValue = Convert.ToInt32(ConfigurationManager.AppSettings.Get("UploadRetryTimes"));
            Assert.AreEqual(projValue, testValue);
        }

        [TestMethod]
        public void AllowUploadingMimeFilterTest()
        {
            // プロジェクトとテストの比較のための変数です。
            string memberName = "AllowUploadingMimeFilter";
            string helperName = "SettingsHelper";
            IEnumerable<string> projValue, testValue;
            string settingValue = string.Empty;

            // コントローラーを作成します。
            var controller = new SubmitController();

            // プライベート オブジェクトを作成します。
            PrivateObject po = new PrivateObject(controller);

            // settingValue を返すようにカスタマイズした AppSettingsHelper_Accessor を作成します。
            var helper = new AppSettingsHelper_Accessor();
            helper.SetGetter(key => settingValue);

            // コントローラーの SettingsHelper をカスタマイズした AppSettingsHelper_Accessor に設定します。
            po.SetProperty(helperName, helper);

            // ひとつの状態を確認します。
            settingValue = "abc";
            testValue = new string[] { "abc" };
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            Assert.IsTrue(CompareStrings(projValue, testValue));

            // 複数の状態を確認します。
            settingValue = "aaa;bbb;ccc";
            testValue = new string[] { "aaa", "bbb", "ccc" };
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            Assert.IsTrue(CompareStrings(projValue, testValue));

            // 個々の要素にトリムがかからないことを確認します。
            settingValue = " 111 ; 222 ; 333 ";
            testValue = new string[] { "111", "222", "333" };
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            Assert.IsFalse(CompareStrings(projValue, testValue));

            // キーがない場合を確認します。
            settingValue = null;
            //testValue = new string[] { };
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            Assert.IsNull(projValue);

            // Get() を加工しない状態で動作を確認します。
            controller = new SubmitController();
            po = new PrivateObject(controller);
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            var strings = new List<string>();
            var tmpValue = ConfigurationManager.AppSettings.Get("AllowUploadingMimeFilter");
            tmpValue = (tmpValue ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(tmpValue))
            {
                tmpValue.Split(';').ToList().ForEach(s =>
                {
                    strings.Add(s.Trim());
                });
            }
            testValue = strings;
            Assert.IsTrue(CompareStrings(projValue, testValue));
        }

        [TestMethod]
        public void DenyUploadingMimeFilterTest()
        {
            // プロジェクトとテストの比較のための変数です。
            string memberName = "DenyUploadingMimeFilter";
            string helperName = "SettingsHelper";
            IEnumerable<string> projValue, testValue;
            string settingValue = string.Empty;

            // コントローラーを作成します。
            var controller = new SubmitController();

            // プライベート オブジェクトを作成します。
            PrivateObject po = new PrivateObject(controller);

            // settingValue を返すようにカスタマイズした AppSettingsHelper_Accessor を作成します。
            var helper = new AppSettingsHelper_Accessor();
            helper.SetGetter(key => settingValue);

            // コントローラーの SettingsHelper をカスタマイズした AppSettingsHelper_Accessor に設定します。
            po.SetProperty(helperName, helper);

            // ひとつの状態を確認します。
            settingValue = "abc";
            testValue = new string[] { "abc" };
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            Assert.IsTrue(CompareStrings(projValue, testValue));

            // 複数の状態を確認します。
            settingValue = "aaa;bbb;ccc";
            testValue = new string[] { "aaa", "bbb", "ccc" };
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            Assert.IsTrue(CompareStrings(projValue, testValue));

            // 個々の要素にトリムがかからないことを確認します。
            settingValue = " 111 ; 222 ; 333 ";
            testValue = new string[] { "111", "222", "333" };
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            Assert.IsFalse(CompareStrings(projValue, testValue));

            // キーがない場合を確認します。
            settingValue = null;
            //testValue = new string[] { };
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            Assert.IsNull(projValue);

            // Get() を加工しない状態で動作を確認します。
            controller = new SubmitController();
            po = new PrivateObject(controller);
            projValue = po.GetProperty(memberName) as IEnumerable<string>;
            var strings = new List<string>();
            var tmpValue = ConfigurationManager.AppSettings.Get("DenyUploadingMimeFilter");
            tmpValue = (tmpValue ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(tmpValue))
            {
                tmpValue.Split(';').ToList().ForEach(s =>
                {
                    strings.Add(s.Trim());
                });
            }
            testValue = strings;
            Assert.IsTrue(CompareStrings(projValue, testValue));
        }

        [TestMethod]
        public void UploadTest()
        {
            // 構成ファイルから Server.MapPath でマッピングするサイトパスを取得します。
            var sitePath = ConfigurationManager.AppSettings.Get("SitePath");
            Assert.IsNotNull(sitePath, "単体テストのアプリケーション構成ファイルからサイトのパス SitePath が取得できませんでした。");
            var siteDirInfo = new DirectoryInfo(sitePath);
            Assert.IsTrue(siteDirInfo.Exists, "単体テストのアプリケーション構成ファイルから取得したサイトのパス SitePath に該当するフォルダーが存在しません。");
            var siteFullName = siteDirInfo.FullName;
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(siteFullName, @"\\$"), "単体テストのアプリケーション構成ファイルから取得したサイトのパス SitePath の末尾に区切り文字 \"\\\" がありません。");
            Console.WriteLine("サイトのパス：[\"{0}\"]", siteFullName);

            // Server.MapPath を使用するために HttpServerUtilityBase モックを作成します。
            var mockServer = new Mock<HttpServerUtilityBase>();
            mockServer.Setup(server => server.MapPath(It.IsAny<string>())).Returns((string path) =>
            {
                return path.Replace("~/", siteFullName);
            });

            // HttpServerUtilityBase モックを使えるように、HttpContextBase モックを作ります。
            var mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(context => context.Server).Returns(mockServer.Object);

            // コントローラーを作ります。
            var controller = new SubmitController();

            // HttpContextBase モックを使えるように、ControllerContext を作ります。
            var routeData = new RouteData();
            var controllerContext = new ControllerContext(mockContext.Object, routeData, controller);
            controller.ControllerContext = controllerContext;

            // 配列で情報を与えられるように、HttpPostedFileBase のリストを作ります。
            var files = new List<HttpPostedFileBase>();

            // テスト素材フォルダー内のファイル名配列を取得します。
            var fixturesPath = ConfigurationManager.AppSettings.Get("FixturesPath");
            var names = Directory.GetFiles(fixturesPath);

            // ファイル名を順次アクセスして HttpPostedFileBase モックを作ります。
            names.ToList().ForEach(name =>
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

            // アクションを起動します。
            ActionResult result = controller.Upload(files.ToArray()) as ActionResult;

            // ファイル ストリームを閉じ、リストを空にします。
            files.ToList().ForEach(file =>
            {
                file.InputStream.Dispose();
            });
            files.Clear();
        }

        #endregion
    }
}
