using SD.Core.Strand.Models;
using System.Text;

namespace SD.Core.Strand.Helpers;
public static class St7APIHelper
{
    public static void ThrowIfFails(this int errorCode, bool releaseApiOnError = false)
    {
        var response = errorCode.HandleApiError(releaseApiOnError);
        if (!response.IsValid) 
            throw new Exception(response.ErrorMessage);
    }
    public static Strand7ApiResponse HandleApiError(this int errorCode, bool releaseApiOnError = false)
    {
        if (errorCode != St7.ERR7_NoError)
        {
            ReleaseApiOnError(releaseApiOnError);

            var sbErrorString = new StringBuilder(St7.kMaxStrLen);
            if (St7.ERR7_NoError == St7.St7GetAPIErrorString(errorCode, sbErrorString, sbErrorString.Capacity) ||
                St7.ERR7_NoError == St7.St7GetSolverErrorString(errorCode, sbErrorString, sbErrorString.Capacity))
            {
                return new Strand7ApiResponse() { ErrorMessage = sbErrorString.ToString(), ErrorCode = errorCode };
            }
            return new Strand7ApiResponse() { ErrorMessage = "An unknown error occured when processing a Strand7 API call." };
        }
        else
        {
            return new Strand7ApiResponse() { IsValid = true };
        }
    }
    private static void ReleaseApiOnError(bool releaseApiOnError)
    {
        if (releaseApiOnError)
            _ = St7.St7Release();
    }
}
