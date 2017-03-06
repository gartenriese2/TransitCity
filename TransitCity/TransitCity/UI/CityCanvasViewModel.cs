namespace TransitCity.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using City;
    using City.Transit;

    using Models;
    using Models.Transit;
    using Models.Transit.Metro;

    using MVVM;

    using Pathfinding;

    using Utility.Coordinates;
    using Utility.Units;

    public class CityCanvasViewModel : PropertyChangedBase
    {
        private readonly Random _rnd = new Random();

        private MetroLine _newMetroLine;

        private uint _residentsNearStations;
        private uint _residentsWithPossibleTransitUse;
        private uint _residentsWithTransitUse;

        private ICommand _showRidershipCommand;

        public CityCanvasViewModel()
        {
            // design mode
        }

        public CityCanvasViewModel(CityModel cityModel, UiModel uiModel)
        {
            UiModel = uiModel;
            UiModel.MetroBuildFinished += UiModelOnMetroBuildFinished;

            CityModel = cityModel;
        }

        public UiModel UiModel { get; }

        public CityModel CityModel { get; }

        public NetworkViewModel NetworkViewModel { get; } = new NetworkViewModel();

        public ObservableCollection<ResidentViewModel> Residents { get; } = new ObservableCollection<ResidentViewModel>();

        public ObservableCollection<JobViewModel> Jobs { get; } = new ObservableCollection<JobViewModel>();

        public ObservableCollection<ConnectionViewModel> Connections { get; } = new ObservableCollection<ConnectionViewModel>();

        public ObservableCollection<StationViewModel> MetroStations { get; } = new ObservableCollection<StationViewModel>();

        public ObservableCollection<MetroConnectionViewModel> MetroConnections { get; } = new ObservableCollection<MetroConnectionViewModel>();

        public ObservableCollection<StationViewModel> NewMetroLineStations { get; } = new ObservableCollection<StationViewModel>();

        public ObservableCollection<MetroConnectionViewModel> NewMetroLineConnections { get; } = new ObservableCollection<MetroConnectionViewModel>();

        public ObservableCollection<MetroLine> MetroLines { get; } = new ObservableCollection<MetroLine>();

        public uint ResidentsNearStations
        {
            get { return _residentsNearStations; }
            set
            {
                if (value != _residentsNearStations)
                {
                    _residentsNearStations = value;
                    OnPropertyChanged();
                }
            }
        }

        public uint ResidentsWithPossibleTransitUse
        {
            get { return _residentsWithPossibleTransitUse; }
            set
            {
                if (value != _residentsWithPossibleTransitUse)
                {
                    _residentsWithPossibleTransitUse = value;
                    OnPropertyChanged();
                }
            }
        }

        public uint ResidentsWithTransitUse
        {
            get { return _residentsWithTransitUse; }
            set
            {
                if (value != _residentsWithTransitUse)
                {
                    _residentsWithTransitUse = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ShowRidershipCommand
            => _showRidershipCommand ?? (_showRidershipCommand = new RelayCommand(isChecked => ShowRidership((bool)isChecked)));

        public double ActualHeight { get; set; }

        public void Initialize()
        {
            foreach (var resident in CityModel.ResidentialBuildings)
            {
                Residents.Add(new ResidentViewModel(resident, resident.Position.ToViewPosition()));
            }

            foreach (var job in CityModel.Jobs)
            {
                Jobs.Add(new JobViewModel(job, job.Position.ToViewPosition()));
            }

            foreach (var connection in CityModel.Connections)
            {
                var residentViewModel = Residents.First(r => r.Model == connection.ResidentialBuilding);
                var jobViewModel = Jobs.First(j => j.Model == connection.Job);
                var c = new ConnectionViewModel(residentViewModel, jobViewModel);
                Connections.Add(c);
                residentViewModel.ConnectionViewModel = c;
                jobViewModel.ConnectionViewModels.Add(c);
            }
        }

        public void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (UiModel.BuildMetroChecked)
            {
                e.Handled = true;
                var mousePos = e.GetPosition((CityCanvas)sender);
                AddStationToNewMetroLine(MouseToView(mousePos));
            }
        }

        private void ShowRidership(bool isChecked)
        {
            foreach (var metroConnection in MetroConnections)
            {
                if (isChecked)
                {
                    metroConnection.SetWidthRelative(1.0);
                }
                else
                {
                    metroConnection.ResetWidth();
                }
            }
        }

        private ViewPosition MouseToView(Point pos) => new ViewPosition(pos.X, ActualHeight - pos.Y);

        private void UiModelOnMetroBuildFinished(object sender, EventArgs eventArgs)
        {
            if (!NewMetroLineIsValid())
            {
                return;
            }

            foreach (var metroStationModel in _newMetroLine.GetStations())
            {
                var vm = GetViewModelFromModel(metroStationModel);
                //vm.ReachVisibility = Visibility.Collapsed;
            }

            foreach (var metroStationViewModel in NewMetroLineStations)
            {
                MetroStations.Add(metroStationViewModel);
            }

            foreach (var metroConnectionViewModel in NewMetroLineConnections)
            {
                MetroConnections.Add(metroConnectionViewModel);
            }

            NewMetroLineStations.Clear();
            NewMetroLineConnections.Clear();

            foreach (var metroStationModel in _newMetroLine.GetStations())
            {
                metroStationModel.Lines.Add(_newMetroLine);
            }

            MetroLines.Add(_newMetroLine);
            _newMetroLine = null;

            FindPathForResidents();
        }

        private bool NewMetroLineIsValid()
        {
            if (_newMetroLine == null || _newMetroLine.GetStations().Count <= 1)
            {
                NewMetroLineStations.Clear();
                NewMetroLineConnections.Clear();
                _newMetroLine = null;
                return false;
            }

            return true;
        }

        private void FindPathForResidents()
        {
            ResidentsNearStations = 0;
            ResidentsWithPossibleTransitUse = 0;
            ResidentsWithTransitUse = 0;

            foreach (var metroStation in MetroStations)
            {
                metroStation.Model.PeopleEntering = 0;
                metroStation.Model.PeopleExiting = 0;
                metroStation.Model.PeopleTransfering = 0;
            }

            foreach (var line in MetroLines)
            {
                line.Ridership = 0;
            }

            foreach (var residentViewModel in Residents)
            {
                foreach (var resident in residentViewModel.Model.Residents)
                {
                    var job = resident.Connection.Job;
                    var stationsNearResident = new List<Station>();
                    var stationsNearJob = new List<Station>();
                    GetStationsNearResidentAndJob(residentViewModel, stationsNearResident, job, stationsNearJob);

                    if (stationsNearResident.Count > 0)
                    {
                        ++ResidentsNearStations;
                    }

                    if (stationsNearResident.Count == 0 || stationsNearJob.Count == 0)
                    {
                        continue;
                    }

                    ++ResidentsWithPossibleTransitUse;

                    var timeWithCar = CalculateTimeWithCar(residentViewModel, job);

                    var stationNodeDictionary = new Dictionary<Station, Node>();

                    var tuple = FindQuickestPath(residentViewModel, job, stationNodeDictionary);
                    if (tuple != null && tuple.Item2 < timeWithCar)
                    {
                        ++ResidentsWithTransitUse;

                        var usedLines = CreateUsedLinesList(tuple, stationNodeDictionary);

                        foreach (var line in usedLines)
                        {
                            var lineModel = MetroLines.First(l => l.Number == line);
                            ++lineModel.Ridership;
                        }
                    }
                }
            }
        }

        private Tuple<Path, Time> FindQuickestPath(ResidentViewModel residentViewModel, JobModel job, Dictionary<Station, Node> stationNodeDictionary)
        {
            var residentNodeInfo = new NodeInfo { Public = true, AllowedTypes = new List<NodeInfo.AllowedType> { NodeInfo.AllowedType.Pedstrian } };
            var jobNodeInfo = new NodeInfo { Public = true, AllowedTypes = new List<NodeInfo.AllowedType> { NodeInfo.AllowedType.Pedstrian } };
            var residentNode = new Node(residentViewModel.Model.Position.ToWorldPosition(), residentNodeInfo);
            var jobNode = new Node(job.Position.ToWorldPosition(), jobNodeInfo);
            ConnectStationsWithResidentAndJob(stationNodeDictionary, residentNode, jobNode);
            ConnectStationNodes(stationNodeDictionary);

            var tuple = PathFinder.FindQuickestPath(residentNode, jobNode, Traveller.Create());
            return tuple;
        }

        private List<List<int>> CreateLineList(Tuple<Path, Time> tuple, Dictionary<Station, Node> stationNodeDictionary)
        {
            var lineList = new List<List<int>>();
            for (var i = 1; i < tuple.Item1.Nodes.Count - 1; ++i)
            {
                var node = tuple.Item1.Nodes[i];
                var station = stationNodeDictionary.Where(p => p.Value == node).ToList().First().Key;
                if (i == 1)
                {
                    ++station.PeopleEntering;
                }
                else if (i == tuple.Item1.Nodes.Count - 2)
                {
                    ++station.PeopleExiting;
                }

                lineList.Add(new List<int>());
                foreach (var line in station.Lines)
                {
                    lineList.Last().Add((int) line.Number);
                }
            }
            return lineList;
        }

        private IEnumerable<int> CreateUsedLinesList(Tuple<Path, Time> tuple, Dictionary<Station, Node> stationNodeDictionary)
        {
            var lineList = CreateLineList(tuple, stationNodeDictionary);
            var lineOnPathList = CreateLineOnPathList(lineList);

            var usedLines = new List<int>();
            var idx = 0;
            for (var i = 1; i < lineOnPathList.Count; ++i)
            {
                var common = lineOnPathList[idx].Intersect(lineOnPathList[i]).ToList();
                if (common.Count == 0)
                {
                    // must have transfered at station before!
                    var node = tuple.Item1.Nodes[i + 1];
                    var station = stationNodeDictionary.Where(p => p.Value == node).ToList().First().Key;
                    ++station.PeopleTransfering;

                    var commonBefore = lineOnPathList[idx].Intersect(lineOnPathList[i - 1]).ToList();
                    var rndIdx = _rnd.Next(commonBefore.Count);
                    usedLines.Add(commonBefore[rndIdx]);

                    idx = i;
                }
                else if (i == lineOnPathList.Count - 1)
                {
                    var rndIdx = _rnd.Next(common.Count);
                    usedLines.Add(common[rndIdx]);
                }
            }

            return usedLines;
        }

        private List<List<int>> CreateLineOnPathList(IReadOnlyList<List<int>> lineList)
        {
            var lineOnPathList = new List<List<int>>();
            for (var i = 1; i < lineList.Count; ++i)
            {
                var linesBefore = lineList[i - 1];
                var linesNow = lineList[i];
                var commonList = linesBefore.Intersect(linesNow).ToList();
                if (commonList.Count == 0)
                {
                    throw new InvalidOperationException("commonlist shouldn't be empty!");
                }

                lineOnPathList.Add(commonList);
            }

            return lineOnPathList;
        }

        private void ConnectStationsWithResidentAndJob(IDictionary<Station, Node> stationNodeDictionary, Node residentNode, Node jobNode)
        {
            foreach (var metroStationViewModel in MetroStations)
            {
                var stationModel = metroStationViewModel.Model;
                var stationNodeInfo = new NodeInfo { Public = true, AllowedTypes = new List<NodeInfo.AllowedType> { NodeInfo.AllowedType.Pedstrian, NodeInfo.AllowedType.Metro } };
                var stationNode = new Node(stationModel.Position.ToWorldPosition(), stationNodeInfo);
                stationNodeDictionary[stationModel] = stationNode;
                Node.Connect(residentNode, stationNode, new PathInfo { Type = NodeInfo.AllowedType.Pedstrian, Speed = new Speed { Kmh = 5 } });
                Node.Connect(stationNode, jobNode, new PathInfo { Type = NodeInfo.AllowedType.Pedstrian, Speed = new Speed { Kmh = 5 } });
            }
        }

        private Time CalculateTimeWithCar(ResidentViewModel residentViewModel, JobModel job)
        {
            var distance = new Distance((float)residentViewModel.Model.Position.ToWorldPosition().GetDistanceTo(job.Position.ToWorldPosition()));
            var timeWithCar = distance / new Speed(11); // 11m/s ~ 40km/h
            timeWithCar += Time.FromSeconds(120); // parking
            return timeWithCar;
        }

        private void GetStationsNearResidentAndJob(ResidentViewModel residentViewModel, ICollection<Station> stationsNearResident, JobModel job, ICollection<Station> stationsNearJob)
        {
            foreach (var stationViewModel in MetroStations)
            {
                var station = stationViewModel.Model;
                if (station.IsWithinWalkingDistance(residentViewModel.Model.Position))
                {
                    stationsNearResident.Add(station);
                }

                if (station.IsWithinWalkingDistance(job.Position))
                {
                    stationsNearJob.Add(station);
                }
            }
        }

        private void ConnectStationNodes(IReadOnlyDictionary<Station, Node> stationNodeDictionary)
        {
            foreach (var metroLineModel in MetroLines)
            {
                for (var i = 1; i < metroLineModel.NumStations; ++i)
                {
                    var stationA = metroLineModel.GetStations()[i - 1];
                    var stationB = metroLineModel.GetStations()[i];
                    var stationANode = stationNodeDictionary[stationA];
                    var stationBNode = stationNodeDictionary[stationB];
                    Node.ConnectBidirectional(stationANode, stationBNode, new PathInfo { Type = NodeInfo.AllowedType.Metro, Speed = new Speed { Kmh = 80 } });
                }
            }
        }

        private void AddStationToNewMetroLine(ViewPosition viewPos)
        {
            // is first station?
            if (_newMetroLine == null)
            {
                _newMetroLine = new MetroLine((uint)(MetroLines.Count + 1), "Test");

                // check if new metro station already exists
                var alreadyExists = false;
                foreach (var station in MetroStations)
                {
                    if (station.ViewPosition.GetDistanceTo(viewPos) < 5.0)
                    {
                        _newMetroLine.GetStations().Add(station.Model);
                        //station.ReachVisibility = Visibility.Visible;
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    var metroStationViewModel = new StationViewModel(viewPos, 8, new Station(viewPos.ToModelPosition()));
                    NewMetroLineStations.Add(metroStationViewModel);
                    _newMetroLine.GetStations().Add(metroStationViewModel.Model); // TODO does not work that way
                }
            }
            else
            {
                // check if clicked on old station of new line
                var isOldStation = false;
                for (var i = 0; i < _newMetroLine.GetStations().Count; ++i)
                {
                    var station = _newMetroLine.GetStations()[i];
                    if (station.Position.ToViewPosition().GetDistanceTo(viewPos) < 5.0)
                    {
                        isOldStation = true;
                        _newMetroLine.GetStations().RemoveAt(i);
                        var vm = GetViewModelFromModel(station);
                        if (vm != null)
                        {
                            vm.ReachVisibility = Visibility.Collapsed;
                        }

                        if (_newMetroLine.GetStations().Count == 0)
                        {
                            _newMetroLine = null;
                        }

                        break;
                    }
                }

                // check if clicked on existing station
                if (!isOldStation)
                {
                    foreach (var station in MetroStations)
                    {
                        if (station.ViewPosition.GetDistanceTo(viewPos) < 5.0)
                        {
                            isOldStation = true;
                            _newMetroLine.GetStations().Add(station.Model);
                            station.ReachVisibility = Visibility.Visible;
                            break;
                        }
                    }
                }

                if (!isOldStation)
                {
                    var station = new MetroStationViewModel(viewPos, new MetroStationModel(viewPos.ToModelPosition()));
                    _newMetroLine.GetStations().Add(station.Model);
                }

                RewriteNewMetroCollections();
            }
        }

        private void RewriteNewMetroCollections()
        {
            if (_newMetroLine == null)
            {
                NewMetroLineStations.Clear();
                NewMetroLineConnections.Clear();
                return;
            }

            var stationList = new List<MetroStationViewModel>();
            var connectionList = new List<MetroConnectionViewModel>();
            for (var i = 0; i < _newMetroLine.Stations.Count; ++i)
            {
                var s1 = _newMetroLine.Stations[i];
                var list = MetroStations.Where(vm => vm.Model == s1).ToList();
                if (list.Count == 0)
                {
                    stationList.Add(new MetroStationViewModel(s1.Position.ToViewPosition(), s1));
                }

                if (i > 0)
                {
                    var s0 = _newMetroLine.Stations[i - 1];
                    var s0Vm = GetViewModelFromModel(s0) ?? new MetroStationViewModel(s0.Position.ToViewPosition(), s0);
                    var s1Vm = GetViewModelFromModel(s1) ?? new MetroStationViewModel(s1.Position.ToViewPosition(), s1);
                    var cList = MetroConnections.Where(vm => (vm.Model.StationA == s0Vm.Model && vm.Model.StationB == s1Vm.Model) || vm.Model.StationB == s0Vm.Model && vm.Model.StationA == s1Vm.Model).ToList();
                    if (cList.Count == 0)
                    {
                        var connectionModel = new MetroConnectionModel(s0, s1, new List<MetroLineModel>());
                        connectionList.Add(new MetroConnectionViewModel(connectionModel));
                    }
                }
            }

            NewMetroLineStations.Clear();
            NewMetroLineConnections.Clear();
            foreach (var stationViewModel in stationList)
            {
                NewMetroLineStations.Add(stationViewModel);
            }

            foreach (var connectionViewModel in connectionList)
            {
                NewMetroLineConnections.Add(connectionViewModel);
            }
        }

        private StationViewModel GetViewModelFromModel(Station model)
        {
            if (model == null)
            {
                throw new ArgumentNullException();
            }

            var list = MetroStations.Where(vm => vm.Model == model).ToList();
            if (list.Count != 0)
            {
                return list.First();
            }

            list = NewMetroLineStations.Where(vm => vm.Model == model).ToList();
            return list.Count != 0 ? list.First() : null;
        }
    }
}
