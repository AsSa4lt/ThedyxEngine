﻿using System;

namespace ThedyxEngine.UI
{
    /**
     * \class CanvasManager
     * \brief Manages the canvas view, zoom, and movement.
     */
    public class CanvasManager
    {
        public int CurrentLeftXIndex { get; private set; }
        public int CurrentRightXIndex { get; private set; }
        public int CurrentTopYIndex { get; private set; }
        public int CurrentBottomYIndex { get; private set; }
        public double ViewportWidth { get; set; }
        public double ViewportHeight { get; set; }

        public CanvasManager()
        {
            ResetZoom();
        }

        public void ResetZoom()
        {
            CurrentLeftXIndex = -100;
            CurrentRightXIndex = 100;
            CurrentTopYIndex = 100;
            CurrentBottomYIndex = -100;
        }

        public void MoveHorizontal(int step)
        {
            CurrentLeftXIndex += step;
            CurrentRightXIndex += step;
        }

        public void MoveVertical(int step)
        {
            CurrentTopYIndex += step;
            CurrentBottomYIndex += step;
        }

        private int getStep()
        {
            int deltaX = CurrentRightXIndex - CurrentLeftXIndex;
            return deltaX > 1000 ? 100 : deltaX > 500 ? 50 : deltaX > 200 ? 20 : deltaX > 100 ? 10 : 5;
        }

        public void AdjustForAspectRatio(double width, double height)
        {
            ViewportWidth = width;
            ViewportHeight = height;
            double ratio = width / height;
            int yDistance = (int)((CurrentRightXIndex - CurrentLeftXIndex) / ratio);
            int currentYDistance = CurrentTopYIndex - CurrentBottomYIndex;
            int yDelta = (yDistance - currentYDistance) / 2;
            CurrentTopYIndex += yDelta;
            CurrentBottomYIndex -= yDelta;
        }
    }
}