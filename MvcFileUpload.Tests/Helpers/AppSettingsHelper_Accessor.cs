using System;
using MvcFileUpload.Helpers;

namespace MvcFileUpload.Tests.Helpers
{
    public class AppSettingsHelper_Accessor : AppSettingsHelper
    {
        /// <summary>
        /// 新しい AppSettingsHelper_Accessor クラス インスタンスを初期化しします。
        /// 基本クラスの Get(string) メソッドを Get_Accessor(string) クラスの既定の処理とします。
        /// </summary>
        public AppSettingsHelper_Accessor()
            : base()
        {
            this.ResetGetter();
        }

        /// <summary>
        /// Get(string) メソッドの動作をカスタマイズするためのデリゲートを取得または設定します。
        /// </summary>
        private Func<string, string> Getter { get; set; }

        /// <summary>
        /// 基本クラスの Get(string) ではなく、Getter プロパティが示すデリゲートを呼び出します。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override string Get(string key)
        {
            return (this.Getter)(key);
        }

        /// <summary>
        /// 指定されたデリゲートまたはラムダ式を Getter プロパティに設定し、Get(string) メソッドの動作をカスタマイズします。
        /// </summary>
        /// <param name="getter">Get(string) メソッドの処理内容とするデリゲートまたはラムダ式。</param>
        /// <returns>メソッドチェーンのために自分自身を返します。</returns>
        public AppSettingsHelper_Accessor SetGetter(Func<string, string> getter)
        {
            this.Getter = getter;
            return this;
        }

        /// <summary>
        /// 基本クラスの Get() メソッドを Getter プロパティに設定し、Get(string) メソッドの動作を既定の処理とします。
        /// </summary>
        /// <returns>メソッドチェーンのために自分自身を返します。</returns>
        public AppSettingsHelper_Accessor ResetGetter()
        {
            this.Getter = base.Get;
            return this;
        }

        /// <summary>
        /// プロテクト スコープである Get(string) メソッドへの外部からのアクセッサです。
        /// 基本クラスの Get(string) ではなく、Getter プロパティが示すデリゲートを呼び出します。
        /// </summary>
        /// <param name="key">取得する値に対応するキー。</param>
        /// <returns></returns>
        public string Get_Acessor(string key)
        {
            return this.Get(key);
        }
    }
}
