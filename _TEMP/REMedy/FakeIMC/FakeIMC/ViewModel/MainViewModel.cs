using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using Reactive.Bindings;
using System.Reactive.Linq;
using Microsoft.Win32;

namespace FakeIMC.UI
{
    public class MainViewModel
    {
        private readonly IImcModel model;

        #region Public properties used by VIEW

        /// <summary>
        /// System log
        /// </summary>
        public ReactiveCollection<LogItem> Log { get; set; }

        public ReactiveCommand RegisterNoah { get; set; }

        public ReactiveCommand UnRegisterNoah { get; set; }

        public ReactiveCommand RegisterLocal { get; set; }

        public ReactiveCommand UnRegisterLocal { get; set; }

        public ReactiveCommand ConnectToNoah { get; set; }

        public ReactiveCommand ClearLog { get; set; }
        
        public ReactiveProperty<bool> HeartBeat { get; set; }

        public ReactiveProperty<bool> RandomCurves { get; set; }

        public ReactiveProperty<bool> AddReugToReag { get; set; }

        public ReactiveCollection<ReactiveCommand> PossibleErrors { get; set; }

        public GridViewModel Grid { get; }

        public SkinViewModel Skin { get; }

        public ReactiveCommand ExportLog { get; set; }

        #endregion Public properties used by VIEW

        public MainViewModel(IImcModel model, GridViewModel grid, SkinViewModel skinViewModel)
        {
            this.model = model;
            this.model.Start();
            Grid = grid;
            Skin = skinViewModel;

            Log = new ReactiveCollection<LogItem>();

            this.model.DataChanged
                .Buffer(TimeSpan.FromMilliseconds(500))
                .ObserveOn(UIDispatcherScheduler.Default)
                .Subscribe(Observer.Create<IList<LogItem>>(c =>
                {
                    Log.AddRangeOnScheduler(c);
                }));
            
            RegisterNoah = new ReactiveCommand();
            RegisterNoah.Subscribe(() => { this.model.RegisterNoah(); });

            UnRegisterNoah = new ReactiveCommand();
            UnRegisterNoah.Subscribe(() => { this.model.UnRegisterNoah(); });

            RegisterLocal = new ReactiveCommand();
            RegisterLocal.Subscribe(() => { this.model.RegisterLocal(); });

            UnRegisterLocal = new ReactiveCommand();
            UnRegisterLocal.Subscribe(() => { this.model.UnRegisterLocal(); });

            ConnectToNoah = new ReactiveCommand();
            ConnectToNoah.Subscribe(() => { this.model.ConnectToNoah(); });

            ClearLog = new ReactiveCommand();
            ClearLog.Subscribe(() => Log.Clear());

            HeartBeat = new ReactiveProperty<bool>(true);
            HeartBeat.Subscribe(c => this.model.SetHeartBeat(c));

            RandomCurves = new ReactiveProperty<bool>(true);
            AddReugToReag = new ReactiveProperty<bool>(false);

            RandomCurves.Subscribe(c => this.model.UpdateCurvesConfiguration(new CurvesConfiguration
            {
                AddRandomValues = c,
                AddReugToReag = AddReugToReag.Value
            }));
            AddReugToReag.Subscribe(c => this.model.UpdateCurvesConfiguration(new CurvesConfiguration
            {
                AddRandomValues = RandomCurves.Value,
                AddReugToReag = c
            }));

            PossibleErrors = new ReactiveCollection<ReactiveCommand>();
            foreach (var possibleError in this.model.PossibleErrors)
            {
                var cmd = new UiCommand {Label = possibleError};
                cmd.Subscribe(() => this.model.SendError(possibleError));
                PossibleErrors.Add(cmd);
            }

            ExportLog = new ReactiveCommand();
            ExportLog.Subscribe(() =>
            {
                var sfd = new SaveFileDialog
                {
                    AddExtension = true,
                    FileName = "FakeImcLog",
                    Filter = "Text file (*.txt)|*.txt"
                };
                sfd.ShowDialog();

                if (!string.IsNullOrEmpty(sfd.FileName))
                {
                    File.WriteAllLines(sfd.FileName, Log.Select(c => c.Text));
                }
            });
        }
    }
}
