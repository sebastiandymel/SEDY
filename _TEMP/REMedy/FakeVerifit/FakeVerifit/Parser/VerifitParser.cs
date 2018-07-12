using System;
using System.Text;

namespace FakeVerifit
{
    class VerifitParser
    {
        private static Side ParseSide(string sideString)
        {
            sideString = sideString.ToLowerInvariant();
            if (sideString == "left")
            {
                return Side.Left;
            }
            else if (sideString == "right")
            {
                return Side.Right;
            }

            throw new ArgumentException($"{nameof(sideString)} is out of range of {nameof(Side)}");
        }

        /// <summary>
        /// <c>calibrated rem left</c>
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>

        public IsCalibratedData ParseIsCalibratedCommand(string line)
        {
            var data = new IsCalibratedData();

            var tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            data.Mode = tokens[1];
            data.Side = ParseSide(tokens[2]);

            return data;
        }

        /// <summary>
        /// <c>setTest speechmap srem right 2 speech-wbcarrots avg65</c>
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public SetTestData ParseSetTestCommand(string line)
        {
            var data = new SetTestData();

            var tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            data.Mode = tokens[2];
            data.Side = ParseSide(tokens[3]);
            data.SlotNumber = tokens[4];
            data.StimulusName = tokens[5];
            data.Level = tokens[6];

            return data;
        }

        /// <summary>
        /// <code>
        /// setClient speechmap rem -target dsladult -languagetype nontonal  -transducer headphone -leftinstrument rite -leftventing occluding 
        /// -rightinstrument rite -rightventing occluding -binaural no -age 480  -lefthl { 2.0 6.0 10.0 14.0 18.0 22.0 26.0 30.0 34.0 38.0 42.0 46.0 } 
        /// -leftbc { 0.0 2.0 4.0 6.0 8.0 10.0 12.0 14.0 16.0 18.0 20.0 22.0 } -leftucl { 100.0 102.0 104.0 106.0 108.0 110.0 112.0 114.0 116.0 118.0 120.0 122.0 } 
        /// -righthl { 2.0 6.0 10.0 14.0 18.0 22.0 26.0 30.0 34.0 38.0 42.0 46.0 }
        /// </code>
        /// </summary>
        /// <param name="commandLine"></param>
        /// <returns></returns>
        public SetClientData ParseSetClientCommand(string commandLine)
        {
            var data = new SetClientData();

            var tokens = commandLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens[0] != "setClient")
            {
                throw new Exception();
            }

            data.Mode = tokens[2];

            for (int i = 3; i < tokens.Length; i++)
            {
                string key = tokens[i];
                string value = tokens[++i];

                if (value == "{")
                {
                    StringBuilder vectorBuilder = new StringBuilder();

                    vectorBuilder.Append(value);
                    while (tokens[++i] != "}")
                    {
                        vectorBuilder.Append($" {tokens[i]}");
                    }
                    vectorBuilder.Append($" {tokens[i]}");

                    value = vectorBuilder.ToString();
                }

                // AudioScanAPI do not ensures if  each parameter will send only once. Let's grab last value which was sent to us.
                data.Parameters[key] = value;
            }

            return data;
        }
    }
}
