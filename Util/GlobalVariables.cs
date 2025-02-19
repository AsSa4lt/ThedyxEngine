namespace ThedyxEngine.Util;

public static class GlobalVariables {
    /** Grid step */
    public static double GridStep = 0.01;
    /** Depth of looking for radiation squares */
    public static int RadiationDepth = 10; 
    /** Air temperature in Kelvin*/
    public static double AirTemperature = 293;
    /** Air thermal conductivity */
    public static double AirThermalConductivity = 0.025;
    /** Stefan-Boltzmann constant */
    public static readonly double StefanBoltzmannConst = 5.67 * Math.Pow(10, -8);
    /** Engine update interval */
    public static double  EngineIntervalUpdatePerSecond = 60; 
    /** Interval between UI updates */
    public static int WindowRefreshRate = 60;
    /** Program major version */
    public static readonly int MajorVersion = 0;
    /** Program minor version */
    public static readonly int MinorVersion = 1;
    /** Program patch version */
    public static bool SaveSimulationHumanReadable = false;
}