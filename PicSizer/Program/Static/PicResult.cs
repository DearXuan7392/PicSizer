#region

using System;

#endregion

namespace PicSizer.Program.Static
{
    public class PicResult
    {
        public readonly bool Ok;
        public readonly CompressResultEnum CompressResult;
        public readonly string Message;

        public enum CompressResultEnum
        {
            Ok = 0,
            OutOfLimit = 1,
            Error = 2,
        }

        private PicResult(bool ok, CompressResultEnum compressResult, string message = null)
        {
            Ok = ok;
            CompressResult = compressResult;
            Message = message;
        }

        private static readonly PicResult ResultOk = new PicResult(true, CompressResultEnum.Ok);

        public static PicResult GetOk() => ResultOk;

        public static PicResult GetOutOfLimit() =>
            new PicResult(false, CompressResultEnum.OutOfLimit, PicStrings.OutOfLimitSize);

        public static PicResult GetError(string message) => new PicResult(false, CompressResultEnum.Error, message);
        public static PicResult GetError(Exception e) => new PicResult(false, CompressResultEnum.Error, e.Message);
    }
}