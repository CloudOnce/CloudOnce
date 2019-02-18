/*
 * http://www.opensource.org/licenses/lgpl-2.1.php
 * JSONObject class
 * for use with Unity
 * Copyright Matt Schoen 2010 - 2013
 * Modified: Sindri Jóelsson 2014, Jan Ivar Carlsen 2015-2016
 */

namespace CloudOnce.Internal
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class JSONObject
    {
        private const int maxDepth = 100;
        private const string infinity = "\"INFINITY\"";
        private const string negInfinity = "\"NEGINFINITY\"";
        private const string nan = "\"NaN\"";

        private static readonly char[] s_whitespace = { ' ', '\r', '\n', '\t' };

        public JSONObject(Type t)
        {
            ObjectType = t;
            switch (t)
            {
                case Type.Array:
                    List = new List<JSONObject>();
                    break;
                case Type.Object:
                    List = new List<JSONObject>();
                    Keys = new List<string>();
                    break;
            }
        }

        public JSONObject(bool b)
        {
            ObjectType = Type.Bool;
            B = b;
        }

        public JSONObject(float f)
        {
            ObjectType = Type.Number;
            F = f;
        }

        public JSONObject(Dictionary<string, string> dic)
        {
            ObjectType = Type.Object;
            Keys = new List<string>();
            List = new List<JSONObject>();

            // Not sure if it's worth removing the foreach here
            foreach (KeyValuePair<string, string> kvp in dic)
            {
                Keys.Add(kvp.Key);
                List.Add(CreateStringObject(kvp.Value));
            }
        }

        public JSONObject(Dictionary<string, JSONObject> dic)
        {
            ObjectType = Type.Object;
            Keys = new List<string>();
            List = new List<JSONObject>();

            // Not sure if it's worth removing the foreach here
            foreach (KeyValuePair<string, JSONObject> kvp in dic)
            {
                Keys.Add(kvp.Key);
                List.Add(kvp.Value);
            }
        }

        public JSONObject(AddJsonContents content)
        {
            content.Invoke(this);
        }

        public JSONObject(IEnumerable<JSONObject> objects)
        {
            ObjectType = Type.Array;
            List = new List<JSONObject>(objects);
        }

        public JSONObject()
        {
        }

        // create a new JSONObject from a string (this will also create any children, and parse the whole string)
        public JSONObject(string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
        {
            Parse(str, maxDepth, storeExcessLevels, strict);
        }

        public delegate void AddJsonContents(JSONObject self);

        public enum Type
        {
            Null,
            String,
            Number,
            Object,
            Array,
            Bool,
            Baked
        }

        public List<JSONObject> List { get; private set; }

        public bool IsContainer
        {
            get { return ObjectType == Type.Array || ObjectType == Type.Object; }
        }

        public Type ObjectType { get; set; }

        public List<string> Keys { get; private set; }

        public string String { get; private set; }

        public float F { get; private set; }

        public bool B { get; private set; }

        public JSONObject this[int index]
        {
            get
            {
                return List.Count > index ? List[index] : null;
            }

            set
            {
                if (List.Count > index)
                {
                    List[index] = value;
                }
            }
        }

        public JSONObject this[string index]
        {
            get { return GetField(index); }
            set { SetField(index, value); }
        }

        public static implicit operator bool(JSONObject o)
        {
            return o != null;
        }

        // Convenience function for creating a JSONObject containing a string.
        // This is not part of the constructor so that malformed JSON data doesn't just turn into a string object
        public static JSONObject StringObject(string val)
        {
            return CreateStringObject(val);
        }

        public static JSONObject Create()
        {
            return new JSONObject();
        }

        public static JSONObject Create(Type t)
        {
            var obj = Create();
            obj.ObjectType = t;
            switch (t)
            {
                case Type.Array:
                    obj.List = new List<JSONObject>();
                    break;
                case Type.Object:
                    obj.List = new List<JSONObject>();
                    obj.Keys = new List<string>();
                    break;
            }

            return obj;
        }

        public static JSONObject Create(bool val)
        {
            var obj = Create();
            obj.ObjectType = Type.Bool;
            obj.B = val;
            return obj;
        }

        public static JSONObject Create(float val)
        {
            var obj = Create();
            obj.ObjectType = Type.Number;
            obj.F = val;
            return obj;
        }

        public static JSONObject Create(int val)
        {
            var obj = Create();
            obj.ObjectType = Type.Number;
            obj.F = val;
            return obj;
        }

        public static JSONObject CreateStringObject(string val)
        {
            var obj = Create();
            obj.ObjectType = Type.String;
            obj.String = val;
            return obj;
        }

        public static JSONObject CreateBakedObject(string val)
        {
            var bakedObject = Create();
            bakedObject.ObjectType = Type.Baked;
            bakedObject.String = val;
            return bakedObject;
        }

        /// <summary>
        /// Create a JSONObject by parsing string data
        /// </summary>
        /// <param name="val">The string to be parsed</param>
        /// <param name="maxDepth">The maximum depth for the parser to search.  Set this to to 1 for the first level,
        /// 2 for the first 2 levels, etc.  It defaults to -2 because -1 is the depth value that is parsed (see below)</param>
        /// <param name="storeExcessLevels">Whether to store levels beyond maxDepth in baked JSONObjects</param>
        /// <param name="strict">Whether to be strict in the parsing. For example, non-strict parsing will successfully
        /// parse "a string" into a string-type </param>
        /// <returns>The created JSON object.</returns>
        public static JSONObject Create(string val, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
        {
            var obj = Create();
            obj.Parse(val, maxDepth, storeExcessLevels, strict);
            return obj;
        }

        public static JSONObject Create(AddJsonContents content)
        {
            JSONObject obj = Create();
            content.Invoke(obj);
            return obj;
        }

        public static JSONObject Create(Dictionary<string, string> dic)
        {
            var obj = Create();
            obj.ObjectType = Type.Object;
            obj.Keys = new List<string>();
            obj.List = new List<JSONObject>();

            // Not sure if it's worth removing the foreach here
            foreach (var kvp in dic)
            {
                obj.Keys.Add(kvp.Key);
                obj.List.Add(CreateStringObject(kvp.Value));
            }

            return obj;
        }

        public static JSONObject Create(Dictionary<string, float> dic)
        {
            var obj = Create();
            obj.ObjectType = Type.Object;
            obj.Keys = new List<string>();
            obj.List = new List<JSONObject>();

            // Not sure if it's worth removing the foreach here
            foreach (var kvp in dic)
            {
                obj.Keys.Add(kvp.Key);
                obj.List.Add(new JSONObject(kvp.Value));
            }

            return obj;
        }

        public void Absorb(JSONObject obj)
        {
            List.AddRange(obj.List);
            Keys.AddRange(obj.Keys);
            String = obj.String;
            F = obj.F;
            B = obj.B;
            ObjectType = obj.ObjectType;
        }

        public void Add(JSONObject obj)
        {
            if (obj)
            {
                // Don't do anything if the object is null
                if (ObjectType != Type.Array)
                {
                    ObjectType = Type.Array; // Congratulations, son, you're an ARRAY now
                    if (List == null)
                    {
                        List = new List<JSONObject>();
                    }
                }

                List.Add(obj);
            }
        }

        public void AddField(string name, bool val)
        {
            AddField(name, Create(val));
        }

        public void AddField(string name, float val)
        {
            AddField(name, Create(val));
        }

        public void AddField(string name, string val)
        {
            AddField(name, CreateStringObject(val));
        }

        public void AddField(string name, JSONObject obj)
        {
            if (obj)
            {
                // Don't do anything if the object is null
                if (ObjectType != Type.Object)
                {
                    if (Keys == null)
                    {
                        Keys = new List<string>();
                    }

                    if (ObjectType == Type.Array)
                    {
                        for (var i = 0; i < List.Count; i++)
                        {
                            Keys.Add(i + string.Empty);
                        }
                    }
                    else if (List == null)
                    {
                        List = new List<JSONObject>();
                    }

                    ObjectType = Type.Object;        // Congratulations, son, you're an OBJECT now
                }

                Keys.Add(name);
                List.Add(obj);
            }
        }

        public void RemoveField(string name)
        {
            if (Keys.IndexOf(name) > -1)
            {
                List.RemoveAt(Keys.IndexOf(name));
                Keys.Remove(name);
            }
        }

        public bool HasFields(params string[] names)
        {
            foreach (string t in names)
            {
                if (!Keys.Contains(t))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return Print();
        }

        public string ToString(bool pretty)
        {
            return Print(pretty);
        }

        public Dictionary<string, string> ToDictionary()
        {
            if (ObjectType == Type.Object)
            {
                var result = new Dictionary<string, string>();
                for (var i = 0; i < List.Count; i++)
                {
                    var val = List[i];
                    switch (val.ObjectType)
                    {
                        case Type.String:
                            result.Add(Keys[i], val.String);
                            break;
                        case Type.Number:
                            result.Add(Keys[i], val.F.ToString(CultureInfo.InvariantCulture));
                            break;
                        case Type.Bool:
                            result.Add(Keys[i], val.B + string.Empty);
                            break;
                        default:
                            UnityEngine.Debug.LogWarning("Omitting object: " + Keys[i] + " in dictionary conversion");
                            break;
                    }
                }

                return result;
            }
#if CLOUDONCE_DEBUG
            UnityEngine.Debug.LogWarning("Tried to turn non-Object JSONObject into a dictionary");
#endif
            return null;
        }

        #region PARSE

        private void Parse(string str, int maxDepth = -2, bool storeExcessLevels = false, bool strict = false)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Trim(s_whitespace);
                if (strict)
                {
                    if (str[0] != '[' && str[0] != '{')
                    {
                        ObjectType = Type.Null;
#if CLOUDONCE_DEBUG
                        UnityEngine.Debug.LogWarning("Improper (strict) JSON formatting.  First character must be [ or {");
#endif
                        return;
                    }
                }

                if (str.Length > 0)
                {
                    if (string.Compare(str, "true", System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ObjectType = Type.Bool;
                        B = true;
                    }
                    else if (string.Compare(str, "false", System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ObjectType = Type.Bool;
                        B = false;
                    }
                    else if (string.Compare(str, "null", System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ObjectType = Type.Null;
                    }
                    else
                    {
                        switch (str)
                        {
                            case infinity:
                                ObjectType = Type.Number;
                                F = float.PositiveInfinity;
                                break;
                            case negInfinity:
                                ObjectType = Type.Number;
                                F = float.NegativeInfinity;
                                break;
                            case nan:
                                ObjectType = Type.Number;
                                F = float.NaN;
                                break;
                            default:
                                if (str[0] == '"')
                                {
                                    ObjectType = Type.String;
                                    String = str.Substring(1, str.Length - 2);
                                }
                                else
                                {
                                    var tokenTmp = 1;
                                    /*
                                     * Checking for the following formatting (www.json.org)
                                     * object - {"field1":value,"field2":value}
                                     * array - [value,value,value]
                                     * value - string    - "string"
                                     *         - number    - 0.0
                                     *         - bool        - true -or- false
                                     *         - null        - null
                                     */
                                    var offset = 0;
                                    switch (str[offset])
                                    {
                                        case '{':
                                            ObjectType = Type.Object;
                                            Keys = new List<string>();
                                            List = new List<JSONObject>();
                                            break;
                                        case '[':
                                            ObjectType = Type.Array;
                                            List = new List<JSONObject>();
                                            break;
                                        default:
                                            try
                                            {
                                                F = System.Convert.ToSingle(str, CultureInfo.InvariantCulture);
                                                ObjectType = Type.Number;
                                            }
                                            catch (System.FormatException)
                                            {
                                                ObjectType = Type.Null;
#if CLOUDONCE_DEBUG
                                                UnityEngine.Debug.LogWarning("improper JSON formatting:" + str);
#endif
                                            }

                                            return;
                                    }

                                    var propName = string.Empty;
                                    var openQuote = false;
                                    var inProp = false;
                                    var depth = 0;
                                    while (++offset < str.Length)
                                    {
                                        if (System.Array.IndexOf(s_whitespace, str[offset]) > -1)
                                        {
                                            continue;
                                        }

                                        if (str[offset] == '\\')
                                        {
                                            offset += 2;
                                        }

                                        if (str[offset] == '"')
                                        {
                                            if (openQuote)
                                            {
                                                if (!inProp && depth == 0 && ObjectType == Type.Object)
                                                {
                                                    propName = str.Substring(tokenTmp + 1, offset - tokenTmp - 1);
                                                }

                                                openQuote = false;
                                            }
                                            else
                                            {
                                                if (depth == 0 && ObjectType == Type.Object)
                                                {
                                                    tokenTmp = offset;
                                                }

                                                openQuote = true;
                                            }
                                        }

                                        if (openQuote)
                                        {
                                            continue;
                                        }

                                        if (ObjectType == Type.Object && depth == 0)
                                        {
                                            if (str[offset] == ':')
                                            {
                                                tokenTmp = offset + 1;
                                                inProp = true;
                                            }
                                        }

                                        switch (str[offset])
                                        {
                                            case '{':
                                            case '[':
                                                depth++;
                                                break;
                                            case '}':
                                            case ']':
                                                depth--;
                                                break;
                                        }

                                        if ((str[offset] == ',' && depth == 0) || depth < 0)
                                        {
                                            inProp = false;
                                            var inner = str.Substring(tokenTmp, offset - tokenTmp).Trim(s_whitespace);
                                            if (inner.Length > 0)
                                            {
                                                if (ObjectType == Type.Object)
                                                {
                                                    Keys.Add(propName);
                                                }

                                                // maxDepth of -1 is the end of the line
                                                if (maxDepth != -1)
                                                {
                                                    List.Add(Create(inner, (maxDepth < -1) ? -2 : maxDepth - 1));
                                                }
                                                else if (storeExcessLevels)
                                                {
                                                    List.Add(CreateBakedObject(inner));
                                                }
                                            }

                                            tokenTmp = offset + 1;
                                        }
                                    }
                                }

                                break;
                        }
                    }
                }
                else
                {
                    ObjectType = Type.Null;
                }
            }
            else
            {
                ObjectType = Type.Null; // If the string is missing, this is a null
            }
        }

        #endregion

        private void SetField(string name, JSONObject obj)
        {
            if (HasField(name))
            {
                List.Remove(this[name]);
                Keys.Remove(name);
            }

            AddField(name, obj);
        }

        private JSONObject GetField(string name)
        {
            if (ObjectType == Type.Object)
            {
                for (var i = 0; i < Keys.Count; i++)
                {
                    if (Keys[i] == name)
                    {
                        return List[i];
                    }
                }
            }

            return null;
        }

        private bool HasField(string name)
        {
            return ObjectType == Type.Object && Keys.Any(t => t == name);
        }

        private string Print(bool pretty = false)
        {
            var builder = new StringBuilder();
            Stringify(0, builder, pretty);
            return builder.ToString();
        }

        #region STRINGIFY

        // Convert the JSONObject into a string
        private void Stringify(int depth, StringBuilder builder, bool pretty = false)
        {
            if (depth++ > maxDepth)
            {
#if CLOUDONCE_DEBUG
                UnityEngine.Debug.Log("reached max depth!");
#endif
                return;
            }

            switch (ObjectType)
            {
                case Type.Baked:
                    builder.Append(String);
                    break;
                case Type.String:
                    builder.AppendFormat("\"{0}\"", String);
                    break;
                case Type.Number:
                    if (float.IsInfinity(F))
                    {
                        builder.Append(infinity);
                    }
                    else if (float.IsNegativeInfinity(F))
                    {
                        builder.Append(negInfinity);
                    }
                    else if (float.IsNaN(F))
                    {
                        builder.Append(nan);
                    }
                    else
                    {
                        builder.Append(F.ToString(CultureInfo.InvariantCulture));
                    }

                    break;
                case Type.Object:
                    builder.Append("{");
                    if (List.Count > 0)
                    {
                        if (pretty)
                        {
                            builder.Append("\n");
                        }

                        for (var i = 0; i < List.Count; i++)
                        {
                            var key = Keys[i];
                            var obj = List[i];
                            if (obj)
                            {
                                if (pretty)
                                {
                                    for (var j = 0; j < depth; j++)
                                    {
                                        builder.Append("\t"); // for a bit more readability
                                    }
                                }

                                builder.AppendFormat("\"{0}\":", key);
                                obj.Stringify(depth, builder, pretty);
                                builder.Append(",");
                                if (pretty)
                                {
                                    builder.Append("\n");
                                }
                            }
                        }

                        if (pretty)
                        {
                            builder.Length -= 2;
                        }
                        else
                        {
                            builder.Length--;
                        }
                    }

                    if (pretty && List.Count > 0)
                    {
                        builder.Append("\n");
                        for (var j = 0; j < depth - 1; j++)
                        {
                            builder.Append("\t"); // for a bit more readability
                        }
                    }

                    builder.Append("}");
                    break;
                case Type.Array:
                    builder.Append("[");
                    if (List.Count > 0)
                    {
                        if (pretty)
                        {
                            builder.Append("\n"); // for a bit more readability
                        }

                        foreach (var t in List)
                        {
                            if (t)
                            {
                                if (pretty)
                                {
                                    for (var j = 0; j < depth; j++)
                                    {
                                        builder.Append("\t"); // for a bit more readability
                                    }
                                }

                                t.Stringify(depth, builder, pretty);
                                builder.Append(",");
                                if (pretty)
                                {
                                    builder.Append("\n"); // for a bit more readability
                                }
                            }
                        }

                        if (pretty)
                        {
                            builder.Length -= 2;
                        }
                        else
                        {
                            builder.Length--;
                        }
                    }

                    if (pretty && List.Count > 0)
                    {
                        builder.Append("\n");
                        for (var j = 0; j < depth - 1; j++)
                        {
                            builder.Append("\t"); // for a bit more readability
                        }
                    }

                    builder.Append("]");
                    break;
                case Type.Bool:
                    builder.Append(B ? "true" : "false");
                    break;
                case Type.Null:
                    builder.Append("null");
                    break;
            }
        }

        #endregion
    }
}
