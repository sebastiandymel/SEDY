using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Remedy.CommonUI;
using Remedy.Core;

namespace FakeVerifit.UI
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IAppConfigManager appConfigManager;

        readonly Random randomGenerator = new Random();

        private string measurementTime;
        private string equalizationTime;
        private string aidedSII;
        private string unaidedSII;
        private string verifitFirmwareVersion;

        public RelayCommand RandomizeCommand { get; }

        public SettingsViewModel(IAppConfigManager appConfigManager)
        {
            this.appConfigManager = appConfigManager;
            MeasurementTime = appConfigManager.GetMeasurementTime();
            EqualizationTime = "2000"; // ms;
            AidedSII = "54";
            UnaidedSII = "61";
            VerifitFirmwareVersion = appConfigManager.GetFirmwareVersion();
            InitializeGrids();

            RandomizeCommand = new RelayCommand(RandomizeData);
        }

        private void RandomizeData()
        {
            RandomizeCollection(TargetResult, 50, 70);
            RandomizeCollection(Percentile30, 30, 46);
            RandomizeCollection(Percentile99, 50, 75);
            RandomizeCollection(LTASS, 33, 47);
            RandomizeCollection(UCL, 110, 135);
            RandomizeCollection(SPLTresholds, 15, 50);
        }

        private void RandomizeCollection(SerializableFreqObservableCollection targetResult, int lowEnd, int highEnd)
        {
            foreach (var freqVal in targetResult)
            {
                freqVal.Value = this.randomGenerator.Next(lowEnd, highEnd).ToString();
            }
        }

        public IEnumerable<FreqVal> GenerateFreqValCollection(SerializableFreqObservableCollection basedOn)
        {
            List<FreqVal> newList = new List<FreqVal>();
            int lowEnd = (int)basedOn.Min(x => x.NumberValue);
            int highEnd = (int)basedOn.Max(x => x.NumberValue);

            foreach (var freqVal in basedOn)
            {
                newList.Add(new FreqVal
                {
                    Frequency = freqVal.Frequency,
                    Value = this.randomGenerator.Next(lowEnd, highEnd).ToString()
                });
            }
            return newList;
        }

        public void InitializeGrids()
        {
            TargetResult.Id = nameof(TargetResult);
            Percentile30.Id = nameof(Percentile30);
            Percentile99.Id = nameof(Percentile99);
            LTASS.Id = nameof(LTASS);
            UCL.Id = nameof(UCL);
            SPLTresholds.Id = nameof(SPLTresholds);
        }

        public ObservableCollection<Setting> Targets { get; } = new ObservableCollection<Setting>
        {
            new Setting {Value = "dsladult", LocalizedName = "DSL adult", IsChecked = true},
            new Setting {Value = "dslchild", LocalizedName = "DSL child", IsChecked = true},
            new Setting {Value = "nal-nl1", LocalizedName = "NAL-NL1", IsChecked = true},
            new Setting {Value = "nal-nl2", LocalizedName = "NAL-NL2", IsChecked = true},
            new Setting {Value = "none", LocalizedName = "None", IsChecked = true}
        };

        public ObservableCollection<Setting> Stimuli { get; } = new ObservableCollection<Setting>
        {
            new Setting {Value = "speech-wbcarrots", LocalizedName = "Speech-std(F)", IsChecked = true},
            new Setting {Value = "speech-wbear", LocalizedName = "Speech-std(M)", IsChecked = true},
            new Setting {Value = "speech-ists", LocalizedName = "Speech-ISTS", IsChecked = true},
            new Setting {Value = "speech-fr", LocalizedName = "Speech(fr)", IsChecked = true},
            new Setting {Value = "speech-de", LocalizedName = "Speech(de)", IsChecked = true},
            new Setting {Value = "speech-ptbr", LocalizedName = "Speech(pt-BR)", IsChecked = true},
            new Setting {Value = "speech-cmn", LocalizedName = "Speech(cmn)", IsChecked = true},
            new Setting {Value = "speech-yue", LocalizedName = "Speech(yue)", IsChecked = true},
            new Setting {Value = "speech-ko", LocalizedName = "Speech(ko)", IsChecked = true},
            new Setting {Value = "mpo", LocalizedName = "MPO", IsChecked = true}
        };

        public ObservableCollection<Setting> VerifitVersions { get; } = new ObservableCollection<Setting>
        {
            new Setting {Value = UIBridgeConstants.Verifit1, LocalizedName = "Verifit 1", GroupName="VerifitVersion"},
            new Setting {Value = UIBridgeConstants.Verifit2, LocalizedName = "Verifit 2", GroupName="VerifitVersion", IsChecked=true},
            new Setting {Value = UIBridgeConstants.FakeVerifit, LocalizedName = "Fake Verifit", GroupName="VerifitVersion"}
        };

        public SerializableFreqObservableCollection TargetResult { get; } = new SerializableFreqObservableCollection
        {
            new FreqVal {Frequency = 250, Value = "32.5"},
            new FreqVal {Frequency = 500, Value = "42"},
            new FreqVal {Frequency = 750, Value = "52"},
            new FreqVal {Frequency = 1000, Value = "39"},
            new FreqVal {Frequency = 1500, Value = "39"},
            new FreqVal {Frequency = 2000, Value = "47"},
            new FreqVal {Frequency = 3000, Value = "50"},
            new FreqVal {Frequency = 4000, Value = "60"},
            new FreqVal {Frequency = 6000, Value = "60"},
            new FreqVal {Frequency = 8000, Value = "70"},
            new FreqVal {Frequency = 12500, Value = "70.8"},
        };

        public readonly int[] PercentileFrequencyPoints =
        {
            200, 210, 225, 235, 250, 265, 280, 300, 315, 335, 355, 375, 400, 420, 450, 470, 500, 530, 560, 595, 630,
            670, 710, 750, 800, 840, 900,
            945, 1000, 1060, 1120, 1190, 1250, 1335, 1400, 1500, 1600, 1680, 1800, 1890, 2000, 2120, 2240, 2380, 2500,
            2670, 2800, 3000, 3150, 3365,
            3550, 3775, 4000, 4240, 4500, 4760, 5000, 5340, 5600, 6000, 6300, 6725, 7100, 7550, 8000, 8500, 9000, 9500,
            10000, 10600, 11200, 11800, 12500
        };

        public SerializableFreqObservableCollection Percentile30 { get; } = new SerializableFreqObservableCollection
        {
            new FreqVal {Frequency = 250,Value = "42.6"},
            new FreqVal {Frequency = 500,Value = "36.9"},
            new FreqVal {Frequency = 750,Value = "32.5"},
            new FreqVal {Frequency = 1000,Value = "31.5"},
            new FreqVal {Frequency = 1500,Value = "31.4"},
            new FreqVal {Frequency = 2000,Value = "31.4"},
            new FreqVal {Frequency = 3000,Value = "34.4"},
            new FreqVal {Frequency = 4000,Value = "36.4"},
            new FreqVal {Frequency = 6000,Value = "41.0"},
            new FreqVal {Frequency = 8000,Value = "43.8"},
            new FreqVal {Frequency = 12500,Value = "45.5"},

        };

        public SerializableFreqObservableCollection Percentile99 { get; } = new SerializableFreqObservableCollection
        {
            new FreqVal {Frequency = 250,Value = "62.6"},
            new FreqVal {Frequency = 500,Value = "56.9"},
            new FreqVal {Frequency = 750,Value = "52.5"},
            new FreqVal {Frequency = 1000,Value = "51.5"},
            new FreqVal {Frequency = 1500,Value = "53.4"},
            new FreqVal {Frequency = 2000,Value = "61.4"},
            new FreqVal {Frequency = 3000,Value = "64.4"},
            new FreqVal {Frequency = 4000,Value = "66.4"},
            new FreqVal {Frequency = 6000,Value = "61.0"},
            new FreqVal {Frequency = 8000,Value = "63.8"},
            new FreqVal {Frequency = 12500,Value = "75.5"},

        };

        public SerializableFreqObservableCollection LTASS { get; } = new SerializableFreqObservableCollection
        {
            new FreqVal {Frequency = 250,Value = "45.6"},
            new FreqVal {Frequency = 500,Value = "43.9"},
            new FreqVal {Frequency = 750,Value = "40.5"},
            new FreqVal {Frequency = 1000,Value = "37.5"},
            new FreqVal {Frequency = 1500,Value = "34.4"},
            new FreqVal {Frequency = 2000,Value = "35.4"},
            new FreqVal {Frequency = 3000,Value = "34.4"},
            new FreqVal {Frequency = 4000,Value = "37.4"},
            new FreqVal {Frequency = 6000,Value = "39.0"},
            new FreqVal {Frequency = 8000,Value = "41.8"},
            new FreqVal {Frequency = 12500,Value = "46.5"},

        };

        public SerializableFreqObservableCollection SPLTresholds { get; } = new SerializableFreqObservableCollection
        {
            new FreqVal {Frequency = 250,Value = "19.5"},
            new FreqVal {Frequency = 500,Value = "16.2"},
            new FreqVal {Frequency = 750,Value = "18.5"},
            new FreqVal {Frequency = 1000,Value = "23.3"},
            new FreqVal {Frequency = 1500,Value = "27.5"},
            new FreqVal {Frequency = 2000,Value = "34.9"},
            new FreqVal {Frequency = 3000,Value = "39.0"},
            new FreqVal {Frequency = 4000,Value = "45.0"},
            new FreqVal {Frequency = 6000,Value = "50.0"},
            new FreqVal {Frequency = 8000,Value = "41.8"},
            new FreqVal {Frequency = 12500,Value = "42.1"},

        };


        public SerializableFreqObservableCollection UCL { get; } = new SerializableFreqObservableCollection
        {
            new FreqVal {Frequency = 250,Value = "117.5"},
            new FreqVal {Frequency = 500,Value = "112.2"},
            new FreqVal {Frequency = 750,Value = "112.5"},
            new FreqVal {Frequency = 1000,Value = "117.3"},
            new FreqVal {Frequency = 1500,Value = "122.5"},
            new FreqVal {Frequency = 2000,Value = "122.9"},
            new FreqVal {Frequency = 3000,Value = "125.0"},
            new FreqVal {Frequency = 4000,Value = "129.0"},
            new FreqVal {Frequency = 6000,Value = "132.0"},
            new FreqVal {Frequency = 8000,Value = "136.8"},
            new FreqVal {Frequency = 12500,Value = "120.1"}
        };

        public SerializableFreqObservableCollection RECD { get; } = new SerializableFreqObservableCollection
        {
            new FreqVal {Frequency = 250,Value = "117.5"},
            new FreqVal {Frequency = 500,Value = "112.2"},
            new FreqVal {Frequency = 750,Value = "112.5"},
            new FreqVal {Frequency = 1000,Value = "117.3"},
            new FreqVal {Frequency = 1500,Value = "122.5"},
            new FreqVal {Frequency = 2000,Value = "122.9"},
            new FreqVal {Frequency = 3000,Value = "125.0"},
            new FreqVal {Frequency = 4000,Value = "129.0"},
            new FreqVal {Frequency = 6000,Value = "132.0"},
            new FreqVal {Frequency = 8000,Value = "136.8"},
            new FreqVal {Frequency = 12500,Value = "120.1"}
        };

        public string MeasurementTime
        {
            get => this.measurementTime;
            set
            {
                if (int.TryParse(value, out int time) || string.IsNullOrEmpty(value))
                {
                    Set(ref this.measurementTime, value);
                }
            }
        }

        public string EqualizationTime
        {
            get => this.equalizationTime;
            set
            {
                if (int.TryParse(value, out int time) || string.IsNullOrEmpty(value))
                {
                    Set(ref this.equalizationTime, value);
                }
            }
        }

        public string AidedSII
        {
            get => this.aidedSII;
            set
            {
                if (int.TryParse(value, out int time) || string.IsNullOrEmpty(value))
                {
                    Set(ref this.aidedSII, value);
                }
            }
        }

        public string UnaidedSII
        {
            get => this.unaidedSII;
            set
            {
                if (int.TryParse(value, out int time) || string.IsNullOrEmpty(value))
                {
                    Set(ref this.unaidedSII, value);
                }
            }
        }

        public string VerifitFirmwareVersion
        {
            get => this.verifitFirmwareVersion;
            set
            {
                // Application should handle grecefully even if version is in bad format
                Set(ref this.verifitFirmwareVersion, value);

                Version symver = new Version();
                IsFirmwareVersionValid = Version.TryParse(value, out symver);

                this.appConfigManager.SaveFirmwareVersion(value);
            }
        }

        private bool isFirmwareVersionValid;
        public bool IsFirmwareVersionValid
        {
            get => this.isFirmwareVersionValid;
            set => Set(ref this.isFirmwareVersionValid, value);
        }

        private bool randomizeOutput = false;
        public bool RandomizeOutput
        {
            get => this.randomizeOutput;
            set => Set(ref this.randomizeOutput, value);
        }

        public IDataItem SelectedVerifitModel
        {
            get
            {
                var verifitVersion = VerifitVersions.SingleOrDefault(x => x.IsChecked);
                if (verifitVersion == null)
                {
                    return new Setting { Value = UIBridgeConstants.Verifit2, LocalizedName = "Verifit 2" };
                }

                return verifitVersion;
            }
        }
    }
}