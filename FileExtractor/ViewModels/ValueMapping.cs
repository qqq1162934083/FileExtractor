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
    public class ValueMapping : INotifyPropertyChanged
    {
        /// <summary>
        /// 排序使用
        /// </summary>
        [JsonIgnore]
        public int No
        {
            get => _no;
            set => HandleSetValue(nameof(No), nameof(_no), value);
        }
        [JsonIgnore]
        private int _no;


        public string VarName
        {
            get => _varName;
            set => HandleSetValue(nameof(VarName), nameof(_varName), value);
        }
        [JsonIgnore]
        private string _varName;


        public string VarValue
        {
            get => _varValue;
            set => HandleSetValue(nameof(VarValue), nameof(_varValue), value);
        }
        [JsonIgnore]
        private string _varValue;

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
