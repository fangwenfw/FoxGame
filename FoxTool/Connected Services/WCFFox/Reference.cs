﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace FoxTool.WCFFox {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MarketModel", Namespace="http://schemas.datacontract.org/2004/07/FoxModel")]
    [System.SerializableAttribute()]
    public partial class MarketModel : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime AddtimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string[] CallsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal CpriceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal EndpriceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime EndtimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int FightField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int GeneationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GenesField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int GrowField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int LuckyField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PictureField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal StartpriceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TokenidField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Addtime {
            get {
                return this.AddtimeField;
            }
            set {
                if ((this.AddtimeField.Equals(value) != true)) {
                    this.AddtimeField = value;
                    this.RaisePropertyChanged("Addtime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string[] Calls {
            get {
                return this.CallsField;
            }
            set {
                if ((object.ReferenceEquals(this.CallsField, value) != true)) {
                    this.CallsField = value;
                    this.RaisePropertyChanged("Calls");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Cprice {
            get {
                return this.CpriceField;
            }
            set {
                if ((this.CpriceField.Equals(value) != true)) {
                    this.CpriceField = value;
                    this.RaisePropertyChanged("Cprice");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Endprice {
            get {
                return this.EndpriceField;
            }
            set {
                if ((this.EndpriceField.Equals(value) != true)) {
                    this.EndpriceField = value;
                    this.RaisePropertyChanged("Endprice");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Endtime {
            get {
                return this.EndtimeField;
            }
            set {
                if ((this.EndtimeField.Equals(value) != true)) {
                    this.EndtimeField = value;
                    this.RaisePropertyChanged("Endtime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Fight {
            get {
                return this.FightField;
            }
            set {
                if ((this.FightField.Equals(value) != true)) {
                    this.FightField = value;
                    this.RaisePropertyChanged("Fight");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Geneation {
            get {
                return this.GeneationField;
            }
            set {
                if ((this.GeneationField.Equals(value) != true)) {
                    this.GeneationField = value;
                    this.RaisePropertyChanged("Geneation");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Genes {
            get {
                return this.GenesField;
            }
            set {
                if ((object.ReferenceEquals(this.GenesField, value) != true)) {
                    this.GenesField = value;
                    this.RaisePropertyChanged("Genes");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Grow {
            get {
                return this.GrowField;
            }
            set {
                if ((this.GrowField.Equals(value) != true)) {
                    this.GrowField = value;
                    this.RaisePropertyChanged("Grow");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Lucky {
            get {
                return this.LuckyField;
            }
            set {
                if ((this.LuckyField.Equals(value) != true)) {
                    this.LuckyField = value;
                    this.RaisePropertyChanged("Lucky");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Picture {
            get {
                return this.PictureField;
            }
            set {
                if ((object.ReferenceEquals(this.PictureField, value) != true)) {
                    this.PictureField = value;
                    this.RaisePropertyChanged("Picture");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Startprice {
            get {
                return this.StartpriceField;
            }
            set {
                if ((this.StartpriceField.Equals(value) != true)) {
                    this.StartpriceField = value;
                    this.RaisePropertyChanged("Startprice");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Tokenid {
            get {
                return this.TokenidField;
            }
            set {
                if ((this.TokenidField.Equals(value) != true)) {
                    this.TokenidField = value;
                    this.RaisePropertyChanged("Tokenid");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
