﻿#region License
// The MIT License (MIT)
// 
// Copyright (c) 2016 Alberto Rodríguez Orozco & LiveCharts contributors
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
// of the Software, and to permit persons to whom the Software is furnished to 
// do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

#region

using System;
using System.Drawing;
using System.Linq;
using LiveCharts.Core.DataSeries;
using LiveCharts.Core.Dimensions;
using LiveCharts.Core.Drawing;
using LiveCharts.Core.Interaction;
using LiveCharts.Core.Interaction.Points;
using LiveCharts.Core.Updating;

#endregion

namespace LiveCharts.Core.Charts
{
    /// <inheritdoc />
    public class PieChartModel : ChartModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PieChartModel"/> class.
        /// </summary>
        /// <param name="view">The chart view.</param>
        public PieChartModel(IChartView view) : base(view)
        {
            Charting.BuildFromTheme((IPieChartView) view);
        }

        /// <inheritdoc />
        protected override int DimensionsCount => 2;

        public override float ScaleToUi(double dataValue, Plane plane, float[] sizeVector = null)
        {
            throw new NotImplementedException();
        }

        public override double ScaleFromUi(float pixelsValue, Plane plane, float[] sizeVector = null)
        {
            throw new NotImplementedException();
        }

        protected override PointF GetTooltipLocation(IChartPoint[] points)
        {
            var p = points[0];

            var model = (PieViewModel)p.ViewModel;
            var angle = ((IPieChartView)View).StartingRotationAngle + model.To.Rotation + model.To.Wedge * .5;
            var radius = model.To.OuterRadius;

            var sx = model.ChartCenter.X + radius * Math.Sin(angle * Math.PI / 180);
            var sy = model.ChartCenter.Y + radius * Math.Cos(angle * Math.PI / 180);

            return new PointF((float) sx, (float) sy);
        }

        /// <inheritdoc />
        protected override void Update(bool restart, UpdateContext context)
        {
            OnUpdateStarted();

            base.Update(restart, context);

            if (DrawAreaSize[0] <= 0 || DrawAreaSize[1] <= 0)
            {
                // skip update if the chart is too small.
                // and lets delete its content...
                CollectResources(true);
                return;
            }

            View.Content.DrawArea = new RectangleF(
                new PointF(DrawAreaLocation[0], DrawAreaLocation[1]),
                new SizeF(DrawAreaSize[0], DrawAreaSize[1]));

            foreach (var series in Series.Where(x => x.IsVisible))
            {
                if (!(series is IPieSeries))
                {
                    throw new LiveChartsException(102, series.ThemeKey.Name, nameof(PieChartModel));
                }

                series.UpdateStarted(View);
                series.UpdateView(this, context);
                series.UpdateFinished(View);
            }

            CollectResources();
            OnUpdateFinished();
        }
    }
}
