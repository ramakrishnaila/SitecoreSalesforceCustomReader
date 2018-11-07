using Sitecore.DataExchange.DataAccess;
using Sitecore.DataExchange.DataAccess.Readers;
using Sitecore.DataExchange.DataAccess.Reflection;
using Sitecore.DataExchange.Providers.XConnect.Extensions;
using Sitecore.DataExchange.Providers.XConnect.Models;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Demo.Salesforce.DataExchange.DataAccess.Readers
{
	public class IndexerPropertyValueReader : IValueReader
	{
		public ICollection<string> ContainsIndexMethodNames
		{
			get;
			private set;
		}

		public object[] Indexes
		{
			get;
			private set;
		}

		public IReflectionUtil ReflectionUtil
		{
			get;
			set;
		}

		public IndexerPropertyValueReader(params object[] indexes)
		{
			this.ReflectionUtil = Sitecore.DataExchange.DataAccess.Reflection.ReflectionUtil.Instance;
			this.ContainsIndexMethodNames = new List<string>();
			this.Indexes = indexes;
		}

		protected virtual bool CanRead(object source, DataAccessContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			bool flag = false;
			if (source != null)
			{
				PropertyInfo indexerProperty = this.ReflectionUtil.GetIndexerProperty(source, this.Indexes);
				if (indexerProperty != null && indexerProperty.CanRead)
				{
					flag = true;
				}
			}
			return flag;
		}

		protected virtual IndexerPropertyKeyStatus IsValueSet(object source, DataAccessContext context)
		{
			IndexerPropertyKeyStatus indexerPropertyKeyStatu;
			using (IEnumerator<string> enumerator = this.ContainsIndexMethodNames.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					MethodInfo method = this.ReflectionUtil.GetMethod(current, source, this.Indexes);
					if (method == null)
					{
						continue;
					}
					object obj = method.Invoke(source, this.Indexes.ToArray<object>());
					if (!object.Equals(false, obj))
					{
						break;
					}
					indexerPropertyKeyStatu = IndexerPropertyKeyStatus.IndexNotUsed;
					return indexerPropertyKeyStatu;
				}
				PropertyInfo indexerProperty = this.ReflectionUtil.GetIndexerProperty(source, this.Indexes);
				if (!(indexerProperty != null) || !indexerProperty.CanRead)
				{
					return IndexerPropertyKeyStatus.Undetermined;
				}
				if (indexerProperty.GetValue(source, this.Indexes.ToArray<object>()) == null)
				{
					return IndexerPropertyKeyStatus.IndexUsedNull;
				}
				return IndexerPropertyKeyStatus.IndexUsedNotNull;
			}
			return indexerPropertyKeyStatu;
		}

		public ReadResult Read(object source, DataAccessContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			bool flag = false;
			object value = null;
            ContactModel contactModel = (ContactModel) source;
            if(contactModel != null)
            {
                AddressList myFacetObject = (AddressList)contactModel.GetFacet(AddressList.DefaultFacetKey);
                value = GetPropValue(myFacetObject.PreferredAddress, this.Indexes?[0]?.ToString());// myFacetObject.PreferredAddress?.GetType()?.GetProperty(this.Indexes?[0]?.ToString())?.GetValue(myFacetObject.PreferredAddress, null)?.ToString();
                if(value != null)
                {
                    flag = true;
                }
            }
            //         PropertyInfo indexerProperty = this.ReflectionUtil.GetIndexerProperty(source, this.Indexes);
            //if (indexerProperty != null && indexerProperty.CanRead)
            //{
            //	IndexerPropertyKeyStatus indexerPropertyKeyStatu = this.IsValueSet(source, context);
            //	if (indexerPropertyKeyStatu == IndexerPropertyKeyStatus.IndexUsedNotNull || indexerPropertyKeyStatu == IndexerPropertyKeyStatus.IndexUsedNull)
            //	{
            //		value = indexerProperty.GetValue(source, this.Indexes.ToArray<object>());
            //	}
            //	flag = true;
            //}
            return new ReadResult(DateTime.UtcNow)
            {
                ReadValue = value,
                WasValueRead = flag
            };
        }
        public static object GetPropValue(object src, string propName)
        {
            try
            {
              return src.GetType()?.GetProperty(propName)?.GetValue(src, null);
            }
            catch (Exception)
            {
               return null;
            }
        }
    }
}