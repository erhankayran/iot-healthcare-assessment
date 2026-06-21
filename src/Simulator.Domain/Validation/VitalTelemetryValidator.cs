using Simulator.Domain.Constants;
using Simulator.Domain.Models;

namespace Simulator.Domain.Validation;

public static class VitalTelemetryValidator
{
    public static bool TryValidate(VitalTelemetry telemetry, out string? errorMessage)
    {
        if (telemetry.HeartRate < VitalSignRanges.HeartRateMin ||
            telemetry.HeartRate > VitalSignRanges.HeartRateMax)
        {
            errorMessage =
                $"Heart rate must be between {VitalSignRanges.HeartRateMin} and {VitalSignRanges.HeartRateMax}.";
            return false;
        }

        if (telemetry.SpO2 < VitalSignRanges.SpO2Min || telemetry.SpO2 > VitalSignRanges.SpO2Max)
        {
            errorMessage =
                $"SpO2 must be between {VitalSignRanges.SpO2Min} and {VitalSignRanges.SpO2Max}.";
            return false;
        }

        errorMessage = null;
        return true;
    }
}
