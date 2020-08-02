﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace BishojoGameLauncher.Configuration {
    /// <summary>ポータブルな設定ファイルの機能を提供する基本クラス。</summary>
    /// <typeparam name="T">設定情報を格納するクラス。</typeparam>
    [DataContract, Serializable]
    public abstract class SettingsBase<T> : INotifyPropertyChanged, IExtensibleDataObject
        where T : SettingsBase<T> {
        #region Fields

        /// <summary>インスタンスメンバーへのアクセスをロックするかどうか。</summary>
        [NonSerialized]
        private bool _isSynchronized;

        /// <summary>読み込んだ設定ファイルのパス。</summary>
        [NonSerialized]
        private string _loadedFilePath;

        /// <summary>設定ファイルのシリアライズ・デシリアライズの機能を提供するクラス。</summary>
        [NonSerialized]
        private ISettingsSerializer<T> _serializer;

        /// <summary>プロパティの名前と値を格納するコレクション。</summary>
        [NonSerialized]
        private Dictionary<string, SettingsProperty> _properties;

        /// <summary>新しいメンバーの追加によって拡張されたデータを格納するためのコンテナ。</summary>
        [NonSerialized]
        private ExtensionDataObject _extensionData;

        /// <summary>デシリアライズが完了しているかどうかを示すフラグ。</summary>
        [NonSerialized]
        private bool _isLoaded;

        #endregion

        #region Property

        /// <summary>プロパティ情報を取得します。</summary>
        /// <param name="name">プロパティ名。</param>
        /// <returns>プロパティ情報を格納する SettingsProperty。</returns>
        public virtual SettingsProperty this[string name] {
            get { return GetProperties()[name]; }
        }

        /// <summary>コレクションを反復処理する列挙子を返します。</summary>
        public IEnumerator<SettingsProperty> GetEnumerator() {
            return GetProperties().Values.GetEnumerator();
        }

        /// <summary>インスタンスメンバーへのアクセスをロックするかどうかを取得または設定します。</summary>
        public virtual bool IsSynchronized {
            get { return _isSynchronized; }
            set { _isSynchronized = value; }
        }

        #endregion

        #region EventHandler

        public delegate void PropertyChangingEventHandler(object sender, PropertyChangingEventArgs args);

        [NonSerialized]
        private PropertyChangingEventHandler _propertyChanging;

        /// <summary>プロパティが変更される直前に発生します。</summary>
        public event PropertyChangingEventHandler PropertyChanging {
            add { _propertyChanging += value; }
            remove { _propertyChanging -= value; }
        }

        /// <summary>PropertyChanging イベントを発生させます。</summary>
        /// <param name="args">イベント情報。</param>
        protected virtual void OnPropertyChanging(PropertyChangingEventArgs args) {
            if (_propertyChanging != null) {
                _propertyChanging(this, args);
            }
        }

        [NonSerialized]
        private PropertyChangedEventHandler _propertyChanged;

        /// <summary>プロパティが変更された直後に発生します。</summary>
        public event PropertyChangedEventHandler PropertyChanged {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        /// <summary>PropertyChanged イベントを発生させます。</summary>
        /// <param name="args">イベント情報。</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) {
            if (_propertyChanged != null) {
                _propertyChanged(this, args);
            }
        }

        [NonSerialized]
        private EventHandler _loaded;

        /// <summary>設定を読み込み終った直後に発生します。</summary>
        public event EventHandler Loaded {
            add { _loaded += value; }
            remove { _loaded -= value; }
        }

        /// <summary>Loaded イベントを発生させます。</summary>
        /// <param name="args">イベント情報。</param>
        protected virtual void OnLoaded(EventArgs args) {
            if (_loaded != null) {
                _loaded(this, args);
            }
        }

        [NonSerialized]
        private CancelEventHandler _saving;

        /// <summary>設定を保存する直前に発生します。</summary>
        public event CancelEventHandler Saving {
            add { _saving += value; }
            remove { _saving -= value; }
        }

        /// <summary>Saving イベントを発生させます。</summary>
        /// <param name="args">イベント情報。</param>
        protected virtual void OnSaving(CancelEventArgs args) {
            if (_saving != null) {
                _saving(this, args);
            }
        }

        #endregion

        #region Get / Set

        /// <summary>プロパティの値を取得します。</summary>
        /// <typeparam name="TValue">プロパティの型。</typeparam>
        /// <param name="expression">プロパティを参照するラムダ式 (a => a.Property)。</param>
        /// <returns>プロパティから取得した値。</returns>
        protected virtual TValue Get<TValue>(Expression<Func<T, TValue>> expression) {
            if (IsSynchronized) {
                lock (this) {
                    return GetPropertyValue(expression);
                }
            } else {
                return GetPropertyValue(expression);
            }
        }

        /// <summary>プロパティの値を設定します。</summary>
        /// <typeparam name="TValue">プロパティの型。</typeparam>
        /// <param name="expression">プロパティを参照するラムダ式 (a => a.Property)。</param>
        /// <param name="value">プロパティに設定する値。</param>
        protected virtual void Set<TValue>(Expression<Func<T, TValue>> expression, TValue value) {
            if (IsSynchronized) {
                lock (this) {
                    SetPropertyValue(expression, value);
                }
            } else {
                SetPropertyValue(expression, value);
            }
        }

        /// <summary>プロパティの値を取得します。</summary>
        /// <typeparam name="TValue">プロパティの型。</typeparam>
        /// <param name="expression">プロパティを参照するラムダ式 (a => a.Property)。</param>
        /// <returns>プロパティから取得した値。</returns>
        private TValue GetPropertyValue<TValue>(Expression<Func<T, TValue>> expression) {
            var member = GetMemberInfo(expression);
            if (member.MemberType != MemberTypes.Property) {
                throw new ArgumentException("expression がプロパティの参照式ではありません。");
            }

            var name = member.Name;
            if (GetProperties().ContainsKey(member.Name) == false) {
                GetProperties().Add(name, new SettingsProperty(member as PropertyInfo, default(TValue)));
            }

            return (TValue)GetProperties()[name].Value;
        }

        /// <summary>プロパティの値を設定します。</summary>
        /// <typeparam name="TValue">プロパティの型。</typeparam>
        /// <param name="expression">プロパティを参照するラムダ式 (a => a.Property)。</param>
        /// <param name="value">プロパティに設定する値。</param>
        private void SetPropertyValue<TValue>(Expression<Func<T, TValue>> expression, TValue value) {
            var member = GetMemberInfo(expression);
            if (member.MemberType != MemberTypes.Property) {
                throw new ArgumentException("expression がプロパティの参照式ではありません。");
            }

            var name = member.Name;
            if (GetProperties().ContainsKey(member.Name) == false) {
                GetProperties().Add(name, new SettingsProperty(member as PropertyInfo, default(TValue)));
            }

            var prop = GetProperties()[name];
            var args = new PropertyChangingEventArgs(prop, value);
            OnPropertyChanging(args);
            if (args.Cancel) {
                return;
            }

            prop.Value = value;
            prop.IsDirty = _isLoaded;
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        /// <summary>ラムダ式で参照しているプロパティ情報を取得します。</summary>
        /// <typeparam name="TValue">プロパティの型。</typeparam>
        /// <param name="expression">プロパティを参照するラムダ式 (a => a.Property)。</param>
        /// <returns>プロパティ情報。</returns>
        private MemberInfo GetMemberInfo<TValue>(Expression<Func<T, TValue>> expression) {
            return (expression.Body as MemberExpression).Member;
        }

        /// <summary>プロパティの名前と値を格納するコレクションを取得します。</summary>
        private Dictionary<string, SettingsProperty> GetProperties() {
            if (_properties == null) {
                _properties = new Dictionary<string, SettingsProperty>();
            }
            return _properties;
        }

        #endregion

        #region Load / Save

        /// <summary>指定の設定ファイルからデシリアライズしてインスタンスを取得します。</summary>
        /// <param name="path">設定ファイルのパス。省略した場合は SettingsPathAttribute の既定値を使用します。</param>
        /// <param name="serializer">デシリアライズに使用するクラス。省略した場合は SettingsXmlSerializer を使用します。</param>
        /// <returns>デシリアライズしたインスタンス。</returns>
        protected static T Load(string path = null, ISettingsSerializer<T> serializer = null) {
            path = path ?? GetSettingsFilePath();
            serializer = serializer ?? new SettingsXmlSerializer<T>();

            T self = serializer.Desilialize(path);
            if (self == null) {
                self = Activator.CreateInstance(typeof(T), true) as T;
            }

            self._loadedFilePath = path;
            self._serializer = serializer;
            self.OnLoaded(EventArgs.Empty);
            self._isLoaded = true;
            return self;
        }

        /// <summary>設定プロパティの現在の値を格納します。</summary>
        /// <param name="path">保存先のパス。省略した場合は Load メソッドで読み込んだファイルのパスを使用します。</param>
        /// <param name="serializer">シリアライズの機能を提供するクラス。省略した場合は Load メソッドで使用したクラスを使用します。</param>
        /// <returns>成功した場合は true、キャンセルした場合は false。</returns>
        public virtual bool Save(string path = null, ISettingsSerializer<T> serializer = null) {
            if (IsSynchronized) {
                lock (this) {
                    return SaveCore(path, serializer);
                }
            } else {
                return SaveCore(path, serializer);
            }
        }

        /// <summary>設定プロパティの現在の値を格納します。</summary>
        /// <param name="path">保存先のパス。</param>
        /// <param name="serializer">シリアライズの機能を提供するクラス。</param>
        /// <returns>成功した場合は true、キャンセルした場合は false。</returns>
        private bool SaveCore(string path, ISettingsSerializer<T> serializer) {
            var args = new CancelEventArgs();
            OnSaving(args);
            if (args.Cancel) {
                return false;
            }

            path = path ?? _loadedFilePath;
            serializer = serializer ?? _serializer;
            serializer.Serialize(path, this as T);
            GetProperties().Select(a => a.Value.IsDirty = false);
            return true;
        }

        /// <summary>クラスに付与されている属性から設定ファイルのパスを取得します。</summary>
        /// <returns>設定ファイルのパス。</returns>
        private static string GetSettingsFilePath() {
            Type type = typeof(T);
            var attrs = type.GetCustomAttributes(typeof(SettingsPathAttribute), false);
            var attr = (attrs.Length > 0) ? (attrs[0] as SettingsPathAttribute) : new SettingsPathAttribute();
            return attr.FullPath;
        }

        /// <summary>デシリアライズが完了しているかどうかを取得します。</summary>
        protected bool GetIsLoaded() {
            return _isLoaded;
        }

        #endregion

        #region IExtensibleDataObject

        /// <summary>新しいメンバーの追加によって拡張されたデータを格納するためのコンテナを取得または設定します。</summary>
        [IgnoreDataMember, XmlIgnore]
        ExtensionDataObject IExtensibleDataObject.ExtensionData {
            get { return _extensionData; }
            set { _extensionData = value; }
        }

        #endregion

        #region PropertyChangingEventArgs

        /// <summary>PropertyChanging イベントのイベント情報を提供します。</summary>
        public class PropertyChangingEventArgs : EventArgs {
            /// <summary>指定されたパラメーターに基づいて、PropertyChangingEventArgs クラスの新しいインスタンスを作成します。</summary>
            /// <param name="property">変更されるプロパティ情報。</param>
            /// <param name="newValue">変更後のプロパティの値。</param>
            public PropertyChangingEventArgs(SettingsProperty property, object newValue) {
                _property = property;
                NewValue = newValue;
            }

            /// <summary>変更されるプロパティ情報。</summary>
            private readonly SettingsProperty _property;

            /// <summary>変更をキャンセルするかどうかを取得または設定します。</summary>
            public bool Cancel { get; set; }

            /// <summary>プロパティ名を取得します。</summary>
            public object Name { get { return _property.Name; } }

            /// <summary>変更前のプロパティの値を取得します。</summary>
            public object Value { get { return _property.Value; } }

            /// <summary>変更後のプロパティの値を取得します。</summary>
            public object NewValue { get; private set; }
        }

        #endregion
    }

    /// <summary>SettingsBase クラスのプロパティ情報を提供します。</summary>
    [DebuggerDisplay("{DebuggerString}")]
    public class SettingsProperty {
        #region Members

        /// <summary>指定されたパラメーターに基づいて、SettingsValue クラスの新しいインスタンスを作成します。</summary>
        /// <param name="propertyInfo">プロパティ情報。</param>
        /// <param name="value">プロパティの値。</param>
        public SettingsProperty(PropertyInfo propertyInfo, object value) {
            PropertyInfo = propertyInfo;
            Name = propertyInfo.Name;
            Value = value;
            IsDirty = false;
        }

        /// <summary>プロパティ情報を取得します。</summary>
        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>プロパティ名を取得します。</summary>
        public string Name { get; private set; }

        /// <summary>プロパティの値を取得または設定します。</summary>
        public object Value { get; set; }

        /// <summary>値の更新状態を取得または設定します。</summary>
        public bool IsDirty { get; set; }

        /// <summary>デバッグ用のインスタンス文字列を取得します。</summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        private string DebuggerString {
            get { return (Value == null) ? "null" : "\"" + Value.ToString() + "\""; }
        }

        #endregion
    }

    /// <summary>そのクラスをシリアライズまたはデシリアライズするときのファイル名を指定します。</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SettingsPathAttribute : Attribute {
        #region Members

        /// <summary>SettingsFilePathAttribute クラスの新しいインスタンスを作成します。</summary>
        public SettingsPathAttribute() {
            DirectoryName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            FileName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().ProcessName) + DefaultExtension;
        }

        /// <summary>設定ファイルのあるディレクトリ名。</summary>
        public virtual string DirectoryName { get; set; }

        /// <summary>設定ファイル名。</summary>
        public virtual string FileName { get; set; }

        /// <summary>既定の拡張子。</summary>
        public virtual string DefaultExtension { get { return ".conf"; } }

        /// <summary>設定ファイルのフルパス。</summary>
        public virtual string FullPath { get { return DirectoryName + "\\" + FileName; } }

        #endregion
    }
}