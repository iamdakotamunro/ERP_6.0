﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WcfTestApp.TestForJMeter {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="StorageRecordInfo", Namespace="http://schemas.datacontract.org/2004/07/ERP.Model")]
    [System.SerializableAttribute()]
    public partial class StorageRecordInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal AccountReceivableField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime AuditTimeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BillNoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime DateCreatedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid FilialeIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LinkTradeCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid LinkTradeIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int LinkTradeTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LogisticsCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid RelevanceFilialeIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid RelevanceWarehouseIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid StockIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int StockStateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int StockTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool StockValidationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int StorageTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal SubtotalQuantityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid ThirdCompanyIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int TradeBothPartiesTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TradeCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TransactorField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid WarehouseIdField;
        
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
        public decimal AccountReceivable {
            get {
                return this.AccountReceivableField;
            }
            set {
                if ((this.AccountReceivableField.Equals(value) != true)) {
                    this.AccountReceivableField = value;
                    this.RaisePropertyChanged("AccountReceivable");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime AuditTime {
            get {
                return this.AuditTimeField;
            }
            set {
                if ((this.AuditTimeField.Equals(value) != true)) {
                    this.AuditTimeField = value;
                    this.RaisePropertyChanged("AuditTime");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BillNo {
            get {
                return this.BillNoField;
            }
            set {
                if ((object.ReferenceEquals(this.BillNoField, value) != true)) {
                    this.BillNoField = value;
                    this.RaisePropertyChanged("BillNo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DateCreated {
            get {
                return this.DateCreatedField;
            }
            set {
                if ((this.DateCreatedField.Equals(value) != true)) {
                    this.DateCreatedField = value;
                    this.RaisePropertyChanged("DateCreated");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description {
            get {
                return this.DescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.DescriptionField, value) != true)) {
                    this.DescriptionField = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid FilialeId {
            get {
                return this.FilialeIdField;
            }
            set {
                if ((this.FilialeIdField.Equals(value) != true)) {
                    this.FilialeIdField = value;
                    this.RaisePropertyChanged("FilialeId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LinkTradeCode {
            get {
                return this.LinkTradeCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.LinkTradeCodeField, value) != true)) {
                    this.LinkTradeCodeField = value;
                    this.RaisePropertyChanged("LinkTradeCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid LinkTradeID {
            get {
                return this.LinkTradeIDField;
            }
            set {
                if ((this.LinkTradeIDField.Equals(value) != true)) {
                    this.LinkTradeIDField = value;
                    this.RaisePropertyChanged("LinkTradeID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int LinkTradeType {
            get {
                return this.LinkTradeTypeField;
            }
            set {
                if ((this.LinkTradeTypeField.Equals(value) != true)) {
                    this.LinkTradeTypeField = value;
                    this.RaisePropertyChanged("LinkTradeType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LogisticsCode {
            get {
                return this.LogisticsCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.LogisticsCodeField, value) != true)) {
                    this.LogisticsCodeField = value;
                    this.RaisePropertyChanged("LogisticsCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid RelevanceFilialeId {
            get {
                return this.RelevanceFilialeIdField;
            }
            set {
                if ((this.RelevanceFilialeIdField.Equals(value) != true)) {
                    this.RelevanceFilialeIdField = value;
                    this.RaisePropertyChanged("RelevanceFilialeId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid RelevanceWarehouseId {
            get {
                return this.RelevanceWarehouseIdField;
            }
            set {
                if ((this.RelevanceWarehouseIdField.Equals(value) != true)) {
                    this.RelevanceWarehouseIdField = value;
                    this.RaisePropertyChanged("RelevanceWarehouseId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid StockId {
            get {
                return this.StockIdField;
            }
            set {
                if ((this.StockIdField.Equals(value) != true)) {
                    this.StockIdField = value;
                    this.RaisePropertyChanged("StockId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int StockState {
            get {
                return this.StockStateField;
            }
            set {
                if ((this.StockStateField.Equals(value) != true)) {
                    this.StockStateField = value;
                    this.RaisePropertyChanged("StockState");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int StockType {
            get {
                return this.StockTypeField;
            }
            set {
                if ((this.StockTypeField.Equals(value) != true)) {
                    this.StockTypeField = value;
                    this.RaisePropertyChanged("StockType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool StockValidation {
            get {
                return this.StockValidationField;
            }
            set {
                if ((this.StockValidationField.Equals(value) != true)) {
                    this.StockValidationField = value;
                    this.RaisePropertyChanged("StockValidation");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int StorageType {
            get {
                return this.StorageTypeField;
            }
            set {
                if ((this.StorageTypeField.Equals(value) != true)) {
                    this.StorageTypeField = value;
                    this.RaisePropertyChanged("StorageType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal SubtotalQuantity {
            get {
                return this.SubtotalQuantityField;
            }
            set {
                if ((this.SubtotalQuantityField.Equals(value) != true)) {
                    this.SubtotalQuantityField = value;
                    this.RaisePropertyChanged("SubtotalQuantity");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ThirdCompanyID {
            get {
                return this.ThirdCompanyIDField;
            }
            set {
                if ((this.ThirdCompanyIDField.Equals(value) != true)) {
                    this.ThirdCompanyIDField = value;
                    this.RaisePropertyChanged("ThirdCompanyID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int TradeBothPartiesType {
            get {
                return this.TradeBothPartiesTypeField;
            }
            set {
                if ((this.TradeBothPartiesTypeField.Equals(value) != true)) {
                    this.TradeBothPartiesTypeField = value;
                    this.RaisePropertyChanged("TradeBothPartiesType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TradeCode {
            get {
                return this.TradeCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.TradeCodeField, value) != true)) {
                    this.TradeCodeField = value;
                    this.RaisePropertyChanged("TradeCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Transactor {
            get {
                return this.TransactorField;
            }
            set {
                if ((object.ReferenceEquals(this.TransactorField, value) != true)) {
                    this.TransactorField = value;
                    this.RaisePropertyChanged("Transactor");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid WarehouseId {
            get {
                return this.WarehouseIdField;
            }
            set {
                if ((this.WarehouseIdField.Equals(value) != true)) {
                    this.WarehouseIdField = value;
                    this.RaisePropertyChanged("WarehouseId");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="StorageRecordDetailInfo", Namespace="http://schemas.datacontract.org/2004/07/ERP.Model")]
    [System.SerializableAttribute()]
    public partial class StorageRecordDetailInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ApprovalNOField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BatchNoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime DateCreatedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime EffectiveDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GoodsCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid GoodsIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GoodsNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int GoodsTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal JoinPriceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int NonceWarehouseGoodsStockField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int QuantityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid RealGoodsIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private byte ShelfTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SpecificationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid StockIdField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid ThirdCompanyIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal UnitPriceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UnitsField;
        
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
        public string ApprovalNO {
            get {
                return this.ApprovalNOField;
            }
            set {
                if ((object.ReferenceEquals(this.ApprovalNOField, value) != true)) {
                    this.ApprovalNOField = value;
                    this.RaisePropertyChanged("ApprovalNO");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BatchNo {
            get {
                return this.BatchNoField;
            }
            set {
                if ((object.ReferenceEquals(this.BatchNoField, value) != true)) {
                    this.BatchNoField = value;
                    this.RaisePropertyChanged("BatchNo");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime DateCreated {
            get {
                return this.DateCreatedField;
            }
            set {
                if ((this.DateCreatedField.Equals(value) != true)) {
                    this.DateCreatedField = value;
                    this.RaisePropertyChanged("DateCreated");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Description {
            get {
                return this.DescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.DescriptionField, value) != true)) {
                    this.DescriptionField = value;
                    this.RaisePropertyChanged("Description");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime EffectiveDate {
            get {
                return this.EffectiveDateField;
            }
            set {
                if ((this.EffectiveDateField.Equals(value) != true)) {
                    this.EffectiveDateField = value;
                    this.RaisePropertyChanged("EffectiveDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GoodsCode {
            get {
                return this.GoodsCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.GoodsCodeField, value) != true)) {
                    this.GoodsCodeField = value;
                    this.RaisePropertyChanged("GoodsCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid GoodsId {
            get {
                return this.GoodsIdField;
            }
            set {
                if ((this.GoodsIdField.Equals(value) != true)) {
                    this.GoodsIdField = value;
                    this.RaisePropertyChanged("GoodsId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GoodsName {
            get {
                return this.GoodsNameField;
            }
            set {
                if ((object.ReferenceEquals(this.GoodsNameField, value) != true)) {
                    this.GoodsNameField = value;
                    this.RaisePropertyChanged("GoodsName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int GoodsType {
            get {
                return this.GoodsTypeField;
            }
            set {
                if ((this.GoodsTypeField.Equals(value) != true)) {
                    this.GoodsTypeField = value;
                    this.RaisePropertyChanged("GoodsType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal JoinPrice {
            get {
                return this.JoinPriceField;
            }
            set {
                if ((this.JoinPriceField.Equals(value) != true)) {
                    this.JoinPriceField = value;
                    this.RaisePropertyChanged("JoinPrice");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NonceWarehouseGoodsStock {
            get {
                return this.NonceWarehouseGoodsStockField;
            }
            set {
                if ((this.NonceWarehouseGoodsStockField.Equals(value) != true)) {
                    this.NonceWarehouseGoodsStockField = value;
                    this.RaisePropertyChanged("NonceWarehouseGoodsStock");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Quantity {
            get {
                return this.QuantityField;
            }
            set {
                if ((this.QuantityField.Equals(value) != true)) {
                    this.QuantityField = value;
                    this.RaisePropertyChanged("Quantity");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid RealGoodsId {
            get {
                return this.RealGoodsIdField;
            }
            set {
                if ((this.RealGoodsIdField.Equals(value) != true)) {
                    this.RealGoodsIdField = value;
                    this.RaisePropertyChanged("RealGoodsId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public byte ShelfType {
            get {
                return this.ShelfTypeField;
            }
            set {
                if ((this.ShelfTypeField.Equals(value) != true)) {
                    this.ShelfTypeField = value;
                    this.RaisePropertyChanged("ShelfType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Specification {
            get {
                return this.SpecificationField;
            }
            set {
                if ((object.ReferenceEquals(this.SpecificationField, value) != true)) {
                    this.SpecificationField = value;
                    this.RaisePropertyChanged("Specification");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid StockId {
            get {
                return this.StockIdField;
            }
            set {
                if ((this.StockIdField.Equals(value) != true)) {
                    this.StockIdField = value;
                    this.RaisePropertyChanged("StockId");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid ThirdCompanyID {
            get {
                return this.ThirdCompanyIDField;
            }
            set {
                if ((this.ThirdCompanyIDField.Equals(value) != true)) {
                    this.ThirdCompanyIDField = value;
                    this.RaisePropertyChanged("ThirdCompanyID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal UnitPrice {
            get {
                return this.UnitPriceField;
            }
            set {
                if ((this.UnitPriceField.Equals(value) != true)) {
                    this.UnitPriceField = value;
                    this.RaisePropertyChanged("UnitPrice");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Units {
            get {
                return this.UnitsField;
            }
            set {
                if ((object.ReferenceEquals(this.UnitsField, value) != true)) {
                    this.UnitsField = value;
                    this.RaisePropertyChanged("Units");
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
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResultInfo", Namespace="http://schemas.datacontract.org/2004/07/ERP.Service.Contract")]
    [System.SerializableAttribute()]
    public partial class ResultInfo : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.ServiceModel.FaultException FaultExceptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSuccessField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
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
        public System.ServiceModel.FaultException FaultException {
            get {
                return this.FaultExceptionField;
            }
            set {
                if ((object.ReferenceEquals(this.FaultExceptionField, value) != true)) {
                    this.FaultExceptionField = value;
                    this.RaisePropertyChanged("FaultException");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSuccess {
            get {
                return this.IsSuccessField;
            }
            set {
                if ((this.IsSuccessField.Equals(value) != true)) {
                    this.IsSuccessField = value;
                    this.RaisePropertyChanged("IsSuccess");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="TestForJMeter.ITestForJMeter")]
    public interface ITestForJMeter {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITestForJMeter/InsertStorageRecord", ReplyAction="http://tempuri.org/ITestForJMeter/InsertStorageRecordResponse")]
        WcfTestApp.TestForJMeter.InsertStorageRecordResponse InsertStorageRecord(WcfTestApp.TestForJMeter.InsertStorageRecordRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="InsertStorageRecord", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class InsertStorageRecordRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public WcfTestApp.TestForJMeter.StorageRecordInfo storageRecord;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=1)]
        public WcfTestApp.TestForJMeter.StorageRecordDetailInfo[] storageRecordDetail;
        
        public InsertStorageRecordRequest() {
        }
        
        public InsertStorageRecordRequest(WcfTestApp.TestForJMeter.StorageRecordInfo storageRecord, WcfTestApp.TestForJMeter.StorageRecordDetailInfo[] storageRecordDetail) {
            this.storageRecord = storageRecord;
            this.storageRecordDetail = storageRecordDetail;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.MessageContractAttribute(WrapperName="InsertStorageRecordResponse", WrapperNamespace="http://tempuri.org/", IsWrapped=true)]
    public partial class InsertStorageRecordResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="", Order=0)]
        public WcfTestApp.TestForJMeter.ResultInfo InsertStorageRecordResult;
        
        public InsertStorageRecordResponse() {
        }
        
        public InsertStorageRecordResponse(WcfTestApp.TestForJMeter.ResultInfo InsertStorageRecordResult) {
            this.InsertStorageRecordResult = InsertStorageRecordResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITestForJMeterChannel : WcfTestApp.TestForJMeter.ITestForJMeter, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TestForJMeterClient : System.ServiceModel.ClientBase<WcfTestApp.TestForJMeter.ITestForJMeter>, WcfTestApp.TestForJMeter.ITestForJMeter {
        
        public TestForJMeterClient() {
        }
        
        public TestForJMeterClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TestForJMeterClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TestForJMeterClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TestForJMeterClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public WcfTestApp.TestForJMeter.InsertStorageRecordResponse InsertStorageRecord(WcfTestApp.TestForJMeter.InsertStorageRecordRequest request) {
            return base.Channel.InsertStorageRecord(request);
        }
    }
}