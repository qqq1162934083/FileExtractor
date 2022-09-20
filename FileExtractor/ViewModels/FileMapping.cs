using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExtractor.ViewModels
{
    class FileMapping : INotifyPropertyChanged
    {
        public string SrcPath
        {
            get => _srcPath;
            set => HandleSetValue(nameof(SrcPath), _srcPath, value);
        }
        [JsonIgnore]
        private string _srcPath;


        public string DestPath
        {
            get => _destPath;
            set => HandleSetValue(nameof(DestPath), _destPath, value);
        }
        [JsonIgnore]
        private string _destPath;

        public event PropertyChangedEventHandler PropertyChanged;
        private void HandleSetValue<T>(string propName, T srcValue, T destValue)
        {
            var targetReference = srcValue;
            var hasChanged = !Equals(targetReference, destValue);
            targetReference = destValue;
            if (hasChanged) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
