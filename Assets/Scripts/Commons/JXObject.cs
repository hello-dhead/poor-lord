using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Commons
{
    /* Json Extended Object */
    public class JXObject
    {
        public JTokenType Type
        {
            get
            {
                if (Data == null)
                    return JTokenType.None;
                return Data.Type;
            }
        }

        public JToken Data { get; private set; }


        public static JXObject Parse(string json)
        {
            JXObject newObject = new JXObject(JProperty.Parse(json));
            return newObject;
        }
        public static bool TryParse(string json, out JXObject result)
        {
            try
            {
                JXObject newObject = new JXObject(JProperty.Parse(json));
                result = newObject;
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }

        }

        public JXObject()
        {
            Data = null;
        }
        public JXObject(JToken other)
        {
            Data = other;
        }
        public JXObject(JValue other)
        {
            Data = other;
        }

        public JXObject GetData(int index)
        {
            if (Data != null && (Data.Type == JTokenType.Array))
            {
                return new JXObject(Data[index]);
            }
            else
            {
                return new JXObject();
            }

        }
        public JXObject GetData(string key)
        {
            if (Data != null && (Data.Type == JTokenType.Object))
                return new JXObject(Data[key]); //알아서 익셉션 납니다.
            else
            {
                return new JXObject();
            }
        }

        public bool TryGetData(string key, out JXObject result)
        {
            if (Data != null && (Data.Type == JTokenType.Object))
            {
                if ((Data as JObject).ContainsKey(key) == false)
                {
                    result = null;
                    return false;
                }
                result = new JXObject(Data[key]);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public JXObject this[int index]
        {
            get
            {
                return GetData(index);
            }
            set
            {
                JXObject indexData = GetData(index);
                if (indexData != null && indexData.Data != null)
                    indexData.Data = value.Data;
            }
        }
        public JXObject this[string key]
        {
            get
            {
                if (Data != null && (Data.Type == JTokenType.Object))
                {
                    if ((Data as JObject).ContainsKey(key) == false)
                        return new JXObject();

                    return new JXObject(Data[key]);
                }
                else
                    return new JXObject();
            }
            set
            {
                JXObject indexData = GetData(key);
                if (indexData != null && indexData.Data != null)
                    Data[key] = value.Data;
            }
        }

        public bool SetData(int index, int data)
        {
            if (Data != null && (Data.Type == JTokenType.Array))
            {
                JValue val = new JValue(data);
                Data[index] = data;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetData(int index, float data)
        {
            if (Data != null && (Data.Type == JTokenType.Array))
            {
                JValue val = new JValue(data);
                Data[index] = data;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetData(int index, string data)
        {
            if (Data != null && (Data.Type == JTokenType.Array))
            {
                JValue val = new JValue(data);
                Data[index] = data;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetData(int index, bool data)
        {
            if (Data != null && (Data.Type == JTokenType.Array))
            {
                JValue val = new JValue(data);
                Data[index] = data;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool SetData(int index, DateTime data)
        {
            if (Data != null && (Data.Type == JTokenType.Array))
            {
                JValue val = new JValue(data);
                Data[index] = data;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetData(string key, int data)
        {
            if (Data != null && (Data.Type == JTokenType.Object))
            {
                JValue val = new JValue(data);
                Data[key] = data;
                return true;
            }
            else
            {
                return false;
            }
        }


        public int Count
        {
            get
            {
                if (Type == JTokenType.Array)
                    return (Data as JArray).Count;
                return 0;
            }
        }

        public bool ContainsKey(string key)
        {
            if (Data != null && (Data.Type == JTokenType.Object))
                return (Data as JObject).ContainsKey(key);
            else
                return false;
        }

        public JXObject DeepClone()
        {
            if (Data == null)
                return null;
            JXObject clone = new JXObject(Data.DeepClone());
            return clone;
        }
        /* 묵시적 캐스팅 */
        public static implicit operator int(JXObject jxObject)
        {
            if (jxObject.Type == JTokenType.Integer || jxObject.Type == JTokenType.Float)
                return jxObject.Data.Value<int>();
            else
                return default;
        }
        public static implicit operator float(JXObject jxObject)
        {
            if (jxObject.Type == JTokenType.Integer || jxObject.Type == JTokenType.Float)
                return jxObject.Data.Value<float>();
            else
                return default;
        }
        public static implicit operator DateTime(JXObject jxObject)
        {
            if (jxObject.Type == JTokenType.Date)
                return jxObject.Data.Value<DateTime>();
            else
                return default;
        }
        public static implicit operator string(JXObject jxObject)
        {
            if (jxObject.Type == JTokenType.String)
                return jxObject.Data.Value<string>();
            else
                return default;
        }
    }
}