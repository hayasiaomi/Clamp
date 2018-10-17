using ShanDain.AIM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDain.AIM.DownloadUnit
{

    internal class DownloadInfo
    {
        //ETag,或者其他用于判断正在下载的版本和曾经下载一半的版本是否一致的标识
        public string Tag { get; private set; }
        /// <summary>
        /// 完整性校验
        /// </summary>
        public byte[] CheckSum { get; private set; }
        /// <summary>
        /// 断点续传支持
        /// </summary>
        public bool SupportResume { get; private set; }

        /// <summary>
        /// 文件总大小(字节)
        /// </summary>
        public long FileSize { get; private set; }
        /// <summary>
        /// 下载偏移
        /// </summary>
        public long FileDownloaded { get; private set; }

        public bool IsComplited
        {
            get { return this.FileDownloaded == FileSize; }
        }

        public DownloadInfo(string tag, byte[] checkSum, bool supportResume, long posit, long size)
        {
            if (checkSum == null || checkSum.Length != 32)
                throw new CheckSumFormatException();
            if (tag.Length > 250)
                throw new Exception("TagLength too long, value:" + tag);
            this.Tag = tag;
            this.CheckSum = checkSum;
            if (size < 0)
                throw new ArgumentException(nameof(size));
            this.FileSize = size;
            if (posit < 0)
                throw new ArgumentException(nameof(posit));
            this.FileDownloaded = posit;
            this.SupportResume = supportResume;
        }

        public void WriteToFile(string tempfilepath)
        {
            var buffer = new MemoryStream();
            var magicNum = Encoding.ASCII.GetBytes("sddlf");
            buffer.Write(magicNum, 0, magicNum.Length);

            if (string.IsNullOrEmpty(this.Tag))
            {
                buffer.WriteByte(0x00);
            }
            else
            {
                var tagvalue = Encoding.ASCII.GetBytes(this.Tag);
                buffer.WriteByte((byte)tagvalue.Length);
                buffer.Write(tagvalue, 0, tagvalue.Length);
            }

            buffer.Write(CheckSum, 0, CheckSum.Length);
            buffer.Write(BitConverter.GetBytes(this.FileSize), 0, 8);
            buffer.Write(BitConverter.GetBytes(this.FileDownloaded), 0, 8);
            buffer.WriteByte((byte)(SupportResume ? 0x01 : 0x00));
            using (var file = new FileStream(tempfilepath, FileMode.CreateNew))
            {
                file.Write(buffer.ToArray(), 0, (int)buffer.Length);
            }
        }

        public void UpdateDownload(int length)
        {
            this.FileDownloaded += length;
        }
    }
}
