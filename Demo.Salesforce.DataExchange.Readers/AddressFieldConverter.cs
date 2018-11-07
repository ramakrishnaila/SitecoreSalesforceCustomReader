using Demo.Salesforce.DataExchange.DataAccess.Readers;
using Sitecore.DataExchange;
using Sitecore.DataExchange.Attributes;
using Sitecore.DataExchange.Converters;
using Sitecore.DataExchange.DataAccess;
using Sitecore.DataExchange.Repositories;
using Sitecore.Services.Core.Model;
using System;
namespace Sitecore.Demo.Salesforce.DataExchange
{ 
    [SupportedIds(new string[] { "{34D69975-7AD7-460D-AAC2-1988355F404D}" })]
    public class AddressFieldConverter : BaseItemModelConverter<IValueReader>
    {
        public const string FieldNamePropertyName = "PropertyName";
         
        public AddressFieldConverter(IItemModelRepository repository)
          : base(repository)
        {
        }


        protected override ConvertResult<IValueReader> ConvertSupportedItem(ItemModel source)
        {
            string stringValue = this.GetStringValue(source, "PropertyName");
            if (!string.IsNullOrWhiteSpace(stringValue))
                return this.PositiveResult((IValueReader)new IndexerPropertyValueReader(stringValue));
            return this.NegativeResult(source, "The property name field must have a value specified.", string.Format("field: {0}", (object)"PropertyName"));
        }
    }
}
