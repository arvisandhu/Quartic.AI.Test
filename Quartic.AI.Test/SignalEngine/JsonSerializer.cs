namespace Quartic.AI.Test.SignalEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Script.Serialization;

    public class JsonSerializer
    {
        private JavaScriptSerializer _javaScriptSerializer;

        public JsonSerializer()
        {
            _javaScriptSerializer = new JavaScriptSerializer();
            _javaScriptSerializer.RegisterConverters(new[] { new JsonConverter() });
        }

        public string Serialize(object data)
        {
            string json = null;

            if (data == null)
                return json;

            try
            {
                json = _javaScriptSerializer.Serialize(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return json;
        }

        public JsonSignal[] Deserialize(string json)
        {
            JsonSignal[] signals = null;

            if (string.IsNullOrWhiteSpace(json))
                return signals;

            try
            {
                signals = this._javaScriptSerializer.Deserialize<JsonSignal[]>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return signals;
        }
    }

    public class JsonConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new[] { typeof(JsonSignal) };
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            JsonSignal signal = null;
            if (dictionary != null)
            {
                try
                {
                    signal = new JsonSignal { SignalID = dictionary["signal"] as string, ValueType = dictionary["value_type"] as string };

                    switch (signal.ValueType)
                    {
                        case "String":
                            signal.Value = dictionary["value"] as string;
                            break;

                        case "Integer":
                            double doubleValue;
                            if (double.TryParse(dictionary["value"] as string, out doubleValue))
                                signal.Value = doubleValue;

                            break;

                        case "Datetime":
                            DateTime dateTime;
                            if (DateTime.TryParse(dictionary["value"] as string, out dateTime))
                                signal.Value = dateTime;

                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }

            return signal;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            return null;
        }
    }
}