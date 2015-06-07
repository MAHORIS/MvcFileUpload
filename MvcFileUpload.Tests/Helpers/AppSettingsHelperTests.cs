using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using MvcFileUpload.Helpers;

namespace MvcFileUpload.Tests.Helpers
{
    [TestClass]
    public class AppSettingsHelperTests
    {
        #region *** tool method.

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

        #region *** Test method.

        [TestMethod]
        public void DefaultConstructorTest()
        {
            // インスタンスを作成します。
            var instance = new AppSettingsHelper();
        }

        [TestMethod]
        public void GetTest()
        {
            // プロジェクトとテストの比較のための変数です。
            string projValue, testValue;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから任意の値を取得して、テスト側の構成ファイルの値と比較します。
            ConfigurationManager.AppSettings.AllKeys.ToList().ForEach(key =>
            {
                // Get_Acessor() の戻り値と、テスト プロジェクトの構成ファイルの値を比較します。
                projValue = instance.Get_Acessor(key);
                testValue = ConfigurationManager.AppSettings.Get(key);
                Assert.AreSame(projValue, testValue);
            });
        }

        [TestMethod]
        public void GetAsBooleanTest()
        {
            // プロジェクトとテストの比較のための変数です。
            bool projValue, testValue;
            string settingValue = string.Empty;
            Type expectedExceptionType = null;
            bool caughtExpectedException;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから値を取得する処理を変更します。
            // GetAsBoolean メソッド呼び出しの第一引数がなんであれ、settingValue を返します。
            instance.SetGetter(key => settingValue);

            // 構成ファイル想定値で "true" を取得させて true を確認します。
            testValue = true;
            settingValue = testValue.ToString().ToLower();
            projValue = instance.GetAsBoolean(string.Empty);
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で "false" を取得させて false を確認します。
            testValue = false;
            settingValue = testValue.ToString().ToLower();
            projValue = instance.GetAsBoolean(string.Empty);
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で null でなくて true でも false でもなければ ConfigurationErrorsException 例外をスローします。
            testValue = true;
            settingValue = " " + testValue.ToString().ToLower() + " ";
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = instance.GetAsBoolean(string.Empty);
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

            // 構成ファイル想定値で null を取得させて（キーがない）ConfigurationErrorsException 例外をスローします。
            settingValue = null;
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = instance.GetAsBoolean(string.Empty);
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
        }

        [TestMethod]
        public void GetAsOptionalBooleanTest()
        {
            // プロジェクトとテストの比較のための変数です。
            bool? projValue, testValue;
            string settingValue = string.Empty;
            Type expectedExceptionType = null;
            bool caughtExpectedException;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから値を取得する処理を変更します。
            // GetAsBoolean メソッド呼び出しの第一引数がなんであれ、settingValue を返します。
            instance.SetGetter(key => settingValue);

            // 構成ファイル想定値で "true" を取得させて true を確認します。
            testValue = true;
            settingValue = testValue.ToString().ToLower();
            projValue = instance.GetAsOptionalBoolean(string.Empty) as bool?;
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で "false" を取得させて false を確認します。
            testValue = false;
            settingValue = testValue.ToString().ToLower();
            projValue = instance.GetAsOptionalBoolean(string.Empty) as bool?;
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で null でなくて true でも false でもなければ ConfigurationErrorsException 例外をスローします。
            testValue = true;
            settingValue = " " + testValue.ToString().ToLower() + " ";
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = instance.GetAsOptionalBoolean(string.Empty) as bool?;
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

            // 構成ファイル想定値で null を取得させて（キーがない）null を確認します。
            settingValue = null;
            projValue = instance.GetAsOptionalBoolean(string.Empty) as bool?;
            Assert.IsNull(projValue);
        }

        [TestMethod]
        public void GetAsInt32Test()
        {
            // プロジェクトとテストの比較のための変数です。
            int projValue, testValue;
            string settingValue = string.Empty;
            Type expectedExceptionType = null;
            bool caughtExpectedException;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから値を取得する処理を変更します。
            // GetAsBoolean メソッド呼び出しの第一引数がなんであれ、settingValue を返します。
            instance.SetGetter(key => settingValue);

            // 構成ファイル想定値で "123" を取得させて 123 を確認します。
            testValue = 123;
            settingValue = testValue.ToString().ToLower();
            projValue = instance.GetAsInt32(string.Empty);
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で "-456" を取得させて -456 を確認します。
            testValue = -456;
            settingValue = testValue.ToString().ToLower();
            projValue = instance.GetAsInt32(string.Empty);
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で null でなくて int32 でもなければ ConfigurationErrorsException 例外をスローします。
            testValue = 0;
            settingValue = " abcdefg ";
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = instance.GetAsInt32(string.Empty);
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

            // 構成ファイル想定値で null を取得させて（キーがない）ConfigurationErrorsException 例外をスローします。
            settingValue = null;
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = instance.GetAsInt32(string.Empty);
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
        }

