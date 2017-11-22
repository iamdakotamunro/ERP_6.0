using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    [Serializable]
    public class KeyAndValue
    {
        public Guid Key { get; set; }

        public int Value { get; set; }

        public KeyAndValue() { }

        public KeyAndValue(Guid key, int value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class StorageLackQuantity
    {
        public Guid StockId { get; set; }

        public String TradeCode { get; set; }

        public Guid FilialeId { get; set; }

        public int Quantity { get; set; }
    }
}
