namespace Quartic.AI.Test.Models
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using Quartic.AI.Test.Enums;
    using Quartic.AI.Test.Extensions;
    using Quartic.AI.Test.SignalEngine;

    public class ElementProperties : ObservableCollection<ElementProperty>
    {
        public void ValidateCrossFieldDependency(SignalRule signalRule)
        {
            if (signalRule != null)
            {
                PropertyInfo[] propertiesInfo = null;
                switch (signalRule.ValueType)
                {
                    case ValueDataType.Integer:
                        propertiesInfo = signalRule.GetType().GetIntegerWhereNotExemptProperties();
                        break;
                    case ValueDataType.String:
                        propertiesInfo = signalRule.GetType().GetStringWhereNotExemptProperties();
                        break;
                    case ValueDataType.Datetime:
                        propertiesInfo = signalRule.GetType().GetDateTimeWhereNotExemptProperties();
                        break;
                }

                if (propertiesInfo != null)
                {
                    bool isAnySet = false;
                    foreach (PropertyInfo propertyInfo in propertiesInfo)
                    {
                        if (propertyInfo.GetValue(signalRule) != null)
                        {
                            if (propertyInfo.PropertyType.IsArray)
                            {
                                if (propertyInfo.PropertyType.Name == "String[]")
                                {
                                    string[] forLength = propertyInfo.GetValue(signalRule) as string[];
                                    if (forLength.Length != 0)
                                    {
                                        isAnySet = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                isAnySet = true;
                                break;
                            }
                        }
                    }

                    foreach (PropertyInfo property in propertiesInfo)
                    {
                        ElementProperty elementProperty = this.FirstOrDefault(x => x.PropertyName == property.Name);
                        if (elementProperty != null)
                        {
                            elementProperty.HasDataError = false;

                            if (signalRule.ValueType == ValueDataType.Integer)
                            {
                                if (signalRule.MaxValue.HasValue && signalRule.MinValue.HasValue)
                                    if (signalRule.MaxValue.Value < signalRule.MinValue.Value)
                                        elementProperty.HasDataError = true;
                            }
                            else if (signalRule.ValueType == ValueDataType.String)
                            {
                                if (signalRule.MaxLength.HasValue && signalRule.MinLength.HasValue)
                                    if (signalRule.MaxLength.Value < signalRule.MinLength.Value)
                                        elementProperty.HasDataError = true;
                            }
                            else if (signalRule.ValueType == ValueDataType.Datetime)
                            {
                                if (!string.IsNullOrEmpty(signalRule.MaxDate) && !string.IsNullOrEmpty(signalRule.MinDate))
                                {
                                    if (DateTime.TryParse(signalRule.MinDate, out DateTime minDate))
                                    {
                                        if (DateTime.TryParse(signalRule.MaxDate, out DateTime maxDate))
                                        {
                                            if (maxDate < minDate)
                                                elementProperty.HasDataError = true;
                                        }
                                    }
                                }
                            }

                            elementProperty.IsMandatory = !isAnySet;
                            elementProperty.Validate(nameof(elementProperty.PropertyValue));
                        }
                    }
                }
            }
        }
    }
}