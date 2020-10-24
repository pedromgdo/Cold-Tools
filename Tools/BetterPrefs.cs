using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ColdHours.Utils
{
    public class BetterPrefs : PlayerPrefs
    {
        /*
         * TODO: Make it more readable and 
         * maybe simply use setObject for
         * most of these, and then cast object
         * when trying to get a specific one
         */
        private enum ArrayType { Float, Int32, Bool, String, Vector2, Vector3, Quaternion, Color }

        static private int endianDiff1;
        static private int endianDiff2;
        static private int idx;
        static private byte[] byteBlock;

        #region Bool
        public static void SetBool(string saveName, bool b) {
            SetInt(saveName, b ? 1 : 0);
        }
        public static bool GetBool(string saveName) {
            int r = GetInt(saveName);
            return r == 1 ? true : false;
        }
        public static bool GetBool(string saveName, bool defaultB) {
            if (!HasKey(saveName)) return defaultB;
            int r = GetInt(saveName);
            return r == 1 ? true : false;
        }
        #endregion

        #region Long
        public static void SetLong(string key, long value) {
            int[] lArray = new int[2];
            lArray[0] = (int)(uint)(ulong)value;
            lArray[1] = (int)(uint)(value >> 32);
            SetIntArray(key, lArray);
        }
        public static long GetLong(string key) {
            int[] lArray = GetIntArray(key);
            if (lArray.Length != 2) return 0;

            // unsigned, to prevent loss of sign bit.
            ulong ret = (uint)lArray[1];
            ret = (ret << 32);
            return (long)(ret | (ulong)(uint)lArray[0]);
        }
        public static long GetLong(string key, long defaultValue) {
            if (HasKey(key))
                return GetLong(key);
            return defaultValue;
        }
        #endregion

        #region Color
        public static void SetColor(string key, Color color) {
            SetFloatArray(key, new float[] { color.r, color.g, color.b, color.a });
        }
        public static Color GetColor(string key) {
            var floatArray = GetFloatArray(key);
            if (floatArray.Length < 4)
                return new Color(0.0f, 0.0f, 0.0f, 0.0f);
            return new Color(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
        }
        public static Color GetColor(string key, Color defaultValue) {
            if (PlayerPrefs.HasKey(key))
                return GetColor(key);
            return defaultValue;
        }
        #endregion

        #region Vector2
        public static void SetVector2(string key, Vector2 vector) {
            SetFloatArray(key, new float[] { vector.x, vector.y });
        }

        static Vector2 GetVector2(string key) {
            var floatArray = GetFloatArray(key);
            if (floatArray.Length < 2) {
                return Vector2.zero;
            }
            return new Vector2(floatArray[0], floatArray[1]);
        }

        public static Vector2 GetVector2(string key, Vector2 defaultValue) {
            if (HasKey(key)) {
                return GetVector2(key);
            }
            return defaultValue;
        }
        #endregion

        #region Vector3
        public static void SetVector3(string key, Vector3 vector) {
            SetFloatArray(key, new float[] { vector.x, vector.y, vector.z });
        }

        public static Vector3 GetVector3(string key) {
            var floatArray = GetFloatArray(key);
            if (floatArray.Length < 3) {
                return Vector3.zero;
            }
            return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
        }

        public static Vector3 GetVector3(string key, Vector3 defaultValue) {
            if (PlayerPrefs.HasKey(key)) {
                return GetVector3(key);
            }
            return defaultValue;
        }
        #endregion

        #region Quaternion
        public static void SetQuaternion(string key, Quaternion vector) {
            SetFloatArray(key, new float[] { vector.x, vector.y, vector.z, vector.w });
        }

        public static Quaternion GetQuaternion(string key) {
            var floatArray = GetFloatArray(key);
            if (floatArray.Length < 4)
                return Quaternion.identity;
            return new Quaternion(floatArray[0], floatArray[1], floatArray[2], floatArray[3]);
        }

        public static Quaternion GetQuaternion(string key, Quaternion defaultValue) {
            if (HasKey(key))
                return GetQuaternion(key);
            return defaultValue;
        }
        #endregion

        #region Bool Array
        public static void SetBoolArray(string key, bool[] boolArray) {
            if (boolArray.Length == 0) {
                Debug.LogError("The bool array cannot have 0 entries when setting " + key);
                return;
            }
            // Make a byte array that's a multiple of 8 in length, plus 5 bytes to store the number of entries as an int32 (+ identifier)
            // We have to store the number of entries, since the boolArray length might not be a multiple of 8, so there could be some padded zeroes
            var bytes = new byte[(boolArray.Length + 7) / 8 + 5];
            bytes[0] = Convert.ToByte(ArrayType.Bool);  // Identifier
            var bits = new BitArray(boolArray);
            bits.CopyTo(bytes, 5);
            Initialize();
            ConvertInt32ToBytes(boolArray.Length, bytes); // The number of entries in the boolArray goes in the first 4 bytes

            SetBytes(key, bytes);
        }

        public static bool[] GetBoolArray(string key) {
            if (HasKey(key)) {
                var bytes = Convert.FromBase64String(GetString(key));
                if (bytes.Length < 6) {
                    Debug.LogError("Corrupt preference file for " + key);
                    return new bool[0];
                }
                if ((ArrayType)bytes[0] != ArrayType.Bool) {
                    Debug.LogError(key + " is not a boolean array");
                    return new bool[0];
                }
                Initialize();

                // Make a new bytes array that doesn't include the number of entries + identifier (first 5 bytes) and turn that into a BitArray
                var bytes2 = new byte[bytes.Length - 5];
                Array.Copy(bytes, 5, bytes2, 0, bytes2.Length);
                var bits = new BitArray(bytes2);
                // Get the number of entries from the first 4 bytes after the identifier and resize the BitArray to that length, then convert it to a boolean array
                bits.Length = ConvertBytesToInt32(bytes);
                var boolArray = new bool[bits.Count];
                bits.CopyTo(boolArray, 0);

                return boolArray;
            }
            return new bool[0];
        }

        public static bool[] GetBoolArray(string key, bool defaultValue, int defaultSize) {
            if (PlayerPrefs.HasKey(key)) {
                return GetBoolArray(key);
            }
            var boolArray = new bool[defaultSize];
            for (int i = 0; i < defaultSize; i++) {
                boolArray[i] = defaultValue;
            }
            return boolArray;
        }
        #endregion

        #region String Array
        public static void SetStringArray(string key, string[] stringArray) {
            if (stringArray.Length == 0) {
                Debug.LogError("The string array cannot have 0 entries when setting " + key);
                return;
            }
            var bytes = new byte[stringArray.Length + 1];
            bytes[0] = Convert.ToByte(ArrayType.String);    // Identifier
            Initialize();

            // Store the length of each string that's in stringArray, so we can extract the correct strings in GetStringArray
            for (var i = 0; i < stringArray.Length; i++) {
                if (stringArray[i] == null) {
                    Debug.LogError("Can't save null entries in the string array when setting " + key);
                    return;
                }
                if (stringArray[i].Length > 255) {
                    Debug.LogError("Strings cannot be longer than 255 characters when setting " + key);
                    return;
                }
                bytes[idx++] = (byte)stringArray[i].Length;
            }

            SetString(key, Convert.ToBase64String(bytes) + "|" + string.Join("", stringArray));
        }

        public static string[] GetStringArray(string key) {
            if (HasKey(key)) {
                var completeString = GetString(key);
                var separatorIndex = completeString.IndexOf("|"[0]);
                if (separatorIndex < 4) {
                    Debug.LogError("Corrupt preference file for " + key);
                    return new string[0];
                }
                var bytes = System.Convert.FromBase64String(completeString.Substring(0, separatorIndex));
                if ((ArrayType)bytes[0] != ArrayType.String) {
                    Debug.LogError(key + " is not a string array");
                    return new string[0];
                }
                Initialize();

                var numberOfEntries = bytes.Length - 1;
                var stringArray = new string[numberOfEntries];
                var stringIndex = separatorIndex + 1;
                for (var i = 0; i < numberOfEntries; i++) {
                    int stringLength = bytes[idx++];
                    if (stringIndex + stringLength > completeString.Length) {
                        Debug.LogError("Corrupt preference file for " + key);
                        return new string[0];
                    }
                    stringArray[i] = completeString.Substring(stringIndex, stringLength);
                    stringIndex += stringLength;
                }

                return stringArray;
            }
            return new string[0];
        }

        public static string[] GetStringArray(string key, string defaultValue, int defaultSize) {
            if (HasKey(key)) {
                return GetStringArray(key);
            }
            var stringArray = new string[defaultSize];
            for (int i = 0; i < defaultSize; i++) {
                stringArray[i] = defaultValue;
            }
            return stringArray;
        }
        #endregion

        #region Int Array
        public static void SetIntArray(string key, int[] intArray) {
            SetValue(key, intArray, ArrayType.Int32, 1, ConvertFromInt);
        }

        public static int[] GetIntArray(string key) {
            var intList = new List<int>();
            GetValue(key, intList, ArrayType.Int32, 1, ConvertToInt);
            return intList.ToArray();
        }

        public static int[] GetIntArray(string key, int defaultValue, int defaultSize) {
            if (HasKey(key)) {
                return GetIntArray(key);
            }
            var intArray = new int[defaultSize];
            for (int i = 0; i < defaultSize; i++) {
                intArray[i] = defaultValue;
            }
            return intArray;
        }
        #endregion

        #region Float Array
        public static void SetFloatArray(string key, float[] floatArray) {
            SetValue(key, floatArray, ArrayType.Float, 1, ConvertFromFloat);
        }
        public static float[] GetFloatArray(string key) {
            var floatList = new List<float>();
            GetValue(key, floatList, ArrayType.Float, 1, ConvertToFloat);
            return floatList.ToArray();
        }

        public static float[] GetFloatArray(string key, float defaultValue, int defaultSize) {
            if (HasKey(key)) {
                return GetFloatArray(key);
            }
            var floatArray = new float[defaultSize];
            for (int i = 0; i < defaultSize; i++) {
                floatArray[i] = defaultValue;
            }
            return floatArray;
        }
        #endregion

        #region Vector2 Array
        public static void SetVector2Array(string key, Vector2[] vector2Array) {
            SetValue(key, vector2Array, ArrayType.Vector2, 2, ConvertFromVector2);
        }
        public static Vector2[] GetVector2Array(string key) {
            var vector2List = new List<Vector2>();
            GetValue(key, vector2List, ArrayType.Vector2, 2, ConvertToVector2);
            return vector2List.ToArray();
        }

        public static Vector2[] GetVector2Array(string key, Vector2 defaultValue, int defaultSize) {
            if (PlayerPrefs.HasKey(key)) {
                return GetVector2Array(key);
            }
            var vector2Array = new Vector2[defaultSize];
            for (int i = 0; i < defaultSize; i++) {
                vector2Array[i] = defaultValue;
            }
            return vector2Array;
        }

        #endregion

        #region Vector3 Array
        public static void SetVector3Array(string key, Vector3[] vector3Array) {
            SetValue(key, vector3Array, ArrayType.Vector3, 3, ConvertFromVector3);
        }
        public static Vector3[] GetVector3Array(string key) {
            var vector3List = new List<Vector3>();
            GetValue(key, vector3List, ArrayType.Vector3, 3, ConvertToVector3);
            return vector3List.ToArray();
        }

        public static Vector3[] GetVector3Array(string key, Vector3 defaultValue, int defaultSize) {
            if (PlayerPrefs.HasKey(key)) {
                return GetVector3Array(key);
            }
            var vector3Array = new Vector3[defaultSize];
            for (int i = 0; i < defaultSize; i++) {
                vector3Array[i] = defaultValue;
            }
            return vector3Array;
        }
        #endregion

        #region Quaternion Array
        public static void SetQuaternionArray(string key, Quaternion[] quaternionArray) {
            SetValue(key, quaternionArray, ArrayType.Quaternion, 4, ConvertFromQuaternion);
        }
        public static Quaternion[] GetQuaternionArray(string key) {
            var quaternionList = new List<Quaternion>();
            GetValue(key, quaternionList, ArrayType.Quaternion, 4, ConvertToQuaternion);
            return quaternionList.ToArray();
        }

        public static Quaternion[] GetQuaternionArray(string key, Quaternion defaultValue, int defaultSize) {
            if (PlayerPrefs.HasKey(key)) {
                return GetQuaternionArray(key);
            }
            var quaternionArray = new Quaternion[defaultSize];
            for (int i = 0; i < defaultSize; i++) {
                quaternionArray[i] = defaultValue;
            }
            return quaternionArray;
        }
        #endregion

        #region Color Array
        public static void SetColorArray(string key, Color[] colorArray) {
            SetValue(key, colorArray, ArrayType.Color, 4, ConvertFromColor);
        }
        public static Color[] GetColorArray(string key) {
            var colorList = new List<Color>();
            GetValue(key, colorList, ArrayType.Color, 4, ConvertToColor);
            return colorList.ToArray();
        }
        public static Color[] GetColorArray(string key, Color defaultValue, int defaultSize) {
            if (HasKey(key)) {
                return GetColorArray(key);
            }
            var colorArray = new Color[defaultSize];
            for (int i = 0; i < defaultSize; i++) {
                colorArray[i] = defaultValue;
            }
            return colorArray;
        }
        #endregion

        #region Bytes Array
        public static void SetBytes(string key, byte[] bytes) {
            SetString(key, Convert.ToBase64String(bytes));
        }
        public static byte[] GetBytes(string key) {
            return Convert.FromBase64String(GetString(key));
        }
        public static byte[] GetBytes(string key, byte[] defaultValue) {
            if (HasKey(key))
                return GetBytes(key);
            return defaultValue;
        }
        #endregion

        #region Object
        public static void SetObject<T>(string key, T value) {
            try {
                SetBytes(key, ObjectToByteArray(value));
            }
            catch(System.Runtime.Serialization.SerializationException) {
                Debug.LogError("Error: Could not save as Object was not marked as Serializable.");
            }
        }
        public static T GetObject<T>(string key) {
            T result;
            try {
                result = ByteArrayToObject<T>(GetBytes(key));
            }
            catch {
                return default;
            }
            return result;
        }
        public static T GetObject<T>(string key, T defaultValue) {
            if (HasKey(key))
                return GetObject<T>(key);
            return defaultValue;
        }
        private static byte[] ObjectToByteArray<T>(T obj) {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream()) {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // Convert a byte array to an Object
        private static T ByteArrayToObject<T>(byte[] arrBytes) {
            using (MemoryStream memStream = new MemoryStream()) {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                T obj = (T)binForm.Deserialize(memStream);

                return obj;
            }
        }
        #endregion

        #region Action Converters
        private static void ConvertFromInt(int[] array, byte[] bytes, int i) {
            ConvertInt32ToBytes(array[i], bytes);
        }

        private static void ConvertFromFloat(float[] array, byte[] bytes, int i) {
            ConvertFloatToBytes(array[i], bytes);
        }

        private static void ConvertFromVector2(Vector2[] array, byte[] bytes, int i) {
            ConvertFloatToBytes(array[i].x, bytes);
            ConvertFloatToBytes(array[i].y, bytes);
        }

        private static void ConvertFromVector3(Vector3[] array, byte[] bytes, int i) {
            ConvertFloatToBytes(array[i].x, bytes);
            ConvertFloatToBytes(array[i].y, bytes);
            ConvertFloatToBytes(array[i].z, bytes);
        }

        private static void ConvertFromQuaternion(Quaternion[] array, byte[] bytes, int i) {
            ConvertFloatToBytes(array[i].x, bytes);
            ConvertFloatToBytes(array[i].y, bytes);
            ConvertFloatToBytes(array[i].z, bytes);
            ConvertFloatToBytes(array[i].w, bytes);
        }

        private static void ConvertFromColor(Color[] array, byte[] bytes, int i) {
            ConvertFloatToBytes(array[i].r, bytes);
            ConvertFloatToBytes(array[i].g, bytes);
            ConvertFloatToBytes(array[i].b, bytes);
            ConvertFloatToBytes(array[i].a, bytes);
        }
        #endregion

        private static void SetValue<T>(string key, T array, ArrayType arrayType, int vectorNumber, Action<T, byte[], int> convert) where T : IList {
            if (array.Count == 0) {
                Debug.LogError("The " + arrayType.ToString() + " array cannot have 0 entries when setting " + key);
                return;
            }
            var bytes = new byte[(4 * array.Count) * vectorNumber + 1];
            bytes[0] = Convert.ToByte(arrayType);   // Identifier
            Initialize();

            for (var i = 0; i < array.Count; i++) {
                convert(array, bytes, i);
            }
            SetBytes(key, bytes);
        }

        private static void GetValue<T>(string key, T list, ArrayType arrayType, int vectorNumber, Action<T, byte[]> convert) where T : IList {
            if (HasKey(key)) {
                var bytes = GetBytes(key);
                if ((bytes.Length - 1) % (vectorNumber * 4) != 0) {
                    Debug.LogError("Corrupt preference file for " + key);
                    return;
                }
                if ((ArrayType)bytes[0] != arrayType) {
                    Debug.LogError(key + " is not a " + arrayType.ToString() + " array");
                    return;
                }
                Initialize();

                var end = (bytes.Length - 1) / (vectorNumber * 4);
                for (var i = 0; i < end; i++) {
                    convert(list, bytes);
                }
            }
        }

        #region Type Converters
        private static void ConvertToInt(List<int> list, byte[] bytes) {
            list.Add(ConvertBytesToInt32(bytes));
        }

        private static void ConvertToFloat(List<float> list, byte[] bytes) {
            list.Add(ConvertBytesToFloat(bytes));
        }

        private static void ConvertToVector2(List<Vector2> list, byte[] bytes) {
            list.Add(new Vector2(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
        }

        private static void ConvertToVector3(List<Vector3> list, byte[] bytes) {
            list.Add(new Vector3(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
        }

        private static void ConvertToQuaternion(List<Quaternion> list, byte[] bytes) {
            list.Add(new Quaternion(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
        }

        private static void ConvertToColor(List<Color> list, byte[] bytes) {
            list.Add(new Color(ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes), ConvertBytesToFloat(bytes)));
        }

        private static void ConvertFloatToBytes(float f, byte[] bytes) {
            byteBlock = BitConverter.GetBytes(f);
            ConvertTo4Bytes(bytes);
        }

        private static float ConvertBytesToFloat(byte[] bytes) {
            ConvertFrom4Bytes(bytes);
            return BitConverter.ToSingle(byteBlock, 0);
        }

        private static void ConvertInt32ToBytes(int i, byte[] bytes) {
            byteBlock = BitConverter.GetBytes(i);
            ConvertTo4Bytes(bytes);
        }

        private static int ConvertBytesToInt32(byte[] bytes) {
            ConvertFrom4Bytes(bytes);
            return BitConverter.ToInt32(byteBlock, 0);
        }

        private static void ConvertTo4Bytes(byte[] bytes) {
            bytes[idx] = byteBlock[endianDiff1];
            bytes[idx + 1] = byteBlock[1 + endianDiff2];
            bytes[idx + 2] = byteBlock[2 - endianDiff2];
            bytes[idx + 3] = byteBlock[3 - endianDiff1];
            idx += 4;
        }

        private static void ConvertFrom4Bytes(byte[] bytes) {
            byteBlock[endianDiff1] = bytes[idx];
            byteBlock[1 + endianDiff2] = bytes[idx + 1];
            byteBlock[2 - endianDiff2] = bytes[idx + 2];
            byteBlock[3 - endianDiff1] = bytes[idx + 3];
            idx += 4;
        }
        #endregion

        #region Auxiliary Methods
        public static void ShowArrayType(string key) {
            var bytes = Convert.FromBase64String(GetString(key));
            if (bytes.Length > 0) {
                ArrayType arrayType = (ArrayType)bytes[0];
                Debug.Log(key + " is a " + arrayType.ToString() + " array");
            }
        }

        private static void Initialize() {
            if (BitConverter.IsLittleEndian) {
                endianDiff1 = 0;
                endianDiff2 = 0;
            }
            else {
                endianDiff1 = 3;
                endianDiff2 = 1;
            }
            if (byteBlock == null) {
                byteBlock = new byte[4];
            }
            idx = 1;
        }
        #endregion

    }
}