using Assets.Scripts.Cloud.Schemas;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


public class CloudDatabaseHelper
{
    

    static public object CreateXPField(ExperiencePoint xp)
    {
        return new
        {
            mapValue = new
            {
                fields = new
                {
                    level = new { integerValue = xp.Level.ToString() },
                    currentXP = new { integerValue = xp.CurrentXP.ToString() },
                    totalXP = new { integerValue = xp.TotalXP.ToString() }
                }
            }
        };
    }

    static public object CreateXPField()
    {
        return new
        {
            mapValue = new
            {
                fields = new
                {
                    level = new { integerValue = "1" },
                    currentXP = new { integerValue = "0" },
                    totalXP = new { integerValue = "0" }
                }
            }
        };
    }

    static public object CreateInventoryField(Inventory inventory)
    {
        var itemsArray = new List<object>();

        foreach (var item in inventory.Items)
        {
            itemsArray.Add(new
            {
                mapValue = new
                {
                    fields = new
                    {
                        itemId = new { stringValue = item.ItemId },
                        quantity = new { integerValue = item.Quantity.ToString() },
                        slotIndex = new { integerValue = item.SlotIndex.ToString() }
                    }
                }
            });
        }

        return new
        {
            mapValue = new
            {
                fields = new
                {
                    maxSlots = new { integerValue = inventory.MaxSlots.ToString() },
                    items = new { arrayValue = new { values = itemsArray } }
                }
            }
        };
    }

    static public object CreateInventoryField()
    {
        
        return new
        {
            mapValue = new
            {
                fields = new
                {
                    maxSlots = new { integerValue = "1" },
                    items = new { arrayValue = new { values = Array.Empty<object>() } }
                }
            }
        };
    }

    static public object CreateDocument(string path, object data)
    {

        return new
        {
            name = path,
            fields = data
        };
    }

    static public object CreateFirestoreValue(object value, string valueType)
    {
        switch (valueType)
        {
            case "integerValue":
                return new { integerValue = value.ToString() };
            case "doubleValue":
                return new { doubleValue = value };
            case "stringValue":
                return new { stringValue = value.ToString() };
            case "booleanValue":
                return new { booleanValue = (bool)value };
            default:
                return new { stringValue = value.ToString() };
        }
    }


    public static class FirestoreMapper
    {
        // Hàm chính: Gọi hàm này để convert
        public static T ToObject<T>(Dictionary<string, FirestoreValue> fields) where T : new()
        {
            T obj = new T();
            Type type = typeof(T);

            // Duyệt qua tất cả các Property trong class T (Farmland, Plot...)
            foreach (PropertyInfo prop in type.GetProperties())
            {
                // 1. Tìm tên field tương ứng trong Dictionary (Ưu tiên đúng case, fallback về camelCase)
                string key = prop.Name;

                // Logic map tên: Class C# thường là PascalCase (TotalPlots), JSON là camelCase (totalPlots)
                string camelKey = char.ToLowerInvariant(key[0]) + key.Substring(1);

                FirestoreValue value = null;
                if (fields.ContainsKey(key)) value = fields[key];
                else if (fields.ContainsKey(camelKey)) value = fields[camelKey];

                if (value == null) continue; // Không tìm thấy dữ liệu thì bỏ qua

                // 2. Set giá trị vào property
                object parsedValue = ParseValue(prop.PropertyType, value);
                if (parsedValue != null)
                {
                    prop.SetValue(obj, parsedValue);
                }
            }

            return obj;
        }

        // Hàm đệ quy để xử lý từng loại dữ liệu
        private static object ParseValue(Type targetType, FirestoreValue fsValue)
        {
            // Case 1: Xử lý Null
            if (fsValue == null) return null;
            // Kiểm tra nullValue của Firestore (nếu bạn có dùng)
            // if (fsValue.NullValue != null) return null; 

            // Case 2: Các kiểu nguyên thủy (Primitive)
            if (targetType == typeof(string)) return fsValue.StringValue;

            if (targetType == typeof(int) || targetType == typeof(long))
                return int.Parse(fsValue.IntegerValue ?? "0");

            if (targetType == typeof(float) || targetType == typeof(double))
                return fsValue.DoubleValue;

            if (targetType == typeof(bool)) return fsValue.BooleanValue;

            if (targetType == typeof(DateTime))
            {
                // Xử lý Timestamp string của Google về DateTime C#
                if (DateTime.TryParse(fsValue.TimestampValue, out DateTime dt)) return dt;
                return DateTime.Now;
            }