        [TestMethod]
        public void GetAsOptionalInt32Test()
        {
            // プロジェクトとテストの比較のための変数です。
            int? projValue, testValue;
            string settingValue = string.Empty;
            Type expectedExceptionType = null;
            bool caughtExpectedException;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから値を取得する処理を変更します。
            // GetAsBoolean メソッド呼び出しの第一引数がなんであれ、settingValue を返します。
            instance.SetGetter(key => settingValue);

            // 構成ファイル想定値で "135" を取得させて 135 を確認します。
            testValue = 135;
            settingValue = testValue.ToString().ToLower();
            projValue = instance.GetAsOptionalInt32(string.Empty) as int?;
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で "-246" を取得させて -246 を確認します。
            testValue = -246;
            settingValue = testValue.ToString().ToLower();
            projValue = instance.GetAsOptionalInt32(string.Empty) as int?;
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で null でなくて int32 でもなければ ConfigurationErrorsException 例外をスローします。
            testValue = 0;
            settingValue = " abcdefg ";
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = instance.GetAsOptionalInt32(string.Empty) as int?;
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

            // 構成ファイル想定値で null を取得させて（キーがない）null を確認します。
            settingValue = null;
            projValue = instance.GetAsOptionalInt32(string.Empty) as int?;
            Assert.IsNull(projValue);
        }

        [TestMethod]
        public void GetAsStringTest()
        {
            // プロジェクトとテストの比較のための変数です。
            string projValue, testValue;
            string settingValue = string.Empty;
            Type expectedExceptionType = null;
            bool caughtExpectedException;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから値を取得する処理を変更します。
            // GetAsString メソッド呼び出しの第一引数がなんであれ、settingValue を返します。
            instance.SetGetter(key => settingValue);

            // 構成ファイル想定値で任意の文字列を取得させて返却値を確認します。
            settingValue = System.Guid.NewGuid().ToString();
            testValue = settingValue;
            projValue = instance.GetAsString(string.Empty) as string;
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で null を取得させて（キーがない）ConfigurationErrorsException 例外をスローします。
            settingValue = null;
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = instance.GetAsString(string.Empty) as string;
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
        }

        [TestMethod]
        public void GetAsOptionalStringTest()
        {
            // プロジェクトとテストの比較のための変数です。
            string projValue, testValue;
            string settingValue = string.Empty;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから値を取得する処理を変更します。
            // GetAsString メソッド呼び出しの第一引数がなんであれ、settingValue を返します。
            instance.SetGetter(key => settingValue);

            // 構成ファイル想定値で任意の文字列を取得させて返却値を確認します。
            settingValue = System.Guid.NewGuid().ToString();
            testValue = settingValue;
            projValue = instance.GetAsOptionalString(string.Empty) as string;
            Assert.AreEqual(projValue, testValue);

            // 構成ファイル想定値で null を取得させて（キーがない）null を確認します。
            settingValue = null;
            projValue = instance.GetAsOptionalString(string.Empty) as string;
            Assert.IsNull(projValue);
        }

        [TestMethod]
        public void GetAsEnumerableStringTest()
        {
            // プロジェクトとテストの比較のための変数です。
            IEnumerable<string> projValue, testValue;
            string settingValue = string.Empty;
            Type expectedExceptionType = null;
            bool caughtExpectedException;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから値を取得する処理を変更します。
            // GetAsEnumerableString メソッド呼び出しの第一引数がなんであれ、settingValue を返します。
            instance.SetGetter(key => settingValue);

            // 構成ファイル想定値で任意の文字列を取得させて返却値を確認します。
            settingValue = "aaa,bbb,ccc";
            testValue = settingValue.Split(',');
            projValue = instance.GetAsEnumerableString(string.Empty, ',') as IEnumerable<string>;
            Assert.IsTrue(CompareStrings(projValue, testValue));

            // 構成ファイル想定値で null を取得させて（キーがない）ConfigurationErrorsException 例外をスローします。
            settingValue = null;
            expectedExceptionType = typeof(ConfigurationErrorsException);
            caughtExpectedException = false;
            try
            {
                projValue = instance.GetAsEnumerableString(string.Empty, ';') as IEnumerable<string>;
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
        }

        [TestMethod]
        public void GetAsOptionalEnumerableStringTest()
        {
            // プロジェクトとテストの比較のための変数です。
            IEnumerable<string> projValue, testValue;
            string settingValue = string.Empty;

            // インスタンスを作成します。
            var instance = new AppSettingsHelper_Accessor();

            // 構成ファイルから値を取得する処理を変更します。
            // GetAsEnumerableString メソッド呼び出しの第一引数がなんであれ、settingValue を返します。
            instance.SetGetter(key => settingValue);

            // 構成ファイル想定値で任意の文字列を取得させて返却値を確認します。
            settingValue = "aaa;bbb;ccc";
            testValue = settingValue.Split(';');
            projValue = instance.GetAsOptionalEnumerableString(string.Empty, ';') as IEnumerable<string>;
            Assert.IsTrue(CompareStrings(projValue, testValue));

            // 構成ファイル想定値で null を取得させて（キーがない）null を確認します。
            settingValue = null;
            projValue = instance.GetAsOptionalEnumerableString(string.Empty, ';') as IEnumerable<string>;
            Assert.IsNull(projValue);
        }

        #endregion
    }
}
