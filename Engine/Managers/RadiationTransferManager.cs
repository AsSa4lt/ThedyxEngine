﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Util;


namespace ThedyxEngine.Engine.Managers{
    /**
     * \class RadiationTransferManager
     * \brief Manages the transfer of radiation heat between objects in the simulation.
     *
     * The RadiationTransferManager class provides methods for calculating and transferring
     * radiation heat between objects in the simulation. It includes methods for transferring
     * radiation heat between two objects and radiation heat loss to air.
     */
    public static class RadiationTransferManager {

        /**
         * Transfer radiation heat between two objects
         * Q = σ * A * e' * (T1^4 - T2^4) * t.
         * \param square1 First square.
         * \param square2 Second square.
         */
        private static void TransferRadiationBetweenTwoSquares(GrainSquare square1, GrainSquare square2) {
            // Calculate emissivity between two objects
            var emmisivityBetweenTwoObjects = 
                (square1.Material.Emmisivity * square2.Material.Emmisivity) / 
                (1 / square1.Material.Emmisivity + 1 / square2.Material.Emmisivity - 1);

            // Calculate the view factor
            var distance = Math.Sqrt(Math.Pow(square2.Position.X - square1.Position.X, 2) +
                                     Math.Pow(square2.Position.Y - square1.Position.Y, 2));
    
            var maxDistance = Math.Sqrt(2) * Const.RadiationDepth; // Max distance within depth range
            var viewFactor = Math.Clamp(1.0 / distance, 0.0, 1.0); // Simplified view factor based on inverse distance
            if (distance > maxDistance) return; // If beyond range, no radiation

            // Stefan-Boltzmann radiation heat transfer
            var energyRadiationLoss = emmisivityBetweenTwoObjects * Const.StefanBoltzmannConst *
                                      Const.GridStep * viewFactor *
                                      (Math.Pow(square1.CurrentTemperature, 4) - Math.Pow(square2.CurrentTemperature, 4)) *
                                      Const.EngineIntervalUpdate / 1000;

            // Apply the energy changes to both squares
            square1.AddEnergyDelta(-energyRadiationLoss / 2);
            // removed apply heat to the second square, because this will be called for the second square
            //square2.AddEnergyDelta(energyRadiationLoss / 2);
        }

        /**
         * Transfer radiation heat loss to air
         * Q = σ*A*e`*(T1^4 - T2^4)*t.
         * \param obj object
         */
        private static void TransferRadiationHeatLooseToAir(EngineObject obj) {
            var squares = obj.GetSquares();
            foreach(var square in squares) {
                // calculated by Stefan-Boltzmann law of radiation and multiplied by the engine update interval
                // we need to find the area of the square that is not touching other squares to calculate the radiation loss to air
                var areaRadiationLoss = Math.Max((4 - square.GetAdjacentSquares().Count),0) * Const.GridStep;
                var energyRadiationLoss = square.Material.Emmisivity * Const.StefanBoltzmannConst * areaRadiationLoss * (Math.Pow(square.CurrentTemperature, 4) - Math.Pow(Engine.AirTemperature, 4)) * Const.EngineIntervalUpdate / 1000;
                square.AddEnergyDelta(-energyRadiationLoss);
            }
        }

        /**
         * 
         * Transfer radiation heat for all objects
         * First, transfer radiation heat loss to air
         * Second, transfer radiation heat between objects
         * Simplified logic for now, details are in the documentation
         * \param objects list of objects
         */
        public static void TransferRadiationHeat(List<EngineObject> objects){
            foreach(var obj in objects){
                TransferRadiationHeatLooseToAir(obj);
            }

            foreach (var obj in objects) {
                List<GrainSquare> objsquares = obj.GetSquares();
                foreach (var square in objsquares) {
                    HashSet<GrainSquare> radiationSquares = square.GetRadiationSquares();
                    foreach (var radSquare in radiationSquares) {
                        TransferRadiationBetweenTwoSquares(square, radSquare);
                    }
                }
            }
        }

    }
}