            // Case 3: Xử lý List/Array
            if (typeof(IList).IsAssignableFrom(targetType) && fsValue.ArrayValue?.Values != null)
            {
                // Tạo một List mới dựa trên kiểu dữ liệu của List trong Class gốc
                var listType = targetType.GetGenericArguments()[0]; // Lấy kiểu T trong List<T>
                var listInstance = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));

                foreach (var item in fsValue.ArrayValue.Values)
                {
                    listInstance.Add(ParseValue(listType, item));
                }
                return listInstance;
            }

            // Case 4: Xử lý Object lồng nhau (Nested Object - ví dụ: Plot trong Farmland)
            if (fsValue.MapValue?.Fields != null)
            {
                // Gọi đệ quy hàm ToObject cho object con
                MethodInfo method = typeof(FirestoreMapper).GetMethod("ToObject");
                MethodInfo generic = method.MakeGenericMethod(targetType);
                return generic.Invoke(null, new object[] { fsValue.MapValue.Fields });
            }

            return null;
        }

        public static List<T> MapCollectionToList<T>(FirestoreListResponse response) where T : new()
        {
            List<T> result = new List<T>();
            if (response == null || response.Documents == null) return result;

            foreach (var doc in response.Documents)
            {
                T item = FirestoreMapper.ToObject<T>(doc.Fields);

                // Mẹo: Nếu Class T của bạn có trường Id, bạn có thể map ID từ doc.Name vào đây
                // string docId = doc.Name.Split('/').Last();
                // ... gán id vào item ...

                result.Add(item);
            }
            return result;
        }
    }


    public static class FirestoreSerializer
    {
        /// <summary>
        /// Hàm chính: Biến Object C# thành Dictionary chuẩn Firestore
        /// Dùng để convert cả một Document hoàn chỉnh
        /// </summary>
        public static object ToFirestoreDocument(object obj, bool addCreationTime = false)
        {
            if (obj == null) return null;
            // 1. Convert object data sang Dictionary fields
            var firestoreFields = GetObjectFields(obj);

            // 2. Logic "Tiêm" createdAt (Chỉ chạy khi addCreationTime = true)
            if (addCreationTime)
            {
                // Kiểm tra để tránh ghi đè nếu trong dataObj đã có sẵn biến CreatedAt
                if (!firestoreFields.ContainsKey("updatedAt"))
                {
                    firestoreFields["updatedAt"] = new
                    {
                        timestampValue = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    };
                }
            }

            // 3. Trả về object hoàn chỉnh
            return new
            {
                fields = firestoreFields
            };
        }

        public static object ToFirestoreBatchDocument(string fullPath, object obj, bool addCreationTime = false)
        {
            if (obj == null) return null;
            // 1. Convert object data sang Dictionary fields
            var firestoreFields = GetObjectFields(obj);

            // 2. Logic "Tiêm" createdAt (Chỉ chạy khi addCreationTime = true)
            if (addCreationTime)
            {
                // Kiểm tra để tránh ghi đè nếu trong dataObj đã có sẵn biến CreatedAt
                if (!firestoreFields.ContainsKey("updatedAt"))
                {
                    firestoreFields["updatedAt"] = new
                    {
                        timestampValue = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    };
                }
            }

            // 3. Trả về object hoàn chỉnh
            return new
            {
                name = fullPath,
                fields = firestoreFields
            };
        }

        /// <summary>
        /// Hàm phụ: Tạo ra dictionary các fields (dùng cho mapValue hoặc root)
        /// </summary>
        private static Dictionary<string, object> GetObjectFields(object obj)
        {
            var fields = new Dictionary<string, object>();
            Type type = obj.GetType();

            // Lấy tất cả public properties
            foreach (PropertyInfo prop in type.GetProperties())
            {
                object value = prop.GetValue(obj);

                // Tự động chuyển tên biến PascalCase (Gold) -> camelCase (gold)
                string key = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);

                // Bỏ qua nếu giá trị null (hoặc bạn có thể dùng CreateValueWrapper để tạo nullValue)
                if (value == null)
                {
                    fields[key] = new { nullValue = (object)null };
                    continue;
                }

                fields[key] = CreateValueWrapper(value);
            }
            return fields;
        }

        /// <summary>
        /// Hàm đệ quy: Bọc giá trị vào wrapper (stringValue, integerValue...)
        /// </summary>
        private static object CreateValueWrapper(object value)
        {
            Type type = value.GetType();

            // 1. Xử lý String
            if (type == typeof(string))
            {
                return new { stringValue = value };
            }

            // 2. Xử lý Số nguyên (Firestore bắt buộc int phải để dưới dạng string)
            if (type == typeof(int) || type == typeof(long))
            {
                return new { integerValue = value.ToString() };
            }

            // 3. Xử lý Số thực
            if (type == typeof(float) || type == typeof(double))
            {
                return new { doubleValue = value };
            }

            // 4. Xử lý Boolean
            if (type == typeof(bool))
            {
                return new { booleanValue = value };
            }

            // 5. Xử lý DateTime -> Timestamp chuẩn ISO 8601
            if (type == typeof(DateTime))
            {
                return new { timestampValue = ((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss.fffZ") };
            }

            // 6. Xử lý List/Array -> arrayValue
            if (value is IList list)
            {
                var valuesList = new List<object>();
                foreach (var item in list)
                {
                    valuesList.Add(CreateValueWrapper(item));
                }
                return new { arrayValue = new { values = valuesList } };
            }

            // 7. Xử lý Object con / Struct (Vector3, PlayerSkill...) -> mapValue
            // Nếu không phải các kiểu trên, coi nó là một Object lồng nhau
            return new
            {
                mapValue = new
                {
                    fields = GetObjectFields(value)
                }
            };
        }
    }

    public static class InitialDataFactory
    {
        public static PlayerData CreatePlayerData(string playerId)
        {
            return new PlayerData
            {
                UserId = playerId,
            };
        }

        public static Farmland CreateFarmlandData(string playerId)
        {
            return new Farmland
            {
                UserId = playerId
            };
        }

        public static AnimalFarm CreateAnimalFarmData(string playerId)
        {
            return new AnimalFarm
            {
                UserId = playerId
            };
        }

        public static Fishing CreateFishingData(string playerId)
        {
            return new Fishing
            {
                UserId = playerId
            };
        }
    }


    static public List<FirestoreBatchGetResponse> ParseBatchGetResponse(string ndjson)
    {
        List<FirestoreBatchGetResponse> list = new List<FirestoreBatchGetResponse>();

        string[] lines = ndjson.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var obj = JsonConvert.DeserializeObject<FirestoreBatchGetResponse>(line);

            if (obj != null)
                list.Add(obj);
        }

        return list;
    }

    public class FirestoreBatchGetResponse
    {
        [JsonProperty("found")]
        public FirestoreFound Found { get; set; }

        [JsonProperty("missing")]
        public string Missing { get; set; }

        [JsonProperty("readTime")]
        public string ReadTime { get; set; }
    }

    public class FirestoreFound
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fields")]
        public Dictionary<string, FirestoreValue> Fields { get; set; }
    }

    public class FirestoreValue
    {
        [JsonProperty("stringValue")]
        public string StringValue { get; set; }

        [JsonProperty("integerValue")]
        public string IntegerValue { get; set; }

        [JsonProperty("doubleValue")]
        public double DoubleValue { get; set; }

        [JsonProperty("booleanValue")]
        public bool BooleanValue { get; set; }

        [JsonProperty("timestampValue")]
        public string TimestampValue { get; set; }

        [JsonProperty("mapValue")]
        public FirestoreMap MapValue { get; set; }

        [JsonProperty("arrayValue")]
        public FirestoreArray ArrayValue { get; set; }
    }

    public class FirestoreMap
    {
        [JsonProperty("fields")]
        public Dictionary<string, FirestoreValue> Fields { get; set; }
    }

    public class FirestoreArray
    {
        [JsonProperty("values")]
        public List<FirestoreValue> Values { get; set; }
    }

    public class FirestoreListResponse
    {
        [JsonProperty("documents")]
        public List<FirestoreFound> Documents { get; set; }
    }


    //[Serializable]
    //public class FirestoreDocument
    //{
    //    public string name;
    //    public Dictionary<string, FirestoreValue> fields;
    //    public string createTime;
    //    public string updateTime;
    //}

    //[Serializable]
    //public class FirestoreValue
    //{
    //    public string stringValue;
    //    public string integerValue;
    //    public double doubleValue;
    //    public bool booleanValue;
    //    public string timestampValue;
    //    public FirestoreMapValue mapValue;
    //    public FirestoreArrayValue arrayValue;
    //}

    //[Serializable]
    //public class FirestoreMapValue
    //{
    //    public Dictionary<string, FirestoreValue> fields;
    //}

    //[Serializable]
    //public class FirestoreArrayValue
    //{
    //    public List<FirestoreValue> values;
    //}

    //[Serializable]
    //public class QueryResult
    //{
    //    public FirestoreDocument document;
    //}

    public class BatchOperation
    {
        public string Collection;
        public string DocumentId;
        public BatchOperationType Type;
        public Dictionary<string, object> FirestoreFields;
    }

    public enum BatchOperationType
    {
        Update,
        Delete
    }

}

