using Microsoft.Maui.Controls.Shapes;
using ThedyxEngine.Engine.Managers;
using ThedyxEngine.UI;
using ThedyxEngine.Util;

namespace ThedyxEngine.Engine;

public class EngineGrainLiquid : GrainSquare {
    public enum CurrentSta {
        Liquid,
        Solid,
        Gas
    }
    
    private double AccumulatedEnergy { get; set; }
    
    public CurrentSta CurrentState { get; set; }
    
    public EngineGrainLiquid(string name, Point position, Material material) : base(name, position) {
        Material = material;
        SetStateFromTemperature();
        SetCachedPoints();
    }
    
    public void SetStateFromTemperature() {
        if(_currentTemperature < Material.MeltingTemperature) {
            CurrentState = CurrentSta.Solid;
        } else if(_currentTemperature > Material.BoilingTemperature) {
            CurrentState = CurrentSta.Gas;
        } else {
            CurrentState = CurrentSta.Liquid;
        }
    }
    
    /**
     * Generates the polygons that visually represent the square.
     * This method overrides the abstract method defined in \ref EngineObject.
     * \return List of polygons constituting the square's visual representation.
     */
    public override void GetPolygons(CanvasManager canvasManager, out List<Polygon> polygons, out List<double> temperatures)
    {
        polygons = [];
        temperatures = [];
        Polygon polygon = new();
        polygon.Points.Add(_position);
        polygon.Points.Add(_cachedPointB);
        polygon.Points.Add(_cachedPointD);
        polygon.Points.Add(_cachedPointC);

        if (!IsSelected)
            polygon.Fill = ColorManager.GetColorFromTemperature(_currentTemperature);
        else
        {
            polygon.Stroke = SolidColorBrush.Black;
            polygon.StrokeThickness = 3;
            polygon.Fill = ColorManager.GetColorFromTemperature(_currentTemperature);
            if (Engine.Mode != Engine.EngineMode.Running)
                polygon.Opacity = 0.5;
        }
        
        if(CurrentState == CurrentSta.Solid) {
            polygon.Opacity = 1;
        }else if(CurrentState == CurrentSta.Liquid) {
            polygon.Opacity = 0.5;
        }else {
            polygon.Opacity = 0.1;
        }

        polygons.Add(polygon);
        temperatures.Add(_currentTemperature);
    }

    public new void ApplyEnergyDelta() {
        lock (EnergyLock) {
            double tempDelta = EnergyDelta / Const.GridStep / Const.GridStep /
                               _material.SolidSpecificHeatCapacity / _material.SolidDensity;
            // we need to check if the temperature we are going to set is in the right range
            // 1) current State is solid
            if (CurrentState == CurrentSta.Solid) {
                // check if we are going to melt the object
                if (_currentTemperature + tempDelta >= _material.MeltingTemperature) {
                    _currentTemperature = _material.MeltingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.MeltingTemperature - _currentTemperature) * Const.GridStep * Const.GridStep * _material.SolidSpecificHeatCapacity * _material.SolidDensity;
                    // check if we have enough energy to melt the object
                    double energyToMelt = _material.MeltingEnergy * Const.GridStep * Const.GridStep * _material.SolidDensity;
                    if (AccumulatedEnergy >= energyToMelt) {
                        AccumulatedEnergy -= energyToMelt;
                        CurrentState = CurrentSta.Liquid;
                        // transfer the rest of the energy to the liquid
                        _currentTemperature += AccumulatedEnergy / Const.GridStep / Const.GridStep / _material.LiquidSpecificHeatCapacity / _material.LiquidDensity;
                        AccumulatedEnergy = 0;
                    }
                }else {
                    _currentTemperature += tempDelta;
                }
            }else if (CurrentState == CurrentSta.Liquid) {
                // check if we are going to boil the object
                if (_currentTemperature + tempDelta >= _material.BoilingTemperature) {
                    _currentTemperature = _material.BoilingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.BoilingTemperature - _currentTemperature) * Const.GridStep * Const.GridStep * _material.LiquidSpecificHeatCapacity * _material.LiquidDensity;
                    // check if we have enough energy to boil the object
                    double energyToBoil = _material.BoilingEnergy * Const.GridStep * Const.GridStep * _material.LiquidDensity;
                    if (AccumulatedEnergy >= energyToBoil) {
                        AccumulatedEnergy -= energyToBoil;
                        CurrentState = CurrentSta.Gas;
                    }
                }
                // check if we are going to freeze the object
                else if (_currentTemperature + tempDelta <= _material.MeltingTemperature) {
                    _currentTemperature = _material.MeltingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.MeltingTemperature - _currentTemperature) * Const.GridStep * Const.GridStep * _material.LiquidSpecificHeatCapacity * _material.LiquidDensity;
                    // check if we have enough energy to freeze the object
                    double energyToFreeze = _material.MeltingEnergy * Const.GridStep * Const.GridStep * _material.LiquidDensity;
                    if (AccumulatedEnergy >= energyToFreeze) {
                        AccumulatedEnergy -= energyToFreeze;
                        CurrentState = CurrentSta.Solid;
                    }
                }
                else {
                    // just add temperature
                    _currentTemperature += tempDelta;
                }
            }else if (CurrentState == CurrentSta.Gas) {
                // check if we are going to condense the object
                if (_currentTemperature + tempDelta <= _material.BoilingTemperature) {
                    _currentTemperature = _material.BoilingTemperature;
                    // we add the rest of the energy to the accumulated energy
                    AccumulatedEnergy += EnergyDelta - (_material.BoilingTemperature - _currentTemperature) * Const.GridStep * Const.GridStep * _material.GasSpecificHeatCapacity * _material.GasDensity;
                    // check if we have enough energy to condense the object
                    double energyToCondense = _material.BoilingEnergy * Const.GridStep * Const.GridStep * _material.GasDensity;
                    if (AccumulatedEnergy >= energyToCondense) {
                        AccumulatedEnergy -= energyToCondense;
                        CurrentState = CurrentSta.Liquid;
                    }
                }else {
                    _currentTemperature += tempDelta;
                }
            }
            CurrentTemperature = Math.Max(0, CurrentTemperature);
            EnergyDelta = 0;
        }
    }
}