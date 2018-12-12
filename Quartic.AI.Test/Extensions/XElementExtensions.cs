namespace Quartic.AI.Test.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Quartic.AI.Test.Enums;
    using Quartic.AI.Test.SignalEngine;

    public static class XElementExtensions
    {
        #region Reading
        public static List<SignalRule> ToListOfTypeSignalRule(this List<XElement> elements)
        {
            List<SignalRule> signalRules = new List<SignalRule>();
            elements.ForEach((Action<XElement>)(element =>
            {
                SignalRule signalRule = new SignalRule();

                XAttribute attribute = element.Attribute("SignalID");
                if (attribute != null)
                {
                    signalRule.SignalID = attribute.Value;
                }

                attribute = element.Attribute("ValueType");
                if (attribute != null)
                {
                    if (Enum.TryParse(attribute.Value, out ValueDataType valueType))
                        signalRule.ValueType = valueType;
                }

                attribute = element.Attribute("IsActive");
                if (attribute != null)
                {
                    if (Enum.TryParse(attribute.Value, out TrueFalse isActive))
                        signalRule.IsActive = isActive;
                }

                attribute = element.Attribute("AllowNull");
                if (attribute != null)
                {
                    if (Enum.TryParse(attribute.Value, out TrueFalse allowNull))
                        signalRule.AllowNull = allowNull;
                }

                attribute = element.Attribute("MinValue");
                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Value))
                {
                    if (double.TryParse(attribute.Value, out double value))
                        signalRule.MinValue = value;
                }

                attribute = element.Attribute("MaxValue");
                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Value))
                {
                    if (double.TryParse(attribute.Value, out double value))
                        signalRule.MaxValue = value;
                }

                attribute = element.Attribute("AllowedValues");
                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Value))
                {
                    signalRule.AllowedValues = attribute.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                }

                attribute = element.Attribute("NotAllowedValues");
                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Value))
                {
                    signalRule.NotAllowedValues = attribute.Value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                }

                attribute = element.Attribute("MinLength");
                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Value))
                {
                    if (int.TryParse(attribute.Value, out int value))
                        signalRule.MinLength = value;
                }

                attribute = element.Attribute("MaxLength");
                if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Value))
                {
                    if (int.TryParse(attribute.Value, out int value))
                        signalRule.MaxLength = value;
                }

                attribute = element.Attribute("AllowFutureDate");
                if (attribute != null)
                {
                    if (Enum.TryParse(attribute.Value, out TrueFalse allow))
                        signalRule.AllowFutureDate = allow;
                }

                attribute = element.Attribute("DateFormat");
                if (attribute != null)
                {
                    signalRule.DateFormat = attribute.Value;
                }

                attribute = element.Attribute("MinDate");
                if (attribute != null)
                {
                    signalRule.MinDate = attribute.Value;
                }

                attribute = element.Attribute("MaxDate");
                if (attribute != null)
                {
                    signalRule.MaxDate = attribute.Value;
                }

                signalRule.Element = element;
                signalRules.Add(signalRule);
            }));

            return signalRules;
        }

        #endregion

        #region New
        public static XElement NewSignalRule()
        {
            return new XElement(
                "Signal",
                new XAttribute("SignalID", string.Empty),
                new XAttribute("ValueType", string.Empty),
                new XAttribute("AllowNull", "False"),
                new XAttribute("IsActive", "True"));
        }

        public static SignalRule ToTypeSignalRule(this XElement element)
        {
            SignalRule signalRule = new SignalRule();
            XAttribute attribute = element.Attribute("SignalID");
            if (attribute != null)
            {
                signalRule.SignalID = attribute.Value;
            }

            attribute = element.Attribute("IsMandatory");
            if (attribute != null)
            {
                if (Enum.TryParse(attribute.Value, out TrueFalse isMandatory))
                {
                    signalRule.AllowNull = isMandatory;
                }
            }

            attribute = element.Attribute("IsActive");
            if (attribute != null)
            {
                if (Enum.TryParse(attribute.Value, out TrueFalse isActive))
                {
                    signalRule.IsActive = isActive;
                }
            }

            attribute = element.Attribute("ValueType");
            if (attribute != null)
            {
                if (Enum.TryParse(attribute.Value, out ValueDataType valueType))
                {
                    signalRule.ValueType = valueType;
                }
            }

            signalRule.Element = element;
            return signalRule;
        }
        #endregion
    }
}