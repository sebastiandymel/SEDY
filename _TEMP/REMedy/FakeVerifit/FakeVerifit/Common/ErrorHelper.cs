using FakeVerifit.Data;
using Newtonsoft.Json.Linq;
using Remedy.Core;

namespace FakeVerifit
{
    /// <summary>
    /// Ported from AudioScan API Library
    /// </summary>
    public enum ErrorEnum
    {
        UnknownException,
        ApplicationException,
        Uncalibrated,
        Unequalized,
        SystemBusy,
        Cancelled,
        NoRefMic,
        Overdrive,
        MaxTMSPL,
        NoFittingFormula,
        NoThresholds,
        NoTargetsForStimulus,
        NoTargetsForLevel,
        NALNL2MissingThresholdAC,
        NALNL2MissingThresholdBC,
        ServerException,
        ConnectionUnavailable,
        IncompatibleVersion,
        InvalidCommand,
        InvalidParamters,
    }

    public static class ErrorHelper
    {
        public  static string ToErrorCode(this ErrorEnum errorEnum)
        {
            switch (errorEnum)
            {
                case ErrorEnum.ApplicationException:
                    return "4000";
                case ErrorEnum.Uncalibrated:
                    return "4001";
                case ErrorEnum.Unequalized:
                    return "4002";
                case ErrorEnum.SystemBusy:
                    return "4003";
                case ErrorEnum.Cancelled:
                    return "4100";
                case ErrorEnum.NoRefMic:
                    return "4101";
                case ErrorEnum.Overdrive:
                    return "4102";
                case ErrorEnum.MaxTMSPL:
                    return "4103";
                case ErrorEnum.NoFittingFormula:
                    return "4200";
                case ErrorEnum.NoThresholds:
                    return "4201";
                case ErrorEnum.NoTargetsForStimulus:
                    return "4202";
                case ErrorEnum.NoTargetsForLevel:
                    return "4203";
                case ErrorEnum.NALNL2MissingThresholdAC:
                    return "4300";
                case ErrorEnum.NALNL2MissingThresholdBC:
                    return "4301";
                case ErrorEnum.ServerException:
                    return "-32000";
                case ErrorEnum.ConnectionUnavailable:
                    return "-32001";
                case ErrorEnum.IncompatibleVersion:
                    return "-32002";
                case ErrorEnum.InvalidCommand:
                    return "-32601";
                case ErrorEnum.InvalidParamters:
                    return "-32602";
                default:
                    return "1000";
            }
        }

        public static JObject CreateErrorResponse(IDataItem selectedError)
        {
            var errorObject = JObject.Parse(EmbededResource.GetFileText("error.json"));
            errorObject["error"]["code"] = selectedError.Value;
            errorObject["error"]["message"] = $"Details for error named {selectedError.LocalizedName}";
            return errorObject;
        }

        public static JObject CreateErrorResponse(ErrorEnum errorEnum)
        {
            var errorObject = JObject.Parse(EmbededResource.GetFileText("error.json"));
            errorObject["error"]["code"] = errorEnum.ToErrorCode();
            errorObject["error"]["message"] = $"Details for error named {errorEnum.ToString()}";
            return errorObject;
        }
    }
}