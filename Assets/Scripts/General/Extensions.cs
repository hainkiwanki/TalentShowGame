using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Extensions
{
    #region FloatExtensions
    public static float Flip(this float _t)
    {
        return 1 - _t;
    }

    public static float Square(this float _t)
    {
        return _t * _t;
    }

    public static float EaseOut(this float _t)
    {
        return Flip(Square(Flip(_t)));
    }

    public static float EaseInEaseOut(this float _t)
    {
        return _t * _t * (3.0f - 2.0f * _t);
    }
    #endregion

    #region StringExtensions
    public static string Capitalize(this string _s)
    {
        switch(_s)
        {
            case null: throw new ArgumentNullException(nameof(_s));
            case "": throw new ArgumentException($"{nameof(_s)} cannot be empty", nameof(_s));
            default: return _s.First().ToString().ToUpper() + _s.Substring(1).ToLower();
        }
    }
    #endregion

    #region Matrix4x4Extensions
    public static Matrix4x4 MatrixLerp(Matrix4x4 _from, Matrix4x4 _to, float _t)
    {
        _t = Mathf.Clamp(_t, 0.0f, 1.0f);
        var newMatrix = new Matrix4x4();
        newMatrix.SetRow(0, Vector4.Lerp(_from.GetRow(0), _to.GetRow(0), _t));
        newMatrix.SetRow(1, Vector4.Lerp(_from.GetRow(1), _to.GetRow(1), _t));
        newMatrix.SetRow(2, Vector4.Lerp(_from.GetRow(2), _to.GetRow(2), _t));
        newMatrix.SetRow(3, Vector4.Lerp(_from.GetRow(3), _to.GetRow(3), _t));
        return newMatrix;
    }
    #endregion

    #region Vector3Extensions
    public static Vector3 NewX(this Vector3 _v, float _x)
    {
        return new Vector3(_x, _v.y, _v.z);
    }

    public static Vector3 NewY(this Vector3 _v, float _y)
    {
        return new Vector3(_v.x, _y, _v.z);
    }
    
    public static Vector3 KeepY(this Vector3 _v, Vector3 _pos)
    {
        _v = new Vector3(_pos.x, _v.y, _pos.z);
        return _v;
    }

    public static Vector3 NewZ(this Vector3 _v, float _z)
    {
        return new Vector3(_v.x, _v.y, _z);
    }

    public static Vector3 Abs(this Vector3 _v)
    {
        return new Vector3(Mathf.Abs(_v.x), Mathf.Abs(_v.y), Mathf.Abs(_v.z));
    }

    public static Vector3 NewZ(this Vector2 _v, float _z = 0.0f)
    {
        return new Vector3(_v.x, _v.y, _z);
    }

    public static Vector3 StayWithinScreenBounds(this Vector3 _v, Vector2 _size, float _offset = 5.0f)
    {
        if (_v.x + _size.x + _offset > Screen.width)
            _v.x = Screen.width - _size.x - _offset;

        if (_v.x < 0.0f)
            _v.x = _offset;

        if (_v.y + _size.y + _offset> Screen.height)
            _v.y = Screen.height - _size.y - _offset;

        if (_v.y < 0.0f)
            _v.y = _offset;

        return _v;
    }

    public static Vector3 AddY(this Vector3 _v, float _f)
    {
        _v.y += _f;
        return _v;
    }
    #endregion

    #region Vector2Extensions
    public static Vector2 AddY(this Vector2 _v, float _f)
    {
        _v.y += _f;
        return _v;
    }

    public static Vector2 NewX(this Vector2 _v, float _f)
    {
        return new Vector2(_f, _v.y);
    }

    public static Vector2 NewY(this Vector2 _v, float _f)
    {
        return new Vector2(_v.x, _f);
    }
    #endregion

    #region ColorExtensions
    public static Color NewA(this Color _c, float _a)
    {
        return new Color(_c.r, _c.g, _c.b, _a);
    }
    #endregion

    public static int RoundFloatToInt(float _x)
    {
        var decimalX = _x % 1.0f;
        if (decimalX > 0.5f)
            return Mathf.CeilToInt(_x);
        else
            return Mathf.FloorToInt(_x);
    }

    public static int ClampIntToVectorSize<T>(int _i, List<T> _list)
    {
        if (_i < 0) _i = _list.Count - 1;
        if (_i >= _list.Count) _i = 0;
        return _i;
    }

    public static Vector3 CrossProductThreePoints(Vector3 _p1, Vector3 _p2, Vector3 _c)
    {
        Vector3 up = _p1 - _c;
        Vector3 right = _p2 - _c;
        return Vector3.Cross(right, up);
    }

    public static Vector3 ReflectPointOverLineUsingZ(Vector3 _p, Vector3 _a, Vector3 _b, float _y = 0.0f)
    {
        // y = mx + c using points _a and _b
        float m = (_b.z - _a.z) / (_b.x - _a.x);
        float c = _a.z - m * _a.x;

        //det
        float det = (_p.x + (_p.z - c) * m) / (1 + m * m);

        float x = 2.0f * det - _p.x;
        float z = 2.0f * det * m - _p.z + 2.0f * c;

        return new Vector3(x, _y, z);
    }

    public static Vector3 FindLineLineIntersectionUsingZ(Vector3 _a, Vector3 _b, Vector3 _c, Vector3 _d, float _y = 0.0f)
    {
        // LINE AB as a1x + b1y = c1
        float a1 = _b.z - _a.z;
        float b1 = _a.x - _b.x;
        float c1 = a1 * (_a.x) + b1 * (_a.z);

        // LINE CD as a2x + b2y = c2
        float a2 = _d.z - _c.z;
        float b2 = _c.x - _d.x;
        float c2 = a2 * (_c.x) + b2 * (_c.z);

        float det = a1 * b2 - a2 * b1;
        if(det == 0)
        {
            return -Vector3.up;
        }
        else
        {
            float x = (b2 * c1 - b1 * c2) / det;
            float z = (a1 * c2 - a2 * c1) / det;
            return new Vector3(x, _y, z);
        }
    }

    public static Vector3 FindIntersection(Vector3 _p, Vector3 _p2, Vector3 _q, Vector3 _q2, float _y = 0.0f)
    {
        // https://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments
        float a1 = _p2.z - _p.z;
        float b1 = _p.x - _p2.x;
        float c1 = a1 * _p.x + b1 * _p.z;

        float a2 = _q2.z - _q.z;
        float b2 = _q.x - _q2.x;
        float c2 = a2 * _q.x + b2 * _q.z;

        float delta = a1 * b2 - a2 * b1;
        Vector3 newPoint = new Vector3(
            (b2 * c1 - b1 * c2) / delta,
            _y,
            (a1 * c2 - a2 * c1) / delta
            );

        if (IsPointOnLine(_p, _p2, newPoint) && IsPointOnLine(_q, _q2, newPoint) && delta != 0)
            return newPoint;
        else
            return Vector3.up;
    }

    public static bool IsPointOnLine(Vector3 _start, Vector3 _end, Vector3 _point)
    {
        var sX = GetSmallest(_start.x, _end.x);
        var sZ = GetSmallest(_start.z, _end.z);
        var bX = GetBiggest(_start.x, _end.x);
        var bZ = GetBiggest(_start.z, _end.z);
        if (sX <= _point.x && _point.x <= bX &&
            sZ <= _point.z && _point.z <= bZ)
            return true;
        return false;
    }

    public static float GetSmallest(float _x, float _y)
    {
        return (_x < _y) ? _x : _y;
    }

    public static float GetBiggest(float _x, float _y)
    {
        return (_x > _y) ? _x : _y;
    }

    public static void SetLayerRecursively(GameObject _obj, int _layer, bool _skipParent = false)
    {
        if (_obj == null) return;

        if(!_skipParent)
            _obj.layer = _layer;

        foreach (Transform child in _obj.transform)
        {
            if (child == null)
                continue;
            SetLayerRecursively(child.gameObject, _layer);
        }
    }

    public static void SaveTexture2DToPNG(Texture2D _tex, string _directory, string _name)
    {
        //\AppData\Local\Packages\<productname>\LocalState.
        byte[] bytes = _tex.EncodeToPNG();
        string fullpath = Application.persistentDataPath + "/";
        if (_directory != null || _directory != "")
        {
            fullpath += _directory;
            if (!Directory.Exists(fullpath))
                Directory.CreateDirectory(fullpath);

            if (_directory[_directory.Length - 1] != '/')
                fullpath += "/";
        }

        fullpath += _name + ".png";

        System.IO.File.WriteAllBytes(fullpath, bytes);
        // Debug.Log("png saved in " + fullpath);
    }

    public static bool LoadTexture2DFromPNG(string _path, out Texture2D _tex)
    {
        _tex = null;
        if(!File.Exists(_path))
        {
            Debug.LogError("Cannot find texture in path " + _path);
            return false;
        }

        byte[] bytes = File.ReadAllBytes(_path);
        _tex = new Texture2D(1, 1);
        _tex.LoadImage(bytes);
        return true;
    }

    public static GameObject FindChildWithTag(this GameObject _parent, string _tag)
    { 
        var allChildren = _parent.GetComponentsInChildren<Transform>(true);

        foreach(var child in allChildren)
        {
            if (child.tag == _tag)
                return child.gameObject;
        }

        return null;
    }

    public static T FindChildWithNameRecusively<T>(this Transform _parent, string _name)
    {
        T obj = default(T);

        foreach (Transform child in _parent.transform)
        {
            if (child == null)
                continue;

            if (child.name == _name)
            {
                return child.GetComponent<T>();
            }

            obj = FindChildWithNameRecusively<T>(child, _name);

            if (obj != null)
                break;
        }

        return obj;
    }

    public static bool ContainsNullObject<T>(this List<T> _list)
    {
        foreach (T obj in _list)
            if (obj == null)
                return true;

        return false;
    }

    public static T GetRandom<T>(this List<T> _list)
    {
        if (_list == null || _list.Count == 0)
            return default(T);
        if (_list.Count == 1)
            return _list[0];
        int index = UnityEngine.Random.Range(0, _list.Count);
        return _list[index];
    }

    public static T RemoveRandom<T>(this List<T> _list)
    {
        if (_list.Count <= 0)
            return default(T);
        T result = _list.GetRandom();
        _list.Remove(result);
        return result;
    }

    public static Vector3 GetRandomPointInCircle(this CircleCollider2D _col)
    {
        Vector3 res = _col.transform.position;
        Vector3 center = _col.transform.position;
        float radius = UnityEngine.Random.Range(0.0f, _col.radius);
        float angle = UnityEngine.Random.Range(0.0f, 360.0f);
        float x = center.x + radius * Mathf.Cos(angle);
        float y = center.y + radius * Mathf.Sin(angle);
        res.x = x;
        res.y = y;
        return res;
    }

    public static Vector3 GetRandomPointOnEdge(this CircleCollider2D _col)
    {
        Vector3 res = _col.transform.position;
        Vector3 center = _col.transform.position;
        float angle = UnityEngine.Random.Range(0.0f, 360.0f);
        float x = center.x + _col.radius * Mathf.Cos(angle);
        float y = center.y + _col.radius * Mathf.Sin(angle);
        res.x = x;
        res.y = y;
        return res;
    }

    public static Vector3 GetRandomPointOnCircle(Vector3 _center, float _radius, float _radiusMargin = 0.0f)
    {
        Vector3 res = Vector3.zero;

        float angle = UnityEngine.Random.Range(0.0f, 360.0f);
        float radius = UnityEngine.Random.Range(_radius, _radius + _radiusMargin);
        res.x = _center.x + radius * Mathf.Cos(angle);
        res.z = _center.z + radius * Mathf.Sin(angle);

        return res;
    }

    public static Vector3 GetRandonPointOnCircleEdge(this SphereCollider _col, float _radiusMargin = 0.0f)
    {
        Vector3 center = _col.transform.position;
        Vector3 res = Vector3.zero;

        float angle = UnityEngine.Random.Range(0.0f, 360.0f);
        float radius = UnityEngine.Random.Range(_col.radius, _col.radius + _radiusMargin);
        res.x = center.x + radius * Mathf.Cos(angle);
        res.z = center.z + radius * Mathf.Sin(angle);

        return res;
    }

    public static Quaternion GetRandomZRotation()
    {
        return Quaternion.Euler(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360.0f));
    }

    public static Vector3 GetRandomVectorZRotation()
    {
        return new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360.0f));
    }

    public static Vector2 To2D(this Vector3 _v)
    {
        return new Vector2(_v.x, _v.y);
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static T Random<T>(this HashSet<T> hs)
    {
        int max = hs.Count;
        int index = UnityEngine.Random.Range(0, max);

        int i = 0;
        foreach(var t in hs)
        {
            if (i == index)
                return t;
            i++;
        }

        return default(T);
    }

    public static bool ChanceRoll(float _goal)
    {
        return UnityEngine.Random.Range(1.0f, 100.0f) >= _goal;
    }

    public static Vector3 GetRandomPositionInCollider(this BoxCollider _collider)
    {
        Bounds bounds = _collider.bounds;
        return new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
    }

    #region Serialization
    public static byte[] ToBytes(this Color _c)
    {
        List<byte> bytes = new List<byte>();

        bytes.AddRange(BitConverter.GetBytes(_c.r));
        bytes.AddRange(BitConverter.GetBytes(_c.g));
        bytes.AddRange(BitConverter.GetBytes(_c.b));
        bytes.AddRange(BitConverter.GetBytes(_c.a));

        return bytes.ToArray();
    }

    public static byte[] ToBytes(this Vector3 _v)
    {
        List<byte> bytes = new List<byte>();

        bytes.AddRange(BitConverter.GetBytes(_v.x));
        bytes.AddRange(BitConverter.GetBytes(_v.y));
        bytes.AddRange(BitConverter.GetBytes(_v.z));

        return bytes.ToArray();
    }

    public static byte[] ToBytes(this Quaternion _q)
    {
        List<byte> bytes = new List<byte>();

        bytes.AddRange(BitConverter.GetBytes(_q.x));
        bytes.AddRange(BitConverter.GetBytes(_q.y));
        bytes.AddRange(BitConverter.GetBytes(_q.z));
        bytes.AddRange(BitConverter.GetBytes(_q.w));

        return bytes.ToArray();
    }

    public static byte[] ToBytes(this string _s)
    {
        List<byte> bytes = new List<byte>();

        bytes.Add((byte)_s.Length);
        bytes.AddRange(System.Text.Encoding.ASCII.GetBytes(_s));

        return bytes.ToArray();
    }


    public static void SeekString(this BinaryReader _reader)
    {
        byte length = _reader.ReadByte();
        _reader.BaseStream.Seek(length, SeekOrigin.Current);
    }

    public static string ReadString(this BinaryReader _reader)
    {
        byte length = _reader.ReadByte();
        string str = new string(_reader.ReadChars(length));
        return str;
    }

    public static Vector3 ReadVector3(this BinaryReader _reader)
    {
        Vector3 pos = new Vector3()
        {
            x = _reader.ReadSingle(),
            y = _reader.ReadSingle(),
            z = _reader.ReadSingle()
        };
        return pos;
    }

    public static Quaternion ReadQuaternion(this BinaryReader _reader)
    {
        Quaternion quat = new Quaternion()
        {
            x = _reader.ReadSingle(),
            y = _reader.ReadSingle(),
            z = _reader.ReadSingle(),
            w = _reader.ReadSingle()
        };
        return quat;
    }

    public static Color ReadColor(this BinaryReader _reader)
    {
        Color col = new Color()
        {
            r = _reader.ReadSingle(),
            g = _reader.ReadSingle(),
            b = _reader.ReadSingle(),
            a = _reader.ReadSingle()
        };
        return col;
    }
    #endregion
}
