using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.AIM.DTO
{
    public class VersionInfo : IComparable<VersionInfo>
    {
        /// <summary>
        /// 第一位,主版本号
        /// </summary>
        public int MajorVersion { get; set; }
        /// <summary>
        /// 第二位,子版本号
        /// </summary>
        public int MinorVersion { get; set; }
        /// <summary>
        /// 第三位,修正版号
        /// </summary>
        public int RevisionVersion { get; set; }
        /// <summary>
        /// 第四位,构建版本号
        /// </summary>
        public int BuildVersion { get; set; }

        public VersionInfo(int v1, int v2, int v3, int v4)
        {
            this.MajorVersion = v1;
            this.MinorVersion = v2;
            this.RevisionVersion = v3;
            this.BuildVersion = v4;
        }

        public static VersionInfo ParseVersion(string val)
        {
            var temp = val.Split('.');
            if (temp.Length != 4)
                throw new FormatException("version format error: " + val);
            var temp2 = temp.Select(x => int.Parse(x)).ToList();
            return new VersionInfo(temp2[0], temp2[1], temp2[2], temp2[3]);
        }

        public override string ToString()
        {
            return $"{MajorVersion}.{MinorVersion}.{RevisionVersion}.{BuildVersion}";
        }

        public int CompareTo(VersionInfo y)
        {
            if (this.MajorVersion > y.MajorVersion)
                return 1;
            if (this.MajorVersion < y.MajorVersion)
                return -1;
            if (this.MinorVersion > y.MinorVersion)
                return 1;
            if (this.MinorVersion < y.MinorVersion)
                return -1;

            if (this.RevisionVersion > y.RevisionVersion)
                return 1;
            if (this.RevisionVersion < y.RevisionVersion)
                return -1;

            if (this.BuildVersion > y.BuildVersion)
                return 1;
            if (this.BuildVersion < y.BuildVersion)
                return -1;
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var temp = obj as VersionInfo;
            if (temp == null)
                return false;
            return this.MajorVersion == temp.MajorVersion && 
                this.MinorVersion == temp.MinorVersion && 
                this.RevisionVersion == this.RevisionVersion && 
                this.BuildVersion == temp.BuildVersion;
        }

        public static bool operator ==(VersionInfo left, VersionInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VersionInfo left, VersionInfo right)
        {
            return !left.Equals(right);
        }
    }
}
