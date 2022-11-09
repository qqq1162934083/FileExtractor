using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.ViewModels
{
    public class ConfigData : INotifyPropertyChanged
    {
        public BindingList<FileMapping> FileMappingList
        {
            get
            {
                if (_fileMappingList == null) _fileMappingList = new BindingList<FileMapping>();
                return _fileMappingList;
            }
            set => HandleSetValue(nameof(FileMappingList), nameof(_fileMappingList), value);
        }
        [JsonIgnore]
        private BindingList<FileMapping> _fileMappingList;


        public BindingList<ValueMapping> ValueMappingList
        {
            get
            {
                if (_valueMappingList == null) _valueMappingList = new BindingList<ValueMapping>();
                return _valueMappingList;
            }
            set => HandleSetValue(nameof(ValueMappingList), nameof(_valueMappingList), value);
        }
        [JsonIgnore]
        private BindingList<ValueMapping> _valueMappingList;


        public BindingList<DirMapping> DirMappingList
        {
            get
            {
                if (_dirMappingList == null) _dirMappingList = new BindingList<DirMapping>();
                return _dirMappingList;
            }
            set => HandleSetValue(nameof(DirMappingList), nameof(_dirMappingList), value);
        }
        [JsonIgnore]
        private BindingList<DirMapping> _dirMappingList;



        public bool EnabledDateTimeExpression
        {
            get => _enabledDateTimeExpression;
            set => HandleSetValue(nameof(EnabledDateTimeExpression), nameof(_enabledDateTimeExpression), value);
        }
        [JsonIgnore]
        private bool _enabledDateTimeExpression;


        public bool EnabledPackageDirFtpSupport
        {
            get => _enabledPackageDirFtpSupport;
            set => HandleSetValue(nameof(EnabledPackageDirFtpSupport), nameof(_enabledPackageDirFtpSupport), value);
        }
        [JsonIgnore]
        private bool _enabledPackageDirFtpSupport;



        public bool EnabledCompress
        {
            get => _enabledCompress;
            set => HandleSetValue(nameof(EnabledCompress), nameof(_enabledCompress), value);
        }
        [JsonIgnore]
        private bool _enabledCompress;



        public string PackageName
        {
            get => _packageName;
            set => HandleSetValue(nameof(PackageName), nameof(_packageName), value);
        }
        [JsonIgnore]
        private string _packageName;



        public string PackageDir
        {
            get => _packageDir;
            set => HandleSetValue(nameof(PackageDir), nameof(_packageDir), value);
        }
        [JsonIgnore]
        private string _packageDir;

        public ConfigData()
        {
            _dirMappingList = new BindingList<DirMapping>();
            _fileMappingList = new BindingList<FileMapping>();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void HandleSetValue<T>(string propName, string srcMemberName, T destValue)
        {
            //获取成员信息
            var type = GetType();
            T srcValue;
            var srcFieldInfo = (FieldInfo)null;
            var srcPropInfo = (PropertyInfo)null;
            srcFieldInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(x => x.Name == srcMemberName);
            if (srcFieldInfo != null)
                srcValue = (T)srcFieldInfo.GetValue(this);
            else
            {
                srcPropInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(x => x.Name == srcMemberName);
                if (srcPropInfo == null) throw new Exception("找不到成员 " + srcMemberName);
                srcValue = (T)srcPropInfo.GetValue(this);
            }

            var hasChanged = !Equals(srcValue, destValue);
            if (hasChanged)
            {
                if (srcFieldInfo != null)
                    srcFieldInfo.SetValue(this, destValue);
                else
                    srcPropInfo.SetValue(this, destValue);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        public void NotifyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
