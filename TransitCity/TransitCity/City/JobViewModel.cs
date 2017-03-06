namespace TransitCity.City
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using Models;

    using Utility;
    using Utility.Coordinates;

    public class JobViewModel : DrawableViewModel
    {
        private bool _activated;
        private double _size;

        public JobViewModel(JobModel model, ViewPosition pos)
            : base(pos, 0, 0)
        {
            Model = model;
            Size = JobsToSize(Model.NumJobs) + 5;
            Bottom = pos.Y - Size / 2;
            Left = pos.X - Size / 2;
        }

        public JobModel Model { get; }

        public List<ConnectionViewModel> ConnectionViewModels { get; } = new List<ConnectionViewModel>();

        public double Size
        {
            get { return _size; }
            set
            {
                if (Math.Abs(value - _size) > double.Epsilon)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Clicked()
        {
            _activated = !_activated;
            foreach (var connectionViewModel in ConnectionViewModels)
            {
                connectionViewModel.Brush = new SolidColorBrush(_activated ? Colors.Black : Colors.DarkGray);
            }
        }

        private double JobsToSize(uint jobs)
        {
            return Math.Sqrt(jobs / Math.PI);
        }
    }
}
