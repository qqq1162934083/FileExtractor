using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.ViewModels
{
    public class FileMapping : INotifyPropertyChanged
    {
        public string SrcPath
        {
            get => _srcPath;
            set => HandleSetValue(nameof(SrcPath), nameof(_srcPath), value);
        }
        [JsonIgnore]
        private string _srcPath;


        public string DestPath
        {
            get => _destPath;
            set => HandleSetValue(nameof(DestPath), nameof(_destPath), value);
        }
        [JsonIgnore]
        private string _destPath;

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
    }
}
