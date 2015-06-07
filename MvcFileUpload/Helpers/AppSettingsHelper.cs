using System;
using System.Collections.Generic;
using System.Configuration;

namespace MvcFileUpload.Helpers
{
    public class AppSettingsHelper
    {
        #region *** Constructors and helpers.

        /// <summary>
        /// 新しい AppSettingsHelper クラス インスタンスを初期化します。
        /// </summary>
        public AppSettingsHelper()
        {
            // no code.
        }

        #endregion

        #region *** Static methods.

        /// <summary>
        /// 省略できない値が省略された場合に使用される例外メッセージを取得します。
        /// </summary>
        /// <param name="key">値とペアのキー。</param>
        /// <returns></returns>
        protected static string DisallowOmitMessage(string key)
        {
            var message = string.Format("構成ファイルのキー {0} で指定する値は省略できませんが、構成ファイルの中に見つかりませんでした。", key);
            return message;
        }

        /// <summary>
        /// 取得した値が指定された型に変換できない場合に使用される例外メッセージを取得します。
        /// </summary>
        /// <param name="key">値とペアのキー。</param>
        /// <param name="type">指定された型。</param>
        /// <returns></returns>
        protected static string TypeMissingMessage(string key, Type type)
        {
            var message = string.Format("構成ファイルのキー {0} で指定する値は {1} 型ですが、{1} 型に評価できませんでした。", key, type.ToString());
            return message;
        }

        #endregion

        #region *** Cupsuled members.

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する値を取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// キーが見つからなかったときは null が返されます。
        /// </returns>
        protected virtual string Get(string key)
        {
            var settingValue = ConfigurationManager.AppSettings.Get(key);
            return settingValue;
        }

        #endregion

        #region *** Public methods.

        #region *** Get the value as bool.

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する省略可能な値を bool? 型で取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// 値が "true" のときは true が返されます（大文字小文字は無視されます）。
        /// 値が "false" のときは false が返されます（大文字小文字は無視されます）。
        /// 値が "true" や "false" でなかったときは ConfigurationErrorsException 例外をスローします。
        /// キーが見つからなかったときは null が返されます。
        /// </returns>
        public virtual bool? GetAsOptionalBoolean(string key)
        {
            var value = this.Get(key);
            if (value == null) return null;
            var isTrue = (value == "true") || (value == "True");
            var isFalse = (value == "false") || (value == "False");
            if (isTrue == isFalse) throw new ConfigurationErrorsException(TypeMissingMessage(key, typeof(bool)));
            return isTrue;
        }

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する省略できない値を bool 型で取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// 値が "true" のときは true が返されます（大文字小文字は無視されます）。
        /// 値が "false" のときは false が返されます（大文字小文字は無視されます）。
        /// 値が "true" や "false" でなかったときは ConfigurationErrorsException 例外をスローします。
        /// キーが見つからなかったときは、ConfigurationErrorsException 例外をスローします。
        /// </returns>
        public virtual bool GetAsBoolean(string key)
        {
            var value = this.GetAsOptionalBoolean(key);
            if (value == null) throw new ConfigurationErrorsException(DisallowOmitMessage(key)); 
            return value == true;
        }

        #endregion

        #region *** Get the value as int32.

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する省略可能な値を int? 型で取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// 値が int32 型に評価できなかったときは ConfigurationErrorsException 例外をスローします。
        /// キーが見つからなかったときは null が返されます。
        /// </returns>
        public virtual int? GetAsOptionalInt32(string key)
        {
            var value = this.Get(key);
            if (value == null) return null;
            value = (value ?? string.Empty).ToLower();
            var typedValue = 0;
            try
            {
                typedValue = Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(TypeMissingMessage(key, typeof(int)), ex);
            }
            return typedValue;
        }

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する省略できない値を int 型で取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// 値が int32 型に評価できなかったときは ConfigurationErrorsException 例外をスローします。
        /// キーが見つからなかったときは、ConfigurationErrorsException 例外をスローします。
        /// </returns>
        public virtual int GetAsInt32(string key)
        {
            var value = GetAsOptionalInt32(key);
            if (value == null) throw new ConfigurationErrorsException(DisallowOmitMessage(key));
            return (int)value;
        }

        #endregion

        #region *** Get the value as string.

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する省略可能な値を string 型で取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// キーが見つからなかったときは null が返されます。
        /// </returns>
        public virtual string GetAsOptionalString(string key)
        {
            var value = this.Get(key);
            return value;
        }

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する省略できない値を string 型で取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// キーが見つからなかったときは、ConfigurationErrorsException 例外をスローします。
        /// </returns>
        public virtual string GetAsString(string key)
        {
            var value = this.GetAsOptionalString(key);
            if (value == null) throw new ConfigurationErrorsException(DisallowOmitMessage(key));
            return value;
        }

        #endregion

        #region *** Get the value as IEnumerable<string>.

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する省略可能な値を IEnumerable&lt;string&gt; 型で取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// キーが見つからなかったときは null が返されます。
        /// </returns>
        public virtual IEnumerable<string> GetAsOptionalEnumerableString(string key, char separator)
        {
            IEnumerable<string> typedValue = null;
            var value = this.Get(key);
            if (value != null)
            {
                typedValue = new List<string>();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    typedValue = value.Split(separator);
                }
            }
            return typedValue;
        }

        /// <summary>
        /// 構成ファイルから、指定したキーに対応する省略できない値を IEnumerable&lt;string&gt; 型で取得します。
        /// </summary>
        /// <param name="key">構成ファイルから取得する値に対応するキー。</param>
        /// <returns>
        /// 構成ファイルで、指定されたキーに対して設定されている値が評価されます。
        /// キーが見つからなかったときは ConfigurationErrorsException 例外をスローします。
        /// </returns>
        public virtual IEnumerable<string> GetAsEnumerableString(string key, char separator)
        {
            var value = this.GetAsOptionalEnumerableString(key, separator);
            if (value == null) throw new ConfigurationErrorsException(DisallowOmitMessage(key));
            return value;
        }

        #endregion

        #endregion
    }
}
