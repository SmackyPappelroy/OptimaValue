using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimaValue;

public class PIDController
{
    public double Kp { get; private set; }
    public double Ki { get; private set; }
    public double Kd { get; private set; }

    // Konstruktor
    public PIDController(double kp, double ki, double kd)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
    }

    // Metod för att uppdatera PID-parametrar
    public void UpdateParameters(double kp, double ki, double kd)
    {
        Kp = kp;
        Ki = ki;
        Kd = kd;
    }
    public override string ToString()
    {
        return $"Kp: {Kp}, Ki: {Ki}, Kd: {Kd}";
    }
}

public class ProcessModel
{
    public double TimeConstant { get; set; }
    public double DeadTime { get; set; }
    public double Gain { get; set; }

    public ProcessModel(double timeConstant, double deadTime, double gain)
    {
        TimeConstant = timeConstant;
        DeadTime = deadTime;
        Gain = gain;
    }
}

public class TuningCalculator
{
    public static PIDController CalculateCohenCoon(ProcessModel model)
    {
        // Beräkna PID-parametrar baserade på Cohen-Coon-metoden
        double kp = (1.35 / model.Gain) * (1 + 0.185 * model.DeadTime / model.TimeConstant);
        double ki = kp / (0.54 * model.TimeConstant + 0.1 * model.DeadTime);
        double kd = kp * (0.5 * model.TimeConstant + 0.085 * model.DeadTime - 0.383 * model.TimeConstant * model.DeadTime / model.TimeConstant);

        return new PIDController(kp, ki, kd);
    }
    // Modifierad för att endast beräkna PI-parametrar
    public static PIDController CalculatePI(ProcessModel model)
    {
        // Beräkna PI-parametrar (utan D)
        double kp = (0.9 / model.Gain) * (1 + 0.083 * model.DeadTime / model.TimeConstant);
        double ki = kp / (3.33 * model.TimeConstant + 0.1 * model.DeadTime);

        // Ingen Kd-beräkning, returnerar en PIDController med Kd satt till 0 eller utelämnat helt
        return new PIDController(kp, ki, 0);
    }
}

public class ResponseTestParameter
{
    public int OutputVariableDB { get; set; }
    public int OutputVariableOffset { get; set; }
    public int ProcessVariableDB { get; set; }
    public int ProcessVariableOffset { get; set; }
    public float StepChange { get; set; }
    public double TestDurationSeconds { get; set; }
    public int SamplingRateMilliseconds { get; set; }
}

public class StepResponseTest
{
    private ExtendedPlc MyPlc;
    private ResponseTestParameter TestParameter;

    public StepResponseTest(ExtendedPlc plc, ResponseTestParameter testParameter)
    {
        MyPlc = plc;
        TestParameter = testParameter;
    }

    private bool ConnectToPlc()
    {
        return MyPlc.Plc.Connect();
    }

    private async Task<float> ReadProcessVariable()
    {
        var plcTag = new PlcTag(S7.Net.DataType.DataBlock, TestParameter.ProcessVariableDB, TestParameter.ProcessVariableOffset);
        plcTag.VarType = VarType.Real;
        plcTag.NrOfElements = 1;
        var readValue = await MyPlc.Plc.ReadAsync(plcTag);
        return readValue.ValueAsFloat;
    }

    private async Task WriteOutputVariable(float value)
    {
        var plcTag = new PlcTag(S7.Net.DataType.DataBlock, TestParameter.OutputVariableDB, TestParameter.OutputVariableOffset);
        plcTag.VarType = VarType.Real;
        plcTag.NrOfElements = 1;
        await MyPlc.Plc.WriteAsync(plcTag, value);
    }

    private async Task<float> ReadOutputVariable()
    {
        var plcTag = new PlcTag(S7.Net.DataType.DataBlock, TestParameter.OutputVariableDB, TestParameter.OutputVariableOffset);
        plcTag.NrOfElements = 1;
        var readValue = await MyPlc.Plc.ReadAsync(plcTag);
        return readValue.ValueAsFloat;
    }

    public async Task<PIDController> PerformStepResponseTest(bool onlyPI)
    {
        // Anslut till PLC om det inte redan är anslutet.
        if (!MyPlc.Plc.IsConnected)
        {
            if (!ConnectToPlc())
            {
                throw new InvalidOperationException("Kunde inte ansluta till PLC.");
            }
        }

        // Läs ursprungsvärdet för utgångsvariabeln.
        var originalOutputValue = await ReadOutputVariable();

        // Sätt utgångsvariabeln till stegstorleken.
        await WriteOutputVariable(TestParameter.StepChange);

        // Samla data för en viss tid.
        List<float> processVariableReadings = new List<float>();
        var testEndTime = DateTime.Now.AddSeconds(TestParameter.TestDurationSeconds);
        while (DateTime.Now < testEndTime)
        {
            var processVariableValue = await ReadProcessVariable();
            processVariableReadings.Add(processVariableValue);

            // Vänta lite innan nästa avläsning för att inte överbelasta nätverket/PLC:n.
            await Task.Delay(TestParameter.SamplingRateMilliseconds);
        }

        // Återställ utgångsvariabeln till ursprungligt värde (om nödvändigt).
        await WriteOutputVariable(originalOutputValue);

        // Här kan du analysera processVariableReadings för att bestämma tidskonstant, dödtid, etc.
        // Detta innebär att bearbeta data och extrahera relevant information för PID-tuning.
        return AnalyzeAndCalculatePIDParameters(processVariableReadings, onlyPI);
    }

    private PIDController AnalyzeAndCalculatePIDParameters(List<float> processVariableReadings, bool onlyPi)
    {
        // Antag att vi har en lista med processvariabelavläsningar
        // List<float> processVariableReadings = ...

        if (processVariableReadings.Count == 0)
            throw new InvalidOperationException("Inga processvariabelavläsningar att analysera.");

        float initialReading = processVariableReadings.First();
        float finalReading = processVariableReadings.Last();

        // Beräkna förstärkning (K)
        double gain = (finalReading - initialReading) / TestParameter.StepChange;

        // Identifiera dödtid (L)
        int deadTimeIndex = processVariableReadings.FindIndex(reading => reading > initialReading + 0.1 * (finalReading - initialReading));
        double deadTime = deadTimeIndex * TestParameter.SamplingRateMilliseconds / 1000.0;

        // Identifiera tidskonstant (T)
        float targetReading = (float)(initialReading + 0.632 * (finalReading - initialReading));
        int timeConstantIndex = processVariableReadings.FindIndex(deadTimeIndex, reading => reading >= targetReading);
        double timeConstant = (timeConstantIndex - deadTimeIndex) * TestParameter.SamplingRateMilliseconds / 1000.0;

        // Skapa processmodell och beräkna PID-parametrar
        ProcessModel model = new ProcessModel(timeConstant, deadTime, gain);
        if (onlyPi)
        {
            PIDController piController = TuningCalculator.CalculatePI(model);
            return piController;
        }

        PIDController pidController = TuningCalculator.CalculateCohenCoon(model);
        return pidController;
    }
}