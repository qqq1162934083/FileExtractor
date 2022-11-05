using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.ViewModels
{
    public class ConfigData : INotifyPropertyChanged
    {
        public BindingList<FileMapping> FileMappingList
        {
            get => _fileMappingList;
            set => HandleSetValue(nameof(FileMappingList), _fileMappingList, value);
        }
        [JsonIgnore]
        private BindingList<FileMapping> _fileMappingList;



        public BindingList<DirMapping> DirMappingList
        {
            get => _dirMappingList;
            set => HandleSetValue(nameof(DirMappingList), _dirMappingList, value);
        }
        [JsonIgnore]
        private BindingList<DirMapping> _dirMappingList;



        public bool EnabledDateTimeExpression
        {
            get => _enabledDateTimeExpression;
            set => HandleSetValue(nameof(EnabledDateTimeExpression), _enabledDateTimeExpression, value);
        }
        [JsonIgnore]
        private bool _enabledDateTimeExpression;



        public bool EnabledCompress
        {
            get => _enabledCompress;
            set => HandleSetValue(nameof(EnabledCompress), _enabledCompress, value);
        }
        [JsonIgnore]
        private bool _enabledCompress;



        public string PackageName
        {
            get => _packageName;
            set => HandleSetValue(nameof(PackageName), _packageName, value);
        }
        [JsonIgnore]
        private string _packageName;



        public string PackageDir
        {
            get => _packageDir;
            set => HandleSetValue(nameof(PackageDir), _packageDir, value);
        }
        [JsonIgnore]
        private string _packageDir;

        public ConfigData()
        {
            _dirMappingList = new BindingList<DirMapping>();
            _fileMappingList = new BindingList<FileMapping>();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void HandleSetValue<T>(string propName, T srcValue, T destValue)
        {
            var targetReference = srcValue;
            var hasChanged = !Equals(targetReference, destValue);
            targetReference = destValue;
            if (hasChanged) NotifyChanged(propName);
        }
        public void NotifyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
