using System.Collections.Generic;
using System.Text.Json;

namespace VeraNet.Utils;

public class JsonHelper
{
        /// <summary>
        /// Helper method to deserialize Vera JSON responses.
        /// </summary>
        /// <param name="json">The JSON response</param>
        /// <returns>
        /// A dictionary with the JSON structure.
        /// The possible object values are the referenced in the <see cref="GetValue"/> method
        /// </returns>
        /// <author>Rodrigo Peireso</author>
        public static Dictionary<string, object> DeserializeJson(JsonElement json)
        {
                // Create a dictionary to hold the deserialized JSON
                Dictionary<string, object> dictionary = new Dictionary<string, object>();

                // Iterate over each property of the JSON object
                foreach (JsonProperty property in json.EnumerateObject())
                {
                    // Convert the JSON element value to its corresponding native type
                    object value = GetValue(property.Value);

                    // Add the property name and its native type value to the dictionary
                    dictionary.Add(property.Name, value);
                }

                return dictionary;
            
        }

        /// <summary>
        /// Helper method to convert JSON elements to native types.
        /// It is used with the DeserializeJson method to recursively convert JSON elements to native types.
        /// </summary>
        /// <param name="element">The json element to be converted.</param>
        /// <returns>Can return native types such as number (int, double, long, decimal), bool, string or array objects.</returns>
        /// <author>Rodrigo Peireso</author>
        private static object GetValue(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out int intValue))
                    {
                        return intValue;
                    }
                    else if (element.TryGetDouble(out double doubleValue))
                    {
                        return doubleValue;
                    }
                    else if (element.TryGetInt64(out long longValue))
                    {
                        return longValue;
                    }
                    else if (element.TryGetDecimal(out decimal decimalValue))
                    {
                        return decimalValue;
                    }

                    break;
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Array:
                    List<object> array = new List<object>();
                    foreach (JsonElement item in element.EnumerateArray())
                    {
                        array.Add(GetValue(item));
                    }

                    return array.ToArray();
                case JsonValueKind.Object:
                    return DeserializeJson(element);
            }

            // Fallback to string representation if value kind is not recognized
            return element.ToString();
        }
}